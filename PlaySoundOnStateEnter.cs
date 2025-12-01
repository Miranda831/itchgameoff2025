using UnityEngine;

/// <summary>
/// 挂在 Animator 的某个 State 上：
/// 每次进入这个 State 播放一次音效
/// </summary>
public class PlaySoundOnStateEnter : StateMachineBehaviour
{
    [Header("音效设置")]
    public AudioClip sfx;
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource _audio;

    // 每次进入这个动画 State 的瞬间都会执行
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (sfx == null) return;

        // 找 AudioSource（挂在 Animator 所在的物体上）
        if (_audio == null)
        {
            _audio = animator.GetComponent<AudioSource>();
            if (_audio == null)
            {
                _audio = animator.gameObject.AddComponent<AudioSource>();
                _audio.playOnAwake = false;
                _audio.loop = false;
            }
        }

        _audio.PlayOneShot(sfx, volume);
    }
}
