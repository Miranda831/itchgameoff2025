using UnityEngine;
using MoreMountains.CorgiEngine;

public class CorgiPauseManager : MonoBehaviour
{
    [Header("Corgi Input Manager")]
    public InputManager inputManager;   // 拖 Corgi 的 InputManager

    [Header("开始界面（StartPanel）")]
    public GameObject startPanel;       // 拖 StartPanel

    [Header("暂停界面（PausePanel）")]
    public GameObject pausePanel;       // 拖 PausePanel

    [Header("ESC 优先关闭的 UI（不要放 StartPanel / PausePanel）")]
    public GameObject[] panelsToClose;  // 例如：RecordPanel、SettingPanel 等

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    void HandleEscape()
    {
        bool startActive = (startPanel != null && startPanel.activeSelf);

        // ===== ① StartPanel 还在：ESC 只关其它 UI，不 Pause、不 Resume =====
        if (startActive)
        {
            if (panelsToClose != null)
            {
                foreach (GameObject panel in panelsToClose)
                {
                    if (panel != null && panel.activeSelf)
                    {
                        // 只关 UI，不 Resume、不 Pause
                        panel.SetActive(false);
                        return;
                    }
                }
            }

            // StartPanel 在，又没有其它 panel → ESC 什么都不做
            return;
        }

        // ===== ② StartPanel 关闭（游戏开始） =====

        // 2-1 先关其它 UI（RecordPanel / SettingPanel），不 Resume
        if (panelsToClose != null)
        {
            foreach (GameObject panel in panelsToClose)
            {
                if (panel != null && panel.activeSelf)
                {
                    panel.SetActive(false);
                    // 不 Resume
                    return;
                }
            }
        }

        // ===== ③ 没有其它 panel 打开 =====

        // 如果 PausePanel 是开的 → ESC 用于 Resume
        if (pausePanel != null && pausePanel.activeSelf)
        {
            ResumeGame();
            return;
        }

        // 如果 PausePanel 是关的 → ESC 用于 Pause（唤出 PausePanel）
        PauseGame();
    }

    // ---------- 给按钮用：暂停 / 恢复 ----------

    public void PauseGame()
    {
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
