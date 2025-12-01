using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHoverClickColor : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("要变色的文字")]
    public TMP_Text targetText;

    [Header("默认状态颜色")]
    public Color normalColor = Color.white;

    [Header("鼠标滑过颜色")]
    public Color hoverColor = Color.yellow;

    [Header("按下时颜色")]
    public Color pressedColor = Color.red;

    [Header("按下后恢复默认颜色的时间（秒）")]
    public float pressedDuration = 0.2f;   // 你可以在 Inspector 里调这个

    private bool _isHovering = false;
    private bool _isPressing = false;
    private Coroutine _pressRoutine;

    void Start()
    {
        ResetColor();
    }

    // 每次 Panel 重新 SetActive(true) 时都会调用
    void OnEnable()
    {
        ResetColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;

        // 如果当前没有在“按下计时中”，才改成 hover 色
        if (!_isPressing && targetText != null)
            targetText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;

        // 如果当前没有在“按下计时中”，才改回默认色
        if (!_isPressing && targetText != null)
            targetText.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetText == null) return;

        _isPressing = true;

        // 先立刻切到按下颜色
        targetText.color = pressedColor;

        // 如果之前有计时协程，先停掉
        if (_pressRoutine != null)
            StopCoroutine(_pressRoutine);

        // 开一个新的倒计时，X 秒后自动恢复默认色
        _pressRoutine = StartCoroutine(PressTimer());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 这里不直接改颜色，让协程管
        _isPressing = false;
    }

    private System.Collections.IEnumerator PressTimer()
    {
        yield return new WaitForSeconds(pressedDuration);

        // 时间到了，强制回到默认颜色（不管鼠标还在不在上面）
        if (targetText != null)
            targetText.color = normalColor;

        _isPressing = false;
        _pressRoutine = null;
    }

    // 给外部 / OnEnable 调用的统一重置方法
    public void ResetColor()
    {
        _isHovering = false;
        _isPressing = false;

        if (_pressRoutine != null)
        {
            StopCoroutine(_pressRoutine);
            _pressRoutine = null;
        }

        if (targetText != null)
            targetText.color = normalColor;
    }
}
