using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public float TimeAlive = 2f;
	public float MovementSpeed = 1f;
	public float ExplosionRadius = 1f;

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

    void WaterSurfaceEnter()
    {
        GetComponent<Explodable> ().Explode (transform.position);
    }
}
