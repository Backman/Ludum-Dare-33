using UnityEngine;
using System.Collections.Generic;

public class BlinkManager : MonoBehaviour
{
    struct BlinkState
    {
        public Color Color;
        public float EndTime;
        public float StartTime;
        public Renderer[] Renderers;
    }
    Dictionary<GameObject, BlinkState> _Blinks = new Dictionary<GameObject, BlinkState>();
    MaterialPropertyBlock _Block ;


    List<GameObject> toRemove = new List<GameObject>();
    public static BlinkManager Instance;
    void Awake()
    {
        Instance = this;
        _Block = new MaterialPropertyBlock();
    }

    void Update()
    {
        foreach (var blink in _Blinks)
        {
            var state = blink.Value;
            if (Time.unscaledTime > state.EndTime)
            {
                toRemove.Add(blink.Key);
            }
            float duration = state.EndTime - state.StartTime;


            float t = (state.EndTime - Time.unscaledTime) / duration;
            _Block.Clear();
            var color = state.Color;
            color.a *=  t;
            _Block.SetColor("_BlinkColor", color);
            for (int i = 0; i < state.Renderers.Length; i++)
            {
                var renderer = state.Renderers[i];
                renderer.SetPropertyBlock(_Block);
            }
        }
        _Block.Clear();
        for (int i = 0; i < toRemove.Count; i++)
        {
            var state = _Blinks[toRemove[i]];
            
            for (int j = 0; j < state.Renderers.Length; j++)
            {
                var renderer = state.Renderers[j];
                renderer.SetPropertyBlock(_Block);
            }

            _Blinks.Remove(toRemove[i]);
        }
        toRemove.Clear();
        
    }

    public void AddBlink(GameObject source, Color color, float duration)
    {
        BlinkState state;
        state.Color = color;
        state.StartTime = Time.unscaledTime;
        state.EndTime = Time.unscaledTime + duration;
        state.Renderers = source.GetComponentsInChildren<Renderer>();
        _Blinks[source] = state;
    }
}
