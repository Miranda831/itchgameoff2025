using UnityEngine;

/// <summary>
/// 花瓣随水波轻微上下、左右晃动 + 轻微旋转
/// 挂在花瓣 Sprite / Prefab 上
/// </summary>
public class PetalBobOnWater : MonoBehaviour
{
    [Header("位移振幅（世界坐标）")]
    public float amplitudeY = 0.15f;   // 上下晃动幅度
    public float amplitudeX = 0.08f;   // 左右晃动幅度

    [Header("晃动速度（频率）")]
    public float frequencyY = 1.2f;    // 上下晃动速度
    public float frequencyX = 0.6f;    // 左右晃动速度

    [Header("旋转效果")]
    public float rotateAmplitude = 8f; // Z轴来回旋转角度（度）
    public float rotateSpeed = 1.0f;   // 旋转快慢

    [Header("随机相位偏移（多个花瓣不一致）")]
    public bool randomPhase = true;

    private Vector3 _startPos;
    private float _phaseOffset;

    void Start()
    {
        // 记录初始位置
        _startPos = transform.position;

        // 给每个花瓣一个随机起始时间，让运动不要完全同步
        _phaseOffset = randomPhase ? Random.Range(0f, 10f) : 0f;
    }

    void Update()
    {
        float t = Time.time + _phaseOffset;

        // 上下位移：正弦波
        float offsetY = Mathf.Sin(t * frequencyY) * amplitudeY;

        // 左右位移：可以频率不一样，看起来更自然
        float offsetX = Mathf.Sin(t * frequencyX) * amplitudeX;

        // 应用位移
        transform.position = _startPos + new Vector3(offsetX, offsetY, 0f);

        // 轻微左右摇摆（Z 轴旋转）
        float rotZ = Mathf.Sin(t * rotateSpeed) * rotateAmplitude;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }
}

