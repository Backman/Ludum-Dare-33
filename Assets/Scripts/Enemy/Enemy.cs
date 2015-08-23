using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
	[System.NonSerialized]
	public GameObject FromPrefab;
	public GameObject ProjectilePrefab;
	public float MovementSpeed = 2f;
	public int Score = 5;

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
		Blokfosk = Blokfosk.Instance;
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

	float _LastVisibleTime = 0f;

	void OnEnable ()
	{
		_LastVisibleTime = Time.time;
	}

	private void FixedUpdate ()
	{
		_rb.position += Direction * MovementSpeed * Time.deltaTime;
		var renderer = GetComponentInChildren<Renderer> ();
		if (renderer.isVisible) {
			_LastVisibleTime = Time.time;
		} else {
			if (_LastVisibleTime + 10f < Time.time) {
				var explode = GetComponent<Explodable> ();
				if (explode) {
					explode.Spawner.ReturnSpawnValue (gameObject);
					explode.Spawner.Despawn (gameObject);
				}
			}
		}
	}

	private void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "Blokfosk_Tentacle") {
			if (OnHitClip) {
				AudioSource.PlayClipAtPoint (OnHitClip, transform.position);
			}

			if (!IsHit) {
				GameLogic.Instance.AddScore (Score);
			}

			GameLogic.Instance.OnRekFace.Invoke (gameObject);
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
