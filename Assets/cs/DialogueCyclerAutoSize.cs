using System.Collections;
using UnityEngine;
using TMPro;

public class WorldBubbleDialogueSafe : MonoBehaviour
{
    [Header("引用")]
    public TMP_Text dialogueText;
    public SpriteRenderer bubbleRenderer;

    [Header("对话内容")]
    [TextArea(2, 4)]
    public string[] lines;

    [Header("播放设置")]
    public float interval = 2f;
    public int playCount = 5;
    public bool loopIfShort = true;

    [Header("永远循环")]
    public bool loopForever = false;   // ← 这个是 √

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

        int index = 0;

        // ================================
        // ⭐ 如果勾选了“永远循环”
        // ================================
        if (loopForever)
        {
            while (true)
            {
                dialogueText.text = lines[index];

                yield return null; // 等一帧刷新
                ResizeBubble();
                yield return new WaitForSeconds(interval);

                index = (index + 1) % lines.Length;
            }
        }

        // ================================
        // ⭐ 不勾选永远循环 → 播放 playCount 次
        // ================================
        int shown = 0;

        while (shown < playCount)
        {
            dialogueText.text = lines[index];

            yield return null;
            ResizeBubble();
            yield return new WaitForSeconds(interval);

            shown++;
            index = (index + 1) % lines.Length;
        }
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
    }
}
