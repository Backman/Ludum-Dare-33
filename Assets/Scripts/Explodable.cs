using UnityEngine;
using System.Collections;

public class Explodable : MonoBehaviour
{
	public GameObject Explosion;

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
		Instantiate (Explosion, pos, rotation);
		Destroy (gameObject);
	}
}
