using UnityEngine;
using System.Collections;

public class EnemyBoat : Enemy
{
	public override EnemyType Type { get { return EnemyType.Boat; } }

	public GameObject MinePrefab;

	protected override void Update ()
	{
		base.Update ();
		if (IsHit && transform.position.y < -0.1f) {
			Explode (true);
		}
	}

	protected override void CheckOtherCollisions(Collider2D collider)
	{
		var flying = collider.GetComponent<EnemyFlying>();
		if (flying && flying.IsHit)
		{
			GameLogic.Instance.OnRekFace.Invoke(gameObject);
			var dir = flying.transform.position - transform.position;
			Hit(dir);
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

	protected override void TentacleHit(Vector2 dir)
	{
		if (!IsHit)
		{
			Explode(false);
		}

		Hit(dir);
	}

	protected override void FireProjectile()
	{
		var blokPos = Blokfosk.transform.position;
		var delta = blokPos - transform.position;
		if (delta.y < 0f)
		{
			// Drop mine
			var go = TrashMan.spawn(MinePrefab, transform.position + Vector3.down * 0.1f);
			go.GetComponent<Projectile>().Direction = Vector2.down;
		}
		else
		{
			BasicFire();
		}
	}
}

