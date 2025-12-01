using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuConfirm : MonoBehaviour
{
    public void ConfirmBackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
