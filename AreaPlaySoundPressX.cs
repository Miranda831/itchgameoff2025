using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaPlaySoundPressROnce : MonoBehaviour
{
    [Header("音效")]
    public AudioClip sfx;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("允许触发的 Layer（勾 Player）")]
    public LayerMask targetLayers;

    [Header("触发键")]
    public KeyCode triggerKey = KeyCode.R;

    private AudioSource _audio;
    private bool _playerInside;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.loop = false;
        _audio.spatialBlend = 0f;
        _audio.clip = sfx;
        _audio.volume = volume;
    }

    void Update()
    {
        // 必须在区域内
        if (!_playerInside)
            return;

        // 每次按 R 播放一次
        if (Input.GetKeyDown(triggerKey))
        {
            if (sfx != null)
            {
                _audio.PlayOneShot(sfx, volume);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
            return;

        _playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
            return;

        _playerInside = false;
    }
}
