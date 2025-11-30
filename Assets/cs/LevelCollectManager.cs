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

    private int _count = 0;
    private bool _spawned = false;

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
            // 1) 门立刻开动画
            if (targetAnimator != null)
                targetAnimator.SetTrigger(openTrigger);

            _spawned = true;

            // 2) 延迟出现特效
            StartCoroutine(DelayedEnable());
        }

        return isFinalCollect;
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
