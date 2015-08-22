using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnderWaterData
{
	public float TentacleSpeed = 20f;
	public float TentacleSinkSpeed = 15f;
	public float RotationSpeed = 2f;
}

[System.Serializable]
public class AirData
{
	public float TentacleSpeed = 20f;
	public float TentacleFallSpeed = 25f;
	public float RotationSpeed = 2f;
}

[System.Serializable]
public class TentacleData
{
	public UnderWaterData UnderWater;
	public AirData InAir;
}

public class Blokfosk : MonoBehaviour
{
	private const string RightStickHorizontal = "RightStickX";
	private const string RightStickVertical = "RightStickY";

	private const string LeftStickHorizontal = "LeftStickX";
	private const string LeftStickVertical = "LeftStickY";

	public TentacleData TentacleSettings;
	public GameObject Body;
	public Tentacle LeftTentacle;
	public Tentacle RightTentacle;

	public CircleCollider2D LeftTentacleBounds;
	public CircleCollider2D RightTentacleBounds;

	public bool InAir { get; set; }

	private void Awake ()
	{
		
	}

	private void Start ()
	{

	}

	private void Update ()
	{
		var rightStick = GetJoystickAxis (RightStickHorizontal, RightStickVertical);
		var leftStick = GetJoystickAxis (LeftStickHorizontal, LeftStickVertical);

//		rightStick = Body.transform.InverseTransformDirection (new Vector3 (rightStick.x, rightStick.y));
//		leftStick = Body.transform.InverseTransformDirection (new Vector3 (leftStick.x, leftStick.y));

		TentacleInput (LeftTentacle, leftStick);
		TentacleInput (RightTentacle, rightStick);

		ApplyTentacleConstraint (LeftTentacle, LeftTentacleBounds);
		ApplyTentacleConstraint (RightTentacle, RightTentacleBounds);

		var diff = LeftTentacle.Target.position - RightTentacle.Target.position;
		

		var angle = Vector3.Angle (LeftTentacle.Target.position, RightTentacle.Target.position);

		var leftDiff = Body.transform.position - LeftTentacle.Target.position;
		var rightDiff = Body.transform.position - RightTentacle.Target.position;

		var speed = InAir ? TentacleSettings.InAir.RotationSpeed : TentacleSettings.UnderWater.RotationSpeed;
		Body.transform.Rotate (0f, 0f, angle * speed * Time.deltaTime);
	}

	private void TentacleInput (Tentacle tentacle, Vector2 input)
	{
		var tentacleSpeed = InAir ? TentacleSettings.InAir.TentacleSpeed : TentacleSettings.UnderWater.TentacleSpeed;

		if (input.sqrMagnitude <= 0f) {
			input = Vector2.down;
			tentacleSpeed = InAir ? TentacleSettings.InAir.TentacleFallSpeed : TentacleSettings.UnderWater.TentacleSinkSpeed;
		}

		tentacle.MoveTarget (input * tentacleSpeed * Time.deltaTime);
	}

	private void ApplyTentacleConstraint (Tentacle tentacle, CircleCollider2D collider)
	{
		var center = collider.bounds.center;
		var radius = collider.radius;

		var delta = tentacle.Target.position - center;
		if (delta.sqrMagnitude >= radius * radius) {
			
			tentacle.SetTarget (center + delta.normalized * radius);
		}
	}

	private Vector2 GetJoystickAxis (string horizontal, string vertical)
	{
		var x = Input.GetAxisRaw (horizontal);
		var y = Input.GetAxisRaw (vertical);

		return new Vector2 (x, y);
	}
}
