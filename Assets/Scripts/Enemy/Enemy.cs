using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
	public GameObject ProjectilePrefab;
	public float MovementSpeed = 2f;

	public Vector2 AttackIntervall;

	public AudioClip OnHitClip;

	public Blokfosk Blokfosk { get; set; }

	public Vector2 Direction { get; set; }

	public bool IsHit { get; set; }

	protected float _attackTimer;
	protected float _counter;

	protected Rigidbody2D _rb;

	protected virtual void Awake ()
	{
		_rb = GetComponent<Rigidbody2D> ();
		Direction = transform.right;
		ResetAttackTimer ();
		Blokfosk = FindObjectOfType<Blokfosk> ();
	}

	protected virtual void Update ()
	{
		if (IsHit) {
			return;
		}

		_counter += Time.deltaTime;
		if (_counter >= _attackTimer) {
			FireProjectile ();
			ResetAttackTimer ();
		}
	}

	private void FixedUpdate ()
	{
		_rb.position += Direction * MovementSpeed * Time.deltaTime;
	}

	private void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "Blokfosk_Tentacle") {
			if (OnHitClip) {
				AudioSource.PlayClipAtPoint (OnHitClip, transform.position);
			}
			IsHit = true;
			var tentaclePos = new Vector2 (collider.transform.position.x, collider.transform.position.y);
			TentacleHit (_rb.position - tentaclePos);
		}
	}

	protected virtual void TentacleHit (Vector2 dir)
	{
		Explode ();
	}

	public void Explode ()
	{
		var explode = GetComponent<Explodable> ();
		if (explode) {
			explode.Explode (transform.position);
		}
	}

	public void FireProjectile ()
	{
		var dir = Blokfosk == null ? transform.right : Blokfosk.transform.position - transform.position;
		var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		var rot = Quaternion.AngleAxis (angle, Vector3.forward);

		var go = (GameObject)Instantiate (ProjectilePrefab, transform.position, rot);
		var projectile = go.GetComponent<Projectile> ();


		projectile.Direction = dir.normalized;
	}

	public virtual void FireAlternative ()
	{

	}

	private void ResetAttackTimer ()
	{
		_counter = 0f;
		RandomizeAttackTimer ();
	}

	private void RandomizeAttackTimer ()
	{
		_attackTimer = Random.Range (AttackIntervall.x, AttackIntervall.y);
	}
}
