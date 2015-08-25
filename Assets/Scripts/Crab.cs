using UnityEngine;
using System.Collections;

public class Crab : MonoBehaviour
{
	private Transform _blokfosk;

	private void Start ()
	{
		var blokfosk = Blokfosk.Instance;
		_blokfosk = blokfosk.transform;
		blokfosk.Crab = transform;
	}

	private void Update ()
	{
		var pos = transform.position;
		pos.x = _blokfosk.position.x;
	}
}
