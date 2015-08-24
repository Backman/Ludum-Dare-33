using UnityEngine;
using System.Collections;

public class ComboTextFade : MonoBehaviour
{
    public float Duration;
    public AnimationCurve AlphaCurve;
    public AnimationCurve XCurve;
    public AnimationCurve YCurve;
    public int EmitCount;

    SpriteRenderer[] _Sprites;
    ParticleSystem[] _Particles;

    void Awake()
    {
        _Sprites = GetComponentsInChildren<SpriteRenderer>();
        _Particles = GetComponentsInChildren<ParticleSystem>();
    }

    float _EnableTime;
    Vector3 _Pos;
    void OnEnable()
    {
        for (int i = 0; i < _Particles.Length;i++)
        {
            _Particles[i].Emit(EmitCount);
        }
        _EnableTime = Time.unscaledTime;
        _Pos = transform.position;
    }
    void Update()
    {
        float t = (Time.unscaledTime - _EnableTime) / Duration;
        for (int i = 0; i < _Sprites.Length; i++)
        {
            var col = _Sprites[i].color;
            col.a = AlphaCurve.Evaluate(t);
            _Sprites[i].color = col;
        }
        Vector3 pos = _Pos;
        pos.x += XCurve.Evaluate(t);
        pos.y += YCurve.Evaluate(t);
        transform.position = pos;
        if (_EnableTime + Duration < Time.unscaledTime)
        {
            TrashMan.despawn(this.gameObject);
        }
    }
}
