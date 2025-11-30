using UnityEngine;
using MoreMountains.CorgiEngine;

public class CorgiStartGame : MonoBehaviour
{
    [Header("Corgi 的 InputManager")]
    public InputManager inputManager;   // 拖 Corgi 自带 InputManager

    [Header("开始界面 Panel")]
    public GameObject startPanel;       // 拖开始界面（可选）

    [Header("是否在开始游戏时启动计时器")]
    public bool startTimerOnGameStart = true;

    void Start()
    {
        // 开局锁输入（玩家不能动）
        if (inputManager != null)
        {
            inputManager.InputDetectionActive = false;
        }
    }

    // 给按钮调用
    public void StartGame()
    {
        // 解锁输入
        if (inputManager != null)
        {
            inputManager.InputDetectionActive = true;
        }

        // 隐藏 UI
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // 开始计时
        if (startTimerOnGameStart && RunTimeManager.Instance != null)
        {
            RunTimeManager.Instance.StartRun();
        }
    }
}
