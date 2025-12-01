using UnityEngine;
using MoreMountains.CorgiEngine;

public class IgnoreWaterAfterRespawn : MonoBehaviour, Respawnable
{
    [Tooltip("复活后多少秒内不触发水花")]
    public float ignoreDuration = 0.35f;

    private float _ignoreUntil = -999f;

    private void Start()
    {
        // 自动把自己注册到所有 CheckPoint
        CheckPoint[] cps = FindObjectsOfType<CheckPoint>();
        foreach (var cp in cps)
        {
            cp.AssignObjectToCheckPoint(this);
        }
    }

    // ✅ 这是 Corgi Respawnable 必须实现的方法
    public void OnPlayerRespawn(CheckPoint checkpoint, Character player)
    {
        _ignoreUntil = Time.time + ignoreDuration;
    }

    // Water 脚本来问：现在能不能喷水花
    public bool CanSplashNow()
    {
        return Time.time >= _ignoreUntil;
    }
}
