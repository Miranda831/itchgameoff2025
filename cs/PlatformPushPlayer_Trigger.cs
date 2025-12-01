using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatformAttachSimple : MonoBehaviour
{
    [Header("哪些 Tag 的物体会跟着平台一起移动（例如 Player / Pushable）")]
    public List<string> attachTags = new List<string>();

    [Header("离平台超过这个竖直距离就自动解绑")]
    public float maxDetachDistanceY = 1.0f;

    private Rigidbody2D _platformRb;
    private Vector2 _lastPosition;

    // 当前在平台上的刚体列表（玩家、箱子都在这里）
    private readonly List<Rigidbody2D> _attachedBodies = new List<Rigidbody2D>();

    private void Awake()
    {
        _platformRb = GetComponent<Rigidbody2D>();
        _lastPosition = _platformRb.position;

        // 只负责检测的触发器
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = _platformRb.position;
        Vector2 delta = currentPosition - _lastPosition;
        _lastPosition = currentPosition;

        // 先清理“已经离平台太远”的刚体（比如被推下去了）
        for (int i = _attachedBodies.Count - 1; i >= 0; i--)
        {
            var body = _attachedBodies[i];
            if (body == null)
            {
                _attachedBodies.RemoveAt(i);
                continue;
            }

            // 超过设定的竖直距离，强制解绑
            if (Mathf.Abs(body.position.y - currentPosition.y) > maxDetachDistanceY)
            {
                _attachedBodies.RemoveAt(i);
            }
        }

        if (delta == Vector2.zero || _attachedBodies.Count == 0)
            return;

        // 把平台本帧的位移加到所有“仍在平台上的”刚体上
        for (int i = 0; i < _attachedBodies.Count; i++)
        {
            var body = _attachedBodies[i];
            if (body == null) continue;

            body.position += delta;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!attachTags.Contains(col.tag))
            return;

        Rigidbody2D body = col.attachedRigidbody ?? col.GetComponentInParent<Rigidbody2D>();
        if (body != null && !_attachedBodies.Contains(body))
        {
            _attachedBodies.Add(body);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!attachTags.Contains(col.tag))
            return;

        Rigidbody2D body = col.attachedRigidbody ?? col.GetComponentInParent<Rigidbody2D>();
        if (body != null)
        {
            _attachedBodies.Remove(body);
        }
    }
}
