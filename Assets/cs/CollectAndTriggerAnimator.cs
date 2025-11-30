using UnityEngine;
using System.Collections;

public class SimpleCollectAndOpen : MonoBehaviour
{
    [Header("Player 设置")]
    public string playerTag = "Player";

    [Header("物品动画（Collect）")]
    public Animator itemAnimator;
    public string collectTrigger = "Collect";
    public float destroyDelay = 0.2f;

    [Header("本关关卡计数器（不拖也行，会自动找）")]
    public LevelCollectManager levelManager;

    private bool _collected = false;

    private void Start()
    {
        if (itemAnimator == null)
            itemAnimator = GetComponentInChildren<Animator>();

        // ✅ 自动找“自己所在关卡根节点”里的 Manager
        if (levelManager == null)
            levelManager = GetComponentInParent<LevelCollectManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_collected) return;
        if (!collision.CompareTag(playerTag)) return;

        _collected = true;

        // 1) 播放收集动画
        if (itemAnimator != null)
            itemAnimator.SetTrigger(collectTrigger);

        // 2) 通知本关 manager +1，并判断是不是最后一个
        bool isFinalCollect = false;
        if (levelManager != null)
            isFinalCollect = levelManager.RegisterCollect();

        // 3) 先隐藏自己（所有情况都隐藏+关碰撞）
        HideAndDisable();

        // 4) 不是最后一个 -> 正常 Destroy
        //    最后一个 -> 不 Destroy（避免协程依赖被中断）
        if (!isFinalCollect)
            Destroy(gameObject, destroyDelay);
    }

    private void HideAndDisable()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.enabled = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}
