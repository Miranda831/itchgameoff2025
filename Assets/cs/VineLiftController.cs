using UnityEngine;
using MoreMountains.CorgiEngine;

/// <summary>
/// 藤蔓升降平台：
/// - 平台上下移动（上升速度、下降速度可分开调）
/// - 绑定一条从“缩在下面”到“升到上面”的动画（animStateName）
/// - 动画上升、下降速度可分开调
/// - 升降过程中自动锁定 Corgi Player（不能移动 / 跳 / 蹲 / 跑，水平不动）
///   ✅ 现在改成：只有玩家在这个平台的 Trigger 里时才锁定，离开 Trigger 就解锁
/// </summary>
public class VineLiftController : MonoBehaviour
{
    [Header("平台移动")]
    [Tooltip("平台从底到顶上升的高度（世界坐标）")]
    public float moveDistance = 2f;

    [Tooltip("从底到顶完整走一趟需要的时间（秒），作为基础时间")]
    public float moveTime = 1f;

    [Tooltip("平台【上升】速度倍率（>1 更快，<1 更慢）")]
    public float platformUpSpeed = 1f;

    [Tooltip("平台【下降】速度倍率（>1 更快，<1 更慢）")]
    public float platformDownSpeed = 1f;

    [Header("动画绑定")]
    [Tooltip("藤蔓/叶子的 Animator（拖那个有 RaiseLeaves 动画的）")]
    public Animator animator;

    [Tooltip("那条从底到顶的动画状态名，比如 RaiseLeaves")]
    public string animStateName = "RaiseLeaves";

    [Tooltip("动画【上升】速度倍率")]
    public float animUpSpeed = 1f;

    [Tooltip("动画【下降】速度倍率")]
    public float animDownSpeed = 1f;

    [Header("输入")]
    [Tooltip("测试用按键（后面可以改成触发器调用 ToggleDirection）")]
    public KeyCode key = KeyCode.K;

    [Header("自动查找玩家设置")]
    public string playerTag = "Player";

    // ---- 平台内部状态 ----
    private Vector3 bottomPos;
    private Vector3 topPos;

    // 0 = 底, 1 = 顶
    private float progress = 0f;

    // +1 上升, -1 下降, 0 停止
    private int direction = 0;

    // 最近一次运动方向（动画区分上下用）
    private int lastMoveDir = 1;

    // 基础速度（不含倍率）
    private float baseSpeed;

    // 用来判断“上一次是不是处于锁定状态”（原来叫 wasMoving，继续复用）
    private bool wasMoving = false;

    // ✅ 玩家是否在当前平台的 Trigger 内
    private bool _playerInTrigger = false;

    // ---- Corgi 玩家相关 ----
    private bool _playerCached = false;
    private GameObject _playerGO;
    private CorgiController _controller;
    private CharacterHorizontalMovement _horizontal;
    private CharacterJump _jump;
    private CharacterCrouch _crouch;
    private CharacterRun _run;
    private Rigidbody2D _rb;
    private RigidbodyConstraints2D _originalConstraints;

    private void Awake()
    {
        // ✅ 允许 BoxCollider2D 或 PolygonCollider2D 任意一个
        bool hasBox = GetComponent<BoxCollider2D>() != null;
        bool hasPoly = GetComponent<PolygonCollider2D>() != null;

        if (!hasBox && !hasPoly)
        {
            Debug.LogError("[VineLiftController] 需要挂一个 BoxCollider2D 或 PolygonCollider2D！");
        }

        bottomPos = transform.position;
        topPos = bottomPos + Vector3.up * moveDistance;

        baseSpeed = (moveTime > 0f && moveTime > Mathf.Epsilon) ? 1f / moveTime : 1f;
    }

    private void Update()
    {
        // 测试用按键，也可以不用，在外部 trigger 调 ToggleDirection()
        if (Input.GetKeyDown(key))
        {
            ToggleDirection();
        }

        UpdateProgress(Time.deltaTime);
        UpdatePlatformPosition();
        UpdateAnimation();
        UpdatePlayerLock();
    }

    /// <summary>
    /// 允许外部脚本调用的切换方法（比如 trigger 碰到时）
    /// </summary>
    public void ToggleDirection()
    {
        // 完全在底部：开始向上
        if (Mathf.Approximately(progress, 0f))
        {
            direction = 1;
            lastMoveDir = 1;
            return;
        }

        // 完全在顶部：开始向下
        if (Mathf.Approximately(progress, 1f))
        {
            direction = -1;
            lastMoveDir = -1;
            return;
        }

        // 中途：直接反向
        direction = -direction;
        if (direction != 0)
        {
            lastMoveDir = direction;
        }
    }

