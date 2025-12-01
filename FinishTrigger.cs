using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("玩家 Tag")]
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (RunTimeManager.Instance != null)
        {
            RunTimeManager.Instance.StopRunAndRecord();
        }

        Debug.Log("🏁 到达终点，计时停止并记录到排行榜");
    }
}
