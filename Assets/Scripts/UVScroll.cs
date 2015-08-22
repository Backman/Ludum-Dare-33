using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class UVScroll : MonoBehaviour
{
    Renderer _Renderer;
    public float OffsetScale;
    void Awake()
    {
        _Renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 pos = Camera.main.transform.position;
        _Renderer.material.SetTextureOffset("_MainTex", new Vector2(OffsetScale * pos.x, OffsetScale * pos.y  ));
    }
}
