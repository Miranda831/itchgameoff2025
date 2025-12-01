using UnityEngine;

public class CoverAreaFlexible : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("进入这个区域时要关闭的物体")]
    public GameObject[] objectsToDisable;

    [Header("进入这个区域时要开启的物体")]
    public GameObject[] objectsToEnable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // 关
        foreach (var obj in objectsToDisable)
            if (obj != null) obj.SetActive(false);

        // 开
        foreach (var obj in objectsToEnable)
            if (obj != null) obj.SetActive(true);
    }
}
