using UnityEngine;
using MoreMountains.CorgiEngine;

public class VerticalAreaSwitcherCorgi : MonoBehaviour
{
    public GameObject topCover;      // 上面的黑块
    public GameObject bottomCover;   // 下面的黑块
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        CorgiController controller = other.GetComponentInParent<CorgiController>();
        if (controller == null) return;

        float vy = controller.Speed.y;

        if (vy < 0f)
        {
            // 上 → 下
            Debug.Log("Corgi Player from TOP to BOTTOM");
            if (topCover != null) topCover.SetActive(true);
            if (bottomCover != null) bottomCover.SetActive(false);
        }
        else if (vy > 0f)
        {
            // 下 → 上
            Debug.Log("Corgi Player from BOTTOM to TOP");
            if (topCover != null) topCover.SetActive(false);
            if (bottomCover != null) bottomCover.SetActive(true);
        }
    }

    // ⭐ 让复活脚本可以控制
    public void ForceTop()
    {
        Debug.Log("[VerticalAreaSwitcher] 强制回到上地图");
        if (topCover != null) topCover.SetActive(false);  // 上地图显示
        if (bottomCover != null) bottomCover.SetActive(true); // 下地图遮住
    }
}
