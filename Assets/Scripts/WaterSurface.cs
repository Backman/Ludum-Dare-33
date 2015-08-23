using UnityEngine;
using System.Collections.Generic;

public class WaterSurface : MonoBehaviour
{
    public float Height;
	public AudioClip SplashSound;

    public GameObject SplashParticle;
    List<GameObject> _toRemove = new List<GameObject>();
    void LateUpdate()
    {
        Vector3 currentPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        currentPos.x = cameraPos.x;
        currentPos.y = 0;
        transform.position = currentPos;
        float frustumHeight = 2.0f * Mathf.Abs(transform.position.z - Camera.main.transform.position.z) * Mathf.Tan((float)(Camera.main.fieldOfView * 0.5 * Mathf.Deg2Rad));
        float frustumWidth = frustumHeight * Camera.main.aspect;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(frustumWidth * 4, Height);

        foreach (var item in _ActiveSplashes)
        {
            if(!item.Value)
                _toRemove.Add(item.Key);
            
        }
        foreach (var item in _toRemove)
        {
            _ActiveSplashes.Remove(item);
        }
        _toRemove.Clear();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var blokFosk = collision.gameObject.GetComponentInParent<Blokfosk>();
        var go = collision.gameObject;
        if(blokFosk != null)
        {
            if(collision.isTrigger)
                return;
            go = blokFosk.gameObject;
        }
        go.SendMessage("WaterSurfaceEnter", this, SendMessageOptions.DontRequireReceiver);
    }

    Dictionary<GameObject, GameObject> _ActiveSplashes = new Dictionary<GameObject,GameObject>();

    public void DoSplash(GameObject source, Vector3 position)
    {
        if(_ActiveSplashes.ContainsKey(source))
            return;
        position.y = 0f;
        position.z = transform.position.z;
        _ActiveSplashes[source] = Instantiate(SplashParticle, position, Quaternion.identity) as GameObject;
		Music.PlayClipAtPoint (SplashSound, position, Music.instance.sfxv);
    }
}
