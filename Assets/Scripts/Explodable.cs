using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Explodable : MonoBehaviour
{
	public GameObject Explosion;
	public AudioClip ExplodeClip;

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
			AudioSource.PlayClipAtPoint (ExplodeClip, pos);
		}

		Instantiate (Explosion, pos, rotation);
		Destroy (gameObject);
	}
}
