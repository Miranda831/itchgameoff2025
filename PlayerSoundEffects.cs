using UnityEngine;
using MoreMountains.CorgiEngine;

public class PlayerSoundEffects : MonoBehaviour
{
    [Header("Panel 屏蔽设置")]
    [Tooltip("带这个 Tag 的 Panel 只要激活，就会禁止玩家所有 SFX")]
    public string blockPanelTag = "BlockPlayerSound";

    [Header("开场静音设置")]
    [Tooltip("是否在场景开始时静音一段时间，避免刚出生落地的声音")]
    public bool muteOnSceneStart = true;
    [Tooltip("开场静音时长（秒）")]
    public float initialMuteDuration = 0.5f;

    [Header("引用")]
    public CorgiController controller;   // 不填会自动 GetComponent

    [Header("Sound Effect 总音量（以后菜单可控）")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("按键音效")]
    public AudioClip jumpClip;           // Space
    public AudioClip crouchClip;         // S

    [Header("默认走路循环声（找不到地形时用这个；不想默认就留空）")]
    public AudioClip defaultWalkLoopClip;

    [System.Serializable]
    public class SurfaceFootstep
    {
        [Tooltip("地面使用的 Tag，例如 Grass / Stone / Wood")]
        public string groundTag;
        [Tooltip("对应地面的循环脚步声 Clip")]
        public AudioClip footstepLoop;
    }

    [Header("不同地面脚步循环声（Tag -> Clip 映射）")]
    public SurfaceFootstep[] surfaces;

    [Header("移动相关")]
    [Tooltip("认为在走路的最小水平速度")]
    public float speedThreshold = 0.1f;

    [Tooltip("哪些 Layer 视为“地面 / 平台”，只对这些 Layer 的碰撞切换脚步声")]
    public LayerMask groundLayer = ~0;

    [Tooltip("离开地面后，仍然当作在地上的缓冲时间（秒）")]
    public float groundedBufferTime = 0.15f;

    // 内部 AudioSource
    private AudioSource _walkSource;     // 循环脚步
    private AudioSource _oneShotSource;  // jump / crouch 等一次性

    private float _muteEndTime;
    private float _lastGroundedTime;

    private bool _footstepOn = false;    // “脚步开关”当前状态

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<CorgiController>();

        // 走路循环 AudioSource
        _walkSource = gameObject.AddComponent<AudioSource>();
        _walkSource.playOnAwake = false;
        _walkSource.loop = true;
        _walkSource.spatialBlend = 0f;

        // 一次性音效 AudioSource
        _oneShotSource = gameObject.AddComponent<AudioSource>();
        _oneShotSource.playOnAwake = false;
        _oneShotSource.loop = false;
        _oneShotSource.spatialBlend = 0f;

        _muteEndTime = muteOnSceneStart ? Time.time + initialMuteDuration : 0f;
    }

    void Update()
    {
        // ① Panel 静音
        if (IsAnyBlockPanelActive())
        {
            SetFootstepOn(false);
            return;
        }

        // ② 开场静音
        if (muteOnSceneStart && Time.time < _muteEndTime)
        {
            SetFootstepOn(false);
            return;
        }

        if (controller == null) return;

        _walkSource.volume = sfxVolume;
        _oneShotSource.volume = sfxVolume;

        // 原始 IsGrounded
        bool rawGrounded = controller.State.IsGrounded;
        if (rawGrounded)
            _lastGroundedTime = Time.time;

        // 带缓冲的 grounded
        bool groundedBuffered = (Time.time - _lastGroundedTime) <= groundedBufferTime;

        float speedX = Mathf.Abs(controller.Speed.x);

        float inputX = Input.GetAxisRaw("Horizontal");
        bool hasMoveInput = Mathf.Abs(inputX) > 0.01f;

        // ---------- Jump ----------
        if (Input.GetKeyDown(KeyCode.Space) && jumpClip != null && sfxVolume > 0f)
            _oneShotSource.PlayOneShot(jumpClip, sfxVolume);

        // ---------- Crouch ----------
        if (Input.GetKeyDown(KeyCode.S) && crouchClip != null && sfxVolume > 0f)
            _oneShotSource.PlayOneShot(crouchClip, sfxVolume);

        // ---------- 计算脚步“开关状态” ----------
        bool shouldFootstepOn =
            groundedBuffered &&
            hasMoveInput &&
            speedX > speedThreshold &&
            _walkSource.clip != null &&
            sfxVolume > 0f;

        SetFootstepOn(shouldFootstepOn);
    }

    /// <summary>
    /// 真正的“脚步开关”逻辑，只在状态变化时才 Play/Stop
    /// </summary>
    void SetFootstepOn(bool on)
    {
        if (_footstepOn == on)
            return; // 状态没变就什么都不做

        _footstepOn = on;

        if (_footstepOn)
        {
            if (_walkSource.clip != null)
                _walkSource.Play();
        }
        else
        {
            if (_walkSource.isPlaying)
                _walkSource.Stop();
        }
    }

    //====== 通过“碰到地面”来切换当前脚步 Clip（只在 Enter 事件触发一次）======//

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleGroundCollision(collision.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleGroundCollision(other);
    }

    // 不再用 Stay，只在第一次接触时切换一次
    // void OnCollisionStay2D / OnTriggerStay2D 已删掉

    void HandleGroundCollision(Collider2D col)
    {
        if (col == null) return;

        // 只对 groundLayer 生效
        if (!IsInLayerMask(col.gameObject.layer, groundLayer))
            return;

        string tag = col.tag;

        AudioClip targetClip = defaultWalkLoopClip;

        if (!string.IsNullOrEmpty(tag))
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                if (surfaces[i] == null || surfaces[i].footstepLoop == null)
                    continue;

                if (surfaces[i].groundTag == tag)
                {
                    targetClip = surfaces[i].footstepLoop;
                    break;
                }
            }
        }

        // 只负责“换 clip”，不负责 Play/Stop
        if (_walkSource.clip != targetClip)
        {
            bool wasOn = _footstepOn; // 记住当前开关状态
            if (_walkSource.isPlaying)
                _walkSource.Stop();

            _walkSource.clip = targetClip;

            // 如果本来就是“开”的，就在换完 clip 后重新 Play
            if (wasOn && _walkSource.clip != null)
                _walkSource.Play();
        }

        // Debug.Log($"[PlayerSound] Hit ground tag = {tag}, use clip = {(targetClip ? targetClip.name : "null")}");
    }

    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    bool IsAnyBlockPanelActive()
    {
        if (string.IsNullOrEmpty(blockPanelTag))
            return false;

        GameObject[] panels;
        try
        {
            panels = GameObject.FindGameObjectsWithTag(blockPanelTag);
        }
        catch
        {
            return false;
        }

        if (panels == null || panels.Length == 0)
            return false;

        foreach (var p in panels)
        {
            if (p != null && p.activeInHierarchy)
                return true;
        }

        return false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.1f);
    }
#endif
}
