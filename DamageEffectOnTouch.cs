using UnityEngine;
using MoreMountains.CorgiEngine;

public class DamageEffectOnTouch : MonoBehaviour
{
    [Header("声音效果（可选）")]
    public AudioClip hitSfx;              // 播放的受伤声音
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("粒子 / 特效（可选）")]
    public GameObject hitVfxPrefab;       // 受击时生成的特效
    public float vfxLifeTime = 2f;        // 特效自动销毁时间

    [Header("目标层级（一般自动从 DamageOnTouch 里读取）")]
    public LayerMask targetLayerMask;     // 想要触发的对象层，例如 Player

    private AudioSource _audio;
    private DamageOnTouch _damageOnTouch;

    void Awake()
    {
        // 找同物体上的 DamageOnTouch，自动同步它的 TargetLayerMask
        _damageOnTouch = GetComponent<DamageOnTouch>();
        if (_damageOnTouch != null)
        {
            targetLayerMask = _damageOnTouch.TargetLayerMask;
        }

        // 自动加一个 AudioSource 用来放 hitSfx
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.loop = false;
        _audio.spatialBlend = 0f;   // 2D 声音
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只对目标层生效（一般是 Player）
        if (((1 << other.gameObject.layer) & targetLayerMask) == 0)
            return;

        PlayEffects(other.bounds.center);
    }

    void PlayEffects(Vector3 hitPosition)
    {
        // 播声音
        if (hitSfx != null && sfxVolume > 0f)
        {
            _audio.PlayOneShot(hitSfx, sfxVolume);
        }

        // 生成特效
        if (hitVfxPrefab != null)
        {
            GameObject vfx = Instantiate(hitVfxPrefab, hitPosition, Quaternion.identity);
            if (vfxLifeTime > 0f)
            {
                Destroy(vfx, vfxLifeTime);
            }
        }
    }
}
