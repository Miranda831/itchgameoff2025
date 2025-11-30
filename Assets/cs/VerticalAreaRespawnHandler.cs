using UnityEngine;
using MoreMountains.CorgiEngine;

public class VerticalAreaRespawnHandler : MonoBehaviour, Respawnable
{
    [Header("引用切换脚本")]
    public VerticalAreaSwitcherCorgi switcher;

    private void Start()
    {
        // 注册复活监听
        CheckPoint[] cps = FindObjectsOfType<CheckPoint>();
        foreach (var cp in cps)
        {
            cp.AssignObjectToCheckPoint(this);
        }
    }

    public void OnPlayerRespawn(CheckPoint checkpoint, Character player)
    {
        Debug.Log("[RespawnHandler] 玩家复活 → 强制回上地图");

        if (switcher != null)
            switcher.ForceTop();
        else
            Debug.LogWarning("RespawnHandler 没有引用 switcher！");
    }
}
