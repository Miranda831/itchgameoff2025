using UnityEngine;

public class HideAndActiveTrigger : MonoBehaviour
{
    [Header("玩家 Tag")]
    public string playerTag = "Player";

    [Header("要被启用/禁用的目标物体")]
    public GameObject targetObject;   // 把你要隐藏/显示的 object 拖进来

    private void Awake()
    {
        if (targetObject != null)
        {
            // 默认关闭
            targetObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (targetObject != null)
                targetObject.SetActive(true);   // 进入 → 显示
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (targetObject != null)
                targetObject.SetActive(false);  // 离开 → 隐藏
        }
    }
}
