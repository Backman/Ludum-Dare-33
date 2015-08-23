using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public Projectile ProjectilePrefab;

	public Vector2 AttackIntervall;

	public Blokfosk Blokfosk { get; set; }

	protected float _attackTimer;
	protected float _counter;

	protected virtual void Awake ()
	{
		ResetAttackTimer ();
	}

	protected virtual void Update ()
	{
		_counter += Time.deltaTime;
		if (_counter >= _attackTimer) {
			ResetAttackTimer ();
		}
	}

	private void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "Blokfosk_Tentacle") {
			GetComponent<Explodable> ().Explode (transform.position);
		}
	}

	public void FireProjectile ()
	{
		var projectile = (Projectile)Instantiate (ProjectilePrefab, transform.position, transform.rotation);

		var direction = Blokfosk == null ? transform.right : Blokfosk.transform.position - transform.position;

		projectile.Direction = direction.normalized;
	}

	public virtual void FireAlternative ()
	{

	}

	private void ResetAttackTimer ()
	{
		_counter += 0f;
		RandomizeAttackTimer ();
	}

	private void RandomizeAttackTimer ()
	{
		_attackTimer = Random.Range (AttackIntervall.x, AttackIntervall.y);
	}
}
