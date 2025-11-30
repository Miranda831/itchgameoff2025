using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class SwitchArea : CorgiMonoBehaviour, Respawnable
    {
        [Header("地图区块")]
        public GameObject[] map1;     // Map1
        public GameObject[] map2;     // Map2

        protected virtual void Start()
        {
            // 自动注册复活监听
            CheckPoint[] cps = FindObjectsOfType<CheckPoint>();
            foreach (var cp in cps)
                cp.AssignObjectToCheckPoint(this);

            // 初始状态回到 Map1
            ResetToMap1();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            // 玩家碰到本 Trigger → 切到 Map2
            SwitchToMap2();
        }

        // 玩家复活 → 回到 Map1
        public void OnPlayerRespawn(CheckPoint cp, Character player)
        {
            ResetToMap1();
        }

        // 切换到 Map2
        private void SwitchToMap2()
        {
            Debug.Log("[SwitchArea] → 切到 Map2");

            SetActive(map1, false);
            SetActive(map2, true);
        }

        // 切回 Map1（初始状态）
        private void ResetToMap1()
        {
            Debug.Log("[SwitchArea] → 切回 Map1");

            SetActive(map1, true);
            SetActive(map2, false);
        }

        // 工具函数：批量开关
        private void SetActive(GameObject[] list, bool status)
        {
            if (list == null) return;

            foreach (var go in list)
                if (go != null)
                    go.SetActive(status);
        }
    }
}
