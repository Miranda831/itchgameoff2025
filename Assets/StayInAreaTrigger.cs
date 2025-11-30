using UnityEngine;

/// <summary>
/// 挂在【触发区域】上的脚本：
/// - Player 在区域内停留 stayTime 秒
/// - 启用 objectToEnable（物体 A）
/// - 让物体 A 向上移动 moveUpDistance
/// </summary>
public class StayEnableAndMoveUp : MonoBehaviour
{
    [Header("玩家设置")]
    public string playerTag = "Player";

    [Header("停留触发时间")]
    public float stayTime = 2f;   // 在区域内停留X秒

    [Header("目标物体（物体 A）")]
    public GameObject objectToEnable;   // 要启用并移动的物体 A

    [Header("上移动设置")]
    public float moveUpDistance = 2f;   // 向上移动距离（世界单位）
    public float moveSpeed = 2f;        // 移动速度

    [Header("是否只触发一次")]
    public bool triggerOnlyOnce = true;

    private float timer = 0f;
    private bool triggered = false;
    private bool moving = false;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        if (objectToEnable != null)
        {
            startPos = objectToEnable.transform.position;
            targetPos = startPos;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (triggerOnlyOnce && triggered) return;

        timer += Time.deltaTime;

        if (timer >= stayTime)
        {
            TriggerAction();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // 离开清空计时
        timer = 0f;
    }

    void TriggerAction()
    {
        if (triggerOnlyOnce && triggered) return;

        triggered = true;

        if (objectToEnable == null)
        {
            Debug.LogWarning("⚠ StayEnableAndMoveUp：没有设置 objectToEnable");
            return;
        }

        // 启用物体
        objectToEnable.SetActive(true);

        // 记录起点和终点
        startPos = objectToEnable.transform.position;
        targetPos = startPos + Vector3.up * moveUpDistance;

        moving = true;
    }

    void Update()
    {
        if (!moving || objectToEnable == null) return;

        objectToEnable.transform.position =
            Vector3.MoveTowards(objectToEnable.transform.position, targetPos, moveSpeed * Time.deltaTime);

        // 到点停止
        if (Vector3.Distance(objectToEnable.transform.position, targetPos) < 0.01f)
        {
            objectToEnable.transform.position = targetPos;
            moving = false;
        }
    }

    /// <summary>
    /// 提供给其它脚本判断：是否还在移动中
    /// </summary>
    public bool IsMoving()
    {
        return moving;
    }
}
