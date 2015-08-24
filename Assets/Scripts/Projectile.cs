using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public float TimeAlive = 2f;
	public float MovementSpeed = 1f;
	public float ExplosionRadius = 1f;
	public int Damage = 5;

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
			var radii = ExplosionRadius * ExplosionRadius;
			var blokCollider = Physics2D.OverlapCircle (_rb.position, ExplosionRadius, 1 << 8);
			TryDoDamage (blokCollider);
			DoExplode ();
		}
	}

	private void FixedUpdate ()
	{
		_rb.position += Direction * MovementSpeed * Time.deltaTime;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		TryDoDamage (other);
	}

	private void TryDoDamage (Collider2D collider)
	{
		if (collider == null) {
			return;
		}

		var blok = collider.GetComponent<Blokfosk> ();
		if (blok) {
			blok.TakeDamage (Damage);
			DoExplode ();
		}
	}

	private void DoExplode ()
	{
		var explode = GetComponent<Explodable> ();
		if (explode) {
			explode.Explode (transform.position, true);
		}
	}

	void WaterSurfaceEnter ()
	{
		DoExplode ();
	}
}
