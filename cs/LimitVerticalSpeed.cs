using UnityEngine;
using MoreMountains.CorgiEngine;

public class LimitVerticalSpeed : MonoBehaviour
{
    public float maxUpSpeed = 15f;

    private CorgiController _controller;

    [HideInInspector]
    public bool disableLimit = false;   // Jumper 会把这个设为 true

    void Awake()
    {
        _controller = GetComponent<CorgiController>();
    }

    void LateUpdate()
    {
        if (_controller == null) return;

        // 如果 Jumper 让它暂时不限制，上不限制
        if (disableLimit) return;

        Vector2 speed = _controller.Speed;

        if (speed.y > maxUpSpeed)
        {
            speed.y = maxUpSpeed;
            _controller.SetVerticalForce(speed.y);
        }
    }
}
