using UnityEngine;
using System.Collections;

public class EnemyFlying : Enemy
{
	protected override void Update ()
	{
		base.Update ();
		if (IsHit && transform.position.y < -0.1f) {
			Explode ();
		}
	}

	protected override void OnTriggerEnter2D (Collider2D collider)
	{
		base.OnTriggerEnter2D (collider);

		var boat = collider.GetComponent<EnemyBoat> ();
		if (boat && boat.IsHit) {
			GameLogic.Instance.OnRekFace.Invoke (gameObject);
			var dir = boat.transform.position - transform.position;
			Hit (dir);
		}

		var flying = collider.GetComponent<EnemyFlying> ();
		if (flying && flying.IsHit) {
			GameLogic.Instance.OnRekFace.Invoke (gameObject);
			var dir = flying.transform.position - transform.position;
			Hit (dir);
		}
	}

	void WaterSurfaceEnter (object obj)
	{
		WaterSurface surface = obj as WaterSurface;
		if (IsHit) {
			Explode ();
			surface.DoSplash (gameObject, transform.position);
		}
	}

	protected override void TentacleHit (Vector2 dir)
	{
		Hit (dir);
	}

	public override void FireAlternative ()
	{

	}
}
