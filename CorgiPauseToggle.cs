using UnityEngine;
using MoreMountains.CorgiEngine;

public class CorgiPauseManager : MonoBehaviour
{
    [Header("Corgi Input Manager")]
    public InputManager inputManager;

    [Header("开始界面（StartPanel）")]
    public GameObject startPanel;

    [Header("暂停界面（PausePanel）")]
    public GameObject pausePanel;

    [Header("ESC 优先关闭的 UI（不要放 StartPanel / PausePanel）")]
    public GameObject[] panelsToClose;

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // 只要 StartPanel 还在，就**强制保证时间是 1**
        if (IsStartPanelActive() && Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
            if (inputManager != null)
                inputManager.InputDetectionActive = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    // 用 activeInHierarchy，父物体被关也能检测到
    bool IsStartPanelActive()
    {
        return startPanel != null && startPanel.activeInHierarchy;
    }

    void HandleEscape()
    {
        // ===== ① StartPanel 还在：ESC 只关其它 UI，不 Pause、不 Resume =====
        if (IsStartPanelActive())
        {
            if (CloseOtherPanels())
            {
                // 关掉一个 panel 就结束
                return;
            }

            // StartPanel 在，又没有其它 panel → ESC 什么都不做
            return;
        }

        // ===== ② StartPanel 已经关掉（游戏开始） =====

        // 先尝试关其它 panel（RecordPanel / SettingPanel 等）
        if (CloseOtherPanels())
        {
            return;
        }

        // 再处理 Pause / Resume
        if (pausePanel != null && pausePanel.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// 关闭 panelsToClose 中当前激活的第一个，返回是否真的关掉了一个
    /// </summary>
    bool CloseOtherPanels()
    {
        if (panelsToClose == null) return false;

        foreach (GameObject panel in panelsToClose)
        {
            if (panel != null && panel.activeSelf)
            {
                panel.SetActive(false);
                return true;
            }
        }
        return false;
    }

    // ---------- 给按钮用：暂停 / 恢复 ----------

    public void PauseGame()
    {
        // ❗ 有 StartPanel 的时候，无论谁调用 PauseGame 都直接无效
        if (IsStartPanelActive())
            return;

        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;

        if (inputManager != null)
            inputManager.InputDetectionActive = false;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (inputManager != null)
            inputManager.InputDetectionActive = true;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
}
