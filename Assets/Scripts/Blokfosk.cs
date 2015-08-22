using UnityEngine;
using System.Collections;

public class Blokfosk : MonoBehaviour
{
	public Tentacle LeftTentacle;
	public Tentacle RightTentacle;

	public BoxCollider2D LeftTentacleBounds;
	public BoxCollider2D RightTentacleBounds;
	
	private Transform _leftTentacleTarget;
	private Transform _rightTentacleTarget;

	public void Awake ()
	{
		_leftTentacleTarget = LeftTentacle.GetComponent<SimpleCCD> ().target;
		_rightTentacleTarget = RightTentacle.GetComponent<SimpleCCD> ().target;
	}

	public void Update ()
	{

	}
}