    private void UpdateProgress(float dt)
    {
        if (direction == 0) return;

        float dirMultiplier = direction > 0 ? platformUpSpeed : platformDownSpeed;
        float realSpeed = baseSpeed * dirMultiplier;

        progress += direction * realSpeed * dt;
        progress = Mathf.Clamp01(progress);

        // 到达两端就停
        if (Mathf.Approximately(progress, 0f) || Mathf.Approximately(progress, 1f))
        {
            direction = 0;
        }
    }

    private void UpdatePlatformPosition()
    {
        transform.position = Vector3.Lerp(bottomPos, topPos, progress);
    }

    private void UpdateAnimation()
    {
        if (animator == null || string.IsNullOrEmpty(animStateName))
            return;

        // 用平台 progress 做基础
        float t = progress;

        // 上升 / 下降 对应不同动画倍率
        float animMultiplier = lastMoveDir >= 0 ? animUpSpeed : animDownSpeed;
        float animT = Mathf.Clamp01(t * animMultiplier);

        animator.Play(animStateName, 0, animT);
        animator.speed = 0f;   // 由脚本控制时间轴
    }

    /// <summary>
    /// ✅ 只在“平台有在动 && 玩家在 Trigger 里”的时候锁定玩家
    /// </summary>
    private void UpdatePlayerLock()
    {
        bool isMoving = direction != 0;

        // 只有平台在动 且 玩家在这个平台触发区里，才需要锁定
        bool shouldLock = isMoving && _playerInTrigger;

        if (shouldLock && !wasMoving)
        {
            // 刚进入“需要锁定”的状态
            LockPlayer(true);
        }
        else if (!shouldLock && wasMoving)
        {
            // 刚从“需要锁定”状态退出（比如平台停了 / 玩家离开触发）
            LockPlayer(false);
        }

        // 这里其实变成“上一次是否处于锁定状态”
        wasMoving = shouldLock;
    }

    // ===================== Trigger 检测（关键新增部分） =====================

    private bool IsPlayerCollider(Collider2D other)
    {
        // 先按 Tag 判
        if (!string.IsNullOrEmpty(playerTag) && other.CompareTag(playerTag))
            return true;

        // 有些时候 Tag 在父物体上，就用 CorgiController 判一下
        return other.GetComponentInParent<CorgiController>() != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            _playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            _playerInTrigger = false;
            // 玩家离开触发，立刻解锁一次，避免残留锁定
            LockPlayer(false);
        }
    }

    // ===================== 玩家锁定逻辑 =====================

    private void CachePlayer()
    {
        if (_playerCached) return;

        // 优先按 Tag 找
        if (!string.IsNullOrEmpty(playerTag))
        {
            _playerGO = GameObject.FindGameObjectWithTag(playerTag);
        }

        // 找不到就按 CorgiController 找
        if (_playerGO == null)
        {
            var controller = FindObjectOfType<CorgiController>();
            if (controller != null)
            {
                _playerGO = controller.gameObject;
            }
        }

        if (_playerGO == null)
        {
            Debug.LogWarning("[VineLiftController] 找不到 Player，锁定功能不会生效。");
            _playerCached = true; // 避免每帧都找
            return;
        }

        _controller = _playerGO.GetComponent<CorgiController>();
        _horizontal = _playerGO.GetComponent<CharacterHorizontalMovement>();
        _jump = _playerGO.GetComponent<CharacterJump>();
        _crouch = _playerGO.GetComponent<CharacterCrouch>();
        _run = _playerGO.GetComponent<CharacterRun>();
        _rb = _playerGO.GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _originalConstraints = _rb.constraints;
        }

        _playerCached = true;
    }

    /// <summary>
    /// true = 锁定玩家，false = 解锁玩家
    /// </summary>
    private void LockPlayer(bool locked)
    {
        CachePlayer();
        if (!_playerCached || _playerGO == null) return;

        if (locked)
        {
            // 1️⃣ 刚体：冻结水平，保留竖直（方便平台顶着往上）
            if (_rb != null)
            {
                _rb.velocity = Vector2.zero;
                _originalConstraints = _rb.constraints;
                _rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }

            // 2️⃣ CorgiController：清除当前速度
            if (_controller != null)
            {
                _controller.SetHorizontalForce(0f);
                _controller.SetVerticalForce(0f);
            }

            // 3️⃣ 禁用左右移动 / 跳 / 蹲 / 跑 输入
            if (_horizontal != null) _horizontal.enabled = false;
            if (_jump != null) _jump.enabled = false;
            if (_crouch != null) _crouch.enabled = false;
            if (_run != null) _run.enabled = false;
        }
        else
        {
            // 解锁：还原刚体约束
            if (_rb != null)
            {
                _rb.constraints = _originalConstraints;
            }

            // 重新启用移动相关脚本
            if (_horizontal != null) _horizontal.enabled = true;
            if (_jump != null) _jump.enabled = true;
            if (_crouch != null) _crouch.enabled = true;
            if (_run != null) _run.enabled = true;
        }
    }
}
