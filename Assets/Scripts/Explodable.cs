using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Explodable : MonoBehaviour
{
	public GameObject Explosion;
	public AudioClip ExplodeClip;
    public EnemySpawner Spawner;

	public void Explode (Transform t)
	{
		Explode (t.position, t.rotation);
	}

	public void Explode (Vector3 pos)
	{
		Explode (pos, Quaternion.identity);
	}

	public void Explode (Vector3 pos, Quaternion rotation)
	{
		if (ExplodeClip) {
			Music.PlayClipAtPoint (ExplodeClip, pos, Music.instance.sfxv);
		}

		Instantiate (Explosion, pos, rotation);
        if (Spawner != null)
            Spawner.Despawn (gameObject);
        else
            Destroy(gameObject);
	}
}
