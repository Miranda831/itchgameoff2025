using UnityEngine;

public class SwitchCameraOnTrigger : MonoBehaviour
{
    [Header("关闭的 Camera")]
    public GameObject camera1;

    [Header("开启的 Camera")]
    public GameObject camera2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (camera1 != null) camera1.SetActive(false);
        if (camera2 != null) camera2.SetActive(true);

        Debug.Log("Camera switched!");
    }
}
