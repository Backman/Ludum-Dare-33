using UnityEngine;
using System.Collections;

public class EnemyBoat : Enemy
{
	protected override void Update ()
	{
		base.Update ();
		if (IsHit && _rb.position.y <= 0.1f) {
			Explode ();
		}
	}

	protected override void TentacleHit (Vector2 dir)
	{
		_rb.AddForce (dir.normalized * 5f, ForceMode2D.Impulse);
		_rb.AddTorque (360f, ForceMode2D.Impulse);
		_rb.gravityScale = 1f;
	}

	public override void FireAlternative ()
	{

	}
}
