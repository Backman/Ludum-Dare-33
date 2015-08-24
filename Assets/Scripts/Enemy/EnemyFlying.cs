using UnityEngine;
using System.Collections;

public class EnemyFlying : Enemy
{
	public override EnemyType Type { get { return EnemyType.Flying; } }

	protected override void Update ()
	{
		base.Update ();
		if (IsHit && transform.position.y < -0.1f) {
			Explode (true);
		}
	}

	protected override void CheckOtherCollisions (Collider2D collider)
	{
		var boat = collider.GetComponent<EnemyBoat> ();
		if (boat && boat.IsHit) {
			GameLogic.Instance.OnRekFace.Invoke (gameObject);
			var dir = boat.transform.position - transform.position;
			Hit (dir);
			Explode(true);
		}

		var flying = collider.GetComponent<EnemyFlying> ();
		if (flying && flying.IsHit) {
			GameLogic.Instance.OnRekFace.Invoke (gameObject);
			var dir = flying.transform.position - transform.position;
			Hit (dir);
			Explode(true);
		}
	}

	void WaterSurfaceEnter (object obj)
	{
		WaterSurface surface = obj as WaterSurface;
		if (IsHit) {
			Explode (true);
			surface.DoSplash (gameObject, transform.position);
		}
	}

	protected override void TentacleHit (Vector2 dir)
	{
		if (!IsHit)
		{
			Explode(false);
		}

		Hit(dir);
	}
}
