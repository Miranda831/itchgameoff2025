using UnityEngine;

/// <summary>
/// 挂在【物体 A】上的脚本：
/// - 只有当 A 不再移动时，Player 碰到才生效
/// - 碰到后：销毁 A，自身；启用物体 B
/// </summary>
public class PickupAndEnable : MonoBehaviour
{
    [Header("玩家Tag")]
    public string playerTag = "Player";

    [Header("移动控制脚本（触发区域上的脚本）")]
    public StayEnableAndMoveUp moveScript;   // 场景里那个挂了 StayEnableAndMoveUp 的对象

    [Header("碰触后要开启的物体 B")]
    public GameObject objectToEnable;

    [Header("是否只触发一次")]
    public bool triggerOnce = true;

    private bool picked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (triggerOnce && picked) return;

        // ⭐ 核心：只有在不移动的时候才能被拾取
        if (moveScript != null && moveScript.IsMoving())
        {
            // 还在移动，不允许拾取
            return;
        }

        PickUp();
    }

    void PickUp()
    {
        picked = true;

        // 启用物体 B
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠ PickupAndEnable：objectToEnable(物体 B) 没有设置！");
        }

        // 销毁自己（物体 A）
        Destroy(gameObject);
    }
}
