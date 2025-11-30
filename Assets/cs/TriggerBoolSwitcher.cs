using UnityEngine;

public class TriggerBoolSwitcher : MonoBehaviour
{
    [Header("Player Tag")]
    public string playerTag = "Player";

    [Header("Animator")]
    public Animator animator;                 // 拖你的 Animator 进来
    public string enterBoolParam = "IsIn";    // Animator 里的 Bool 参数名
    public string exitTriggerParam = "Exit";  // Animator 里的 Trigger 参数名（可选）

    [Header("Debug Bools (optional)")]
    public bool onEnterBool = false;   // 只是给你 Inspector 看状态用
    public bool onExitBool = false;

    private void Reset()
    {
        // 自动尝试拿本物体 Animator，省得你忘拖
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (animator == null) return;

        onEnterBool = true;
        onExitBool = false;

        // ✅ 真正驱动动画
        animator.SetBool(enterBoolParam, true);

        Debug.Log("Player ENTER → SetBool(IsIn,true)");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (animator == null) return;

        onEnterBool = false;
        onExitBool = true;

        // ✅ 离开时关掉 enterBool
        animator.SetBool(enterBoolParam, false);

        // ✅ 如果你想离开播一次动画，用 Trigger
        if (!string.IsNullOrEmpty(exitTriggerParam))
            animator.SetTrigger(exitTriggerParam);

        Debug.Log("Player EXIT → SetBool(IsIn,false) + Trigger(Exit)");
    }
}
