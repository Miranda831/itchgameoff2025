using UnityEngine;
using MoreMountains.CorgiEngine;

public class ToggleTriggerPair : CorgiMonoBehaviour, Respawnable
{
    [Header("自己的触发器")]
    public Collider2D selfTrigger;

    [Header("对方的触发器（A ↔ B）")]
    public Collider2D otherTrigger;

    [Header("（可选）触发时要打开的 C")]
    public GameObject objectC;   // 只在 A 上拖，B 留空

    [Header("触发者 Tag")]
    public string triggerTag = "Player";

    [Header("开 的延迟秒数")]
    public float delay = 2f;

    [Header("初始状态：这个 Trigger 是否是【开启】的？")]
    public bool initialOn = true;    // A 勾 true，B 勾 false

    private void Reset()
    {
        selfTrigger = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        // 注册复活回调
        CheckPoint[] cps = FindObjectsOfType<CheckPoint>();
        foreach (var cp in cps)
            cp.AssignObjectToCheckPoint(this);

        // 游戏初始状态
        ApplyInitialState();
    }

    private void ApplyInitialState()
    {
        // 自己的开关状态看 initialOn
        if (selfTrigger != null)
            selfTrigger.enabled = initialOn;

        // C 在初始 & 复活时必须关
        if (objectC != null)
            objectC.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(triggerTag)) return;

        Debug.Log("[ToggleTriggerPair] Triggered: " + name + " by " + collision.name);

        // 关自己：关是没有延迟的
        if (selfTrigger != null)
            selfTrigger.enabled = false;

        // 开别人的事，走协程，有延迟
        StartCoroutine(SwitchAfterDelay());
    }

    private System.Collections.IEnumerator SwitchAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // 延迟后 → 打开对方 Trigger
        if (otherTrigger != null)
            otherTrigger.enabled = true;

        // 如果这个 Trigger 绑定了 C，就把 C 打开
        //（等于：“谁负责开 C，就把 C 拖到谁身上”）
        if (objectC != null)
            objectC.SetActive(true);
    }

    // -------- Corgi 复活回调 --------
    public void OnPlayerRespawn(CheckPoint cp, Character player)
    {
        ApplyInitialState();
    }
}
