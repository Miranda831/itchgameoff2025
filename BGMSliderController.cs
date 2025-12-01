using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BGMSliderController : MonoBehaviour
{
    [Header("拖入：你的 BGM AudioSource")]
    public AudioSource bgmSource;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();

        // 绑定监听
        _slider.onValueChanged.AddListener(OnSliderChanged);

        // 初始化一次
        if (bgmSource != null)
            bgmSource.volume = _slider.value;

        Debug.Log("BGMSliderController Awake in " + gameObject.name);
    }

    private void OnSliderChanged(float value)
    {
        Debug.Log("BGM Slider value = " + value);

        if (bgmSource != null)
        {
            bgmSource.volume = Mathf.Clamp01(value);
        }
    }
}
