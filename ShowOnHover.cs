using UnityEngine;
using UnityEngine.EventSystems;

public class ShowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("鼠标滑过时要显示的物体（文字 + 发光）")]
    public GameObject target;

    private void Start()
    {
        if (target != null)
            target.SetActive(false);  // 开始先隐藏
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (target != null)
            target.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (target != null)
            target.SetActive(false);
    }
}
