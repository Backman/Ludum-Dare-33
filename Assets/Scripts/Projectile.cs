using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public float TimeAlive = 2f;
	public float MovementSpeed = 1f;
	public float ExplosionRadius = 1f;
	public float Damage = 5f;

	public Vector2 Direction { get; set; }

	private float _timer;
	private Rigidbody2D _rb;

	private void Awake ()
	{
		_rb = GetComponent<Rigidbody2D> ();
	}

	private void Update ()
	{
		_timer += Time.deltaTime;
		if (_timer >= TimeAlive) {
			GetComponent<Explodable> ().Explode (transform.position);
		}
	}

	private void FixedUpdate ()
	{
		_rb.position += Direction * MovementSpeed * Time.deltaTime;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		var blok = other.GetComponent<Blokfosk> ();
		if (blok) {
			blok.TakeDamage (Damage);
		}
	}

	void WaterSurfaceEnter ()
	{
		GetComponent<Explodable> ().Explode (transform.position);
	}
}
