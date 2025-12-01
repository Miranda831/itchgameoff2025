using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaPlaySoundSimple : MonoBehaviour
{
    [Header("音效设置")]
    public AudioClip sfx;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("目标 Layer（要勾 Player 那个）")]
    public LayerMask targetLayers;

    [Header("播放设置")]
    public bool loop = false;      // ✅ 是否循环（在区域里一直播）
    public bool playOnce = true;   // ✅ 每次进入区域时播一次（不叠加）

    [Header("Loop 时离开是否停止")]
    public bool stopOnExit = true; // ✅ 只对 loop 模式生效

    private AudioSource _audio;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.loop = loop;
        _audio.spatialBlend = 0f;
        _audio.clip = sfx;
        _audio.volume = volume;
    }

    private bool IsTarget(Collider2D other)
    {
        return ((1 << other.gameObject.layer) & targetLayers) != 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsTarget(other))
            return;

        if (sfx == null)
            return;

        // 🔁 循环模式：进来就开始循环播
        if (loop)
        {
            if (!_audio.isPlaying)
                _audio.Play();
            return;
        }

        // 🎵 非循环模式：每次 OnTriggerEnter 播一次
        if (playOnce)
        {
            // 这里的 playOnce 含义是：这次触发只播一遍（OnTriggerEnter 本来就只调一次）
            _audio.PlayOneShot(sfx, volume);
        }
        else
        {
            // 如果你以后想改成“一个区域内可以多次触发+冷却时间”，可以在这里加计时逻辑
            _audio.PlayOneShot(sfx, volume);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsTarget(other))
            return;

        if (loop && stopOnExit)
        {
            _audio.Stop();
        }
    }
}
