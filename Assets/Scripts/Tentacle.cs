using UnityEngine;
using System.Collections;

public class Tentacle : MonoBehaviour
{
	public SimpleCCD CCD;
	[HideInInspector]
	public Transform Target;

	private void Awake ()
	{
		if (!CCD) {
			CCD = GetComponent<SimpleCCD> ();
		}

		Target = CCD.target;
	}

	public void MoveTarget (Vector2 move)
	{
		Target.position += new Vector3 (move.x, move.y);
	}

	public void SetTarget (Vector3 newPos)
	{
		Target.position = newPos;
	}
}
