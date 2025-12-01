using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SFXSliderController : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();

        _slider.onValueChanged.AddListener(OnSliderChanged);

        // 初始化一次
        AudioListener.volume = _slider.value;

        Debug.Log("SFXSliderController Awake in " + gameObject.name);
    }

    private void OnSliderChanged(float value)
    {
        Debug.Log("SFX Slider value = " + value);
        AudioListener.volume = Mathf.Clamp01(value);
    }
}
