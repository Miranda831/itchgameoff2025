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

    [Header("获取音效（可选）")]
    public AudioClip collectSfx;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private bool _collected = false;
    private AudioSource _audio;

    private void Awake()
    {
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.loop = false;
        _audio.spatialBlend = 0f;
        _audio.volume = sfxVolume;
    }

    private void Start()
    {
        if (itemAnimator == null)
            itemAnimator = GetComponentInChildren<Animator>();

        if (levelManager == null)
            levelManager = GetComponentInParent<LevelCollectManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_collected) return;
        if (!collision.CompareTag(playerTag)) return;

        _collected = true;

        // ✅ 0) 播音效 + 计算音效长度
        float clipLength = PlayCollectSoundAndGetLength();

        // 1) 播收集动画
        if (itemAnimator != null)
            itemAnimator.SetTrigger(collectTrigger);

        // 2) 通知 Manager
        bool isFinalCollect = false;
        if (levelManager != null)
            isFinalCollect = levelManager.RegisterCollect();

        // 3) 立刻隐藏 & 禁用碰撞
        HideAndDisable();

        // ✅ 4) 延迟 Destroy：等声音播完
        if (!isFinalCollect)
        {
            float delay = Mathf.Max(destroyDelay, clipLength);
            StartCoroutine(DestroyAfterDelay(delay));
        }
    }

    // ✅ 播放并返回音效长度
    private float PlayCollectSoundAndGetLength()
    {
        if (collectSfx != null && _audio != null)
        {
            _audio.PlayOneShot(collectSfx, sfxVolume);
            return collectSfx.length;
        }

        return 0f;
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void HideAndDisable()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.enabled = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}
