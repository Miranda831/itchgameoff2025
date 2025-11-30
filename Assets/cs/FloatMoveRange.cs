using UnityEngine;

/// 漂浮 + 左右来回移动的平台（刚体版，适合托住玩家和 Pushable）
[RequireComponent(typeof(Rigidbody2D))]
public class FloatMoveRange : MonoBehaviour
{
    [Header("左右移动范围（相对初始位置）")]
    public float leftDistance = 1f;   // 向左最远
    public float rightDistance = 1f;  // 向右最远

    [Header("左右移动速度")]
    public float moveSpeed = 1f;

    [Header("上下浮动")]
    public float floatHeight = 0.2f;
    public float floatSpeed = 2f;

    // 这一帧平台的水平位移（可选，给别的脚本用）
    public float PlatformVelocityX { get; private set; }

    private Rigidbody2D _rb;
    private Vector2 _startPos;
    private bool _movingRight = true;
    private float _lastX;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true;      // 关键：漂浮板是 kinematic
    }

    private void Start()
    {
        _startPos = _rb.position;
        _lastX = _startPos.x;
    }

    // 用 FixedUpdate + MovePosition，和物理系统同步
    private void FixedUpdate()
    {
        // 上下浮动
        float y = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // 目标 X（左右来回）
        float targetX = _movingRight
            ? _startPos.x + rightDistance
            : _startPos.x - leftDistance;

        // 当前 X
        float currentX = _rb.position.x;

        // 朝目标匀速移动
        float newX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.fixedDeltaTime);

        // 计算这一帧平台水平位移
        PlatformVelocityX = newX - _lastX;
        _lastX = newX;

        // 通过刚体移动，而不是直接改 transform
        Vector2 newPos = new Vector2(newX, y);
        _rb.MovePosition(newPos);

        // 到边界就掉头
        if (Mathf.Abs(newX - targetX) < 0.01f)
        {
            _movingRight = !_movingRight;
        }
    }
}
