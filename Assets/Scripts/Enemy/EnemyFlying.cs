using UnityEngine;
using System.Collections;

public class EnemyFlying : Enemy
{
	public override EnemyType Type { get { return EnemyType.Flying; } }

	public float MaxYValue = 3.0f;
	public float MinYValue = 0.5f;
	public float PlanaUtSpeed = 60f;
	private float _targetY;

	private bool _planaUt;

	protected override void Update ()
	{
		base.Update ();
		if (IsHit && transform.position.y < -0.1f) {
			Explode (true);
		}

		if (!_planaUt && _rb.position.y <= _targetY) {
			_planaUt = true;
			var dir = Direction;
			dir.y = 0f;
			Direction = dir;
		}
	}

	protected override void FixedUpdate ()
	{
		if (!_planaUt) {
			var angle = Mathf.Atan2 (Direction.y, Direction.x) * Mathf.Rad2Deg;
			angle = Direction.x < 0f ? angle - 180f : angle;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}

		if (_planaUt) {
			_rb.rotation = Mathf.MoveTowardsAngle (_rb.rotation, 0f, PlanaUtSpeed * Time.deltaTime);
		}
		base.FixedUpdate ();
	}

	protected override void CheckOtherCollisions (Collider2D collider)
	{
		var boat = collider.GetComponent<EnemyBoat> ();
		if (boat && boat.IsHit) {
			GameLogic.Instance.OnRekFace.Invoke (gameObject);
			var dir = boat.transform.position - transform.position;
			Hit (dir);
			Explode (true);
		}

		var flying = collider.GetComponent<EnemyFlying> ();
		if (flying && flying.IsHit) {
			GameLogic.Instance.OnRekFace.Invoke (gameObject);
			var dir = flying.transform.position - transform.position;
			Hit (dir);
			Explode (true);
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
		if (!IsHit) {
			Explode (false);
		}

		Hit (dir);
	}

	public override void Reset ()
	{
		base.Reset ();

		var dir = Blokfosk.transform.position - transform.position;
		_targetY = Random.Range (MinYValue, MaxYValue);
		dir.y = -_targetY;

		Direction = dir.normalized;
		_planaUt = false;
	}
}
