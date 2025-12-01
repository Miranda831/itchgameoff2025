using UnityEngine;
using System.Collections;

public class LevelCollectManager : MonoBehaviour
{
    [Header("每关需要收集多少个才打开")]
    public int targetCount = 3;

    [Header("目标物体（传送门 Open）")]
    public Animator targetAnimator;
    public string openTrigger = "Open";

    [Header("延迟后激活的传送特效")]
    public GameObject objectToEnable;   // 传送魔法 / 特效
    public float delaySpawn = 1f;       // 门打开后延迟多久出现特效
    public bool spawnOnlyOnce = true;   // 只激活一次

    [Header("传送阵激活音效（可选）")]
    public AudioClip openSfx;            // ✅ 门开启声音
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private int _count = 0;
    private bool _spawned = false;
    private AudioSource _audio;          // ✅ 播放器

    private void Awake()
    {
        // ✅ 自动创建 AudioSource
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.loop = false;
        _audio.spatialBlend = 0f; // 2D 声
        _audio.volume = sfxVolume;
    }

    /// <summary>
    /// Collect 物体调用它来 +1
    /// </summary>
    public bool RegisterCollect()
    {
        _count++;

        // 达标且这一关还没触发过（或允许多次触发）
        bool isFinalCollect = (_count >= targetCount) && (!spawnOnlyOnce || !_spawned);

        if (isFinalCollect)
        {
            // ✅ 0) 播放传送阵激活音效
            PlayOpenSound();

            // 1) 门立刻开动画
            if (targetAnimator != null)
                targetAnimator.SetTrigger(openTrigger);

            _spawned = true;

            // 2) 延迟出现特效
            StartCoroutine(DelayedEnable());
        }

        return isFinalCollect;
    }

    private void PlayOpenSound()
    {
        if (openSfx != null && _audio != null)
        {
            _audio.PlayOneShot(openSfx, sfxVolume);
        }
    }

    private IEnumerator DelayedEnable()
    {
        yield return new WaitForSeconds(delaySpawn);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);
    }

    // （可选）如果你想调试看现在收了几个
    public int GetCurrentCount() => _count;
}
