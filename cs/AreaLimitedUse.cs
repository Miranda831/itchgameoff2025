using UnityEngine;

public class AreaTriggerVine : MonoBehaviour
{
    [Header("玩家 Tag")]
    public string playerTag = "Player";

    [Header("要激活的 VineLiftController 对象")]
    public VineLiftController vineLift;   // 把藤蔓平台拖进来

    private void Awake()
    {
        if (vineLift != null)
        {
            vineLift.enabled = false;  // 默认禁用
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // 玩家进入 → 激活
            if (vineLift != null)
                vineLift.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // 玩家离开 → 关闭
            if (vineLift != null)
                vineLift.enabled = false;
        }
    }
}
