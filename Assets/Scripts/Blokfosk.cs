using UnityEngine;
using System.Collections;

[System.Serializable]
public class TentacleData
{
	public float TentacleSpeed = 20f;
	public float WaterSinkSpeed = 15f;
	public float AirSinkSpeed = 25f;
}

public class Blokfosk : MonoBehaviour
{
	private const string RightStickHorizontal = "RightStickX";
	private const string RightStickVertical = "RightStickY";

	private const string LeftStickHorizontal = "LeftStickX";
	private const string LeftStickVertical = "LeftStickY";

	public TentacleData TentacleSettings;
	public Tentacle LeftTentacle;
	public Tentacle RightTentacle;

	public BoxCollider2D LeftTentacleBounds;
	public BoxCollider2D RightTentacleBounds;

	private Rigidbody2D _rb;

	private void Awake ()
	{
		_rb = GetComponent<Rigidbody2D> ();
	}

	private void Start ()
	{

	}

	private void Update ()
	{
		var rightStick = GetJoystickAxis (RightStickHorizontal, RightStickVertical);
		var leftStick = GetJoystickAxis (LeftStickHorizontal, LeftStickVertical);

		MoveTentacle (LeftTentacle, leftStick);
		MoveTentacle (RightTentacle, rightStick);


		ConstraintTargetPositions ();
	}

	private void MoveTentacle (Tentacle tentacle, Vector2 axis)
	{
		var move = axis;

		var tentacleSpeed = TentacleSettings.TentacleSpeed;

		if (move.sqrMagnitude <= 0f) {
			move = Vector2.down;
			tentacleSpeed = TentacleSettings.WaterSinkSpeed;
		}

		tentacle.MoveTarget (move * tentacleSpeed * Time.deltaTime);
	}

	private void ConstraintTargetPositions ()
	{
		var leftBounds = LeftTentacleBounds.bounds;
		var rightBounds = RightTentacleBounds.bounds;

		if (!leftBounds.Contains (LeftTentacle.Target.position)) {
			var closestPoint = leftBounds.ClosestPoint (LeftTentacle.Target.position);
			LeftTentacle.SetTarget (closestPoint);

			//			_rb.AddTorque (10f, ForceMode2D.Impulse);

		}

		if (!rightBounds.Contains (RightTentacle.Target.position)) {
			var closestPoint = rightBounds.ClosestPoint (RightTentacle.Target.position);

			var delta = RightTentacle.Target.position - rightBounds.min;
			if (delta.x < 0f) {
				_rb.AddTorque (-3f, ForceMode2D.Force);
			}

			RightTentacle.SetTarget (closestPoint);
		}
	}

	private Vector2 GetJoystickAxis (string horizontal, string vertical)
	{
		var x = Input.GetAxisRaw (horizontal);
		var y = Input.GetAxisRaw (vertical);

		return new Vector2 (x, y);
	}
}
