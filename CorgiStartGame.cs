using UnityEngine;
using MoreMountains.CorgiEngine;

public class CorgiStartGame : MonoBehaviour
{
    public InputManager inputManager;
    public GameObject startPanel;
    public bool startTimerOnGameStart = true;

    void Start()
    {
        if (inputManager != null)
            inputManager.InputDetectionActive = false;
    }

    // 给按钮调用
    public void StartGame()
    {
        if (inputManager != null)
            inputManager.InputDetectionActive = true;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (startTimerOnGameStart && RunTimeManager.Instance != null)
            RunTimeManager.Instance.StartRun();
    }
}
