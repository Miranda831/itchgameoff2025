using System.Collections;
using UnityEngine;
using TMPro;

public class WorldBubbleDialogueSafe : MonoBehaviour
{
    [Header("引用")]
    public TMP_Text dialogueText;
    public SpriteRenderer bubbleRenderer;

    [Header("三角形指针")]
    public Transform pointer;
    [Tooltip("三角形离气泡右边缩进多少")]
    public float pointerRightOffset = 0.2f;
    [Tooltip("三角形离气泡底边距离")]
    public float pointerBottomOffset = 0.05f;

    [Header("对话内容")]
    [TextArea(2, 4)]
    public string[] lines;

    [Header("播放设置")]
    public float interval = 2f;
    public int playCount = 5;
    public bool loopIfShort = true;

    [Header("永远循环")]
    public bool loopForever = false;

    [Header("随机播放")]
    [Tooltip("勾上后：一开始随机，之后也随机")]
    public bool randomizeOrder = false;

    [Header("尺寸设置(世界单位)")]
    public float paddingX = 0.6f;
    public float paddingY = 0.35f;
    public float maxWidth = 6f;
    public float minWidth = 1.2f;
    public float minHeight = 0.8f;

    [Header("背景贴合系数")]
    public float widthFactor = 1f;
    public float heightFactor = 1f;

    private void OnEnable()
    {
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        if (dialogueText == null || bubbleRenderer == null || lines == null || lines.Length == 0)
            yield break;

        // ✅ 这里：一开始就随机
        int index = randomizeOrder
            ? Random.Range(0, lines.Length)
            : 0;

        // ========== 永远循环 ==========
        if (loopForever)
        {
            while (true)
            {
                dialogueText.text = lines[index];

                yield return null;
                ResizeBubble();
                yield return new WaitForSeconds(interval);

                index = GetNextIndex(index, lines.Length);
            }
        }

        // ========== 固定次数 ==========
        int shown = 0;
        while (shown < playCount)
        {
            dialogueText.text = lines[index];

            yield return null;
            ResizeBubble();
            yield return new WaitForSeconds(interval);

            shown++;
            index = GetNextIndex(index, lines.Length);
        }
    }

    // 根据 randomizeOrder 决定下一句怎么来
    int GetNextIndex(int currentIndex, int length)
    {
        if (length <= 1) return 0;

        // 顺序模式
        if (!randomizeOrder)
            return (currentIndex + 1) % length;

        // 随机模式（避免连着重复）
        int next = currentIndex;
        int safety = 0;
        while (next == currentIndex && safety < 10)
        {
            next = Random.Range(0, length);
            safety++;
        }
        return next;
    }

    void ResizeBubble()
    {
        dialogueText.ForceMeshUpdate();

        Vector2 localSize = dialogueText.GetRenderedValues(false);

        float textW = localSize.x * dialogueText.transform.lossyScale.x;
        float textH = localSize.y * dialogueText.transform.lossyScale.y;

        float targetW = Mathf.Clamp(textW + paddingX, minWidth, maxWidth) * widthFactor;
        float targetH = Mathf.Max(textH + paddingY, minHeight) * heightFactor;

        bubbleRenderer.size = new Vector2(targetW, targetH);

        // ======== 三角形跟右下角 ========
        if (pointer != null)
        {
            float halfW = targetW * 0.5f;
            float halfH = targetH * 0.5f;

            float pointerX = halfW - pointerRightOffset;
            float pointerY = -halfH - pointerBottomOffset;

            Vector3 localPos = pointer.localPosition;
            localPos.x = pointerX;
            localPos.y = pointerY;
            pointer.localPosition = localPos;
        }
    }
}
