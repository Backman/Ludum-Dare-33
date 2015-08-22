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
	#if UNITY_STANDALONE_OSX
	private const string RightStickHorizontal = "RightStickX";
	private const string RightStickVertical = "RightStickY";

	private const string LeftStickHorizontal = "LeftStickX";
	private const string LeftStickVertical = "LeftStickY";

	private const string HypeButton = "Hype";
	#elif UNITY_STANDALONE_WIN
	private const string RightStickHorizontal = "RightStickX_WIN";
	private const string RightStickVertical = "RightStickY_WIN";

	private const string LeftStickHorizontal = "LeftStickX_WIN";
	private const string LeftStickVertical = "LeftStickX_WIN";

	private const string HypeButton = "Fire1";
	#endif

	public TentacleData TentacleSettings;
	public Tentacle LeftTentacle;
	public Tentacle RightTentacle;

	public CircleCollider2D LeftTentacleBounds;
	public CircleCollider2D RightTentacleBounds;

	public float MaxHype = 5f;
	public float HypeBuildUp = 1f;

	public bool InAir { get; set; }

	private Rigidbody2D _rb;

	private bool _isHyping;
	[SerializeField]
	private float _currentHype;

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

//		rightStick = Body.transform.InverseTransformDirection (new Vector3 (rightStick.x, rightStick.y));
//		leftStick = Body.transform.InverseTransformDirection (new Vector3 (leftStick.x, leftStick.y));

		TentacleInput (LeftTentacle, leftStick);
		TentacleInput (RightTentacle, rightStick);

		ApplyTentacleConstraint (LeftTentacle, LeftTentacleBounds);
		ApplyTentacleConstraint (RightTentacle, RightTentacleBounds);

//		ApplyRotation ();

		HypeInput ();
	}

	private void FixedUpdate ()
	{
		if (_isHyping) {
			_rb.MovePosition (_rb.position + Vector2.down * Time.deltaTime);
		}
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

	private void ApplyRotation ()
	{
		var diff = LeftTentacle.Target.position - RightTentacle.Target.position;


		var angle = Vector3.Angle (LeftTentacle.Target.position, RightTentacle.Target.position);

		var leftDiff = transform.position - LeftTentacle.Target.position;
		var rightDiff = transform.position - RightTentacle.Target.position;

		var speed = InAir ? TentacleSettings.InAir.RotationSpeed : TentacleSettings.UnderWater.RotationSpeed;
		transform.Rotate (0f, 0f, angle * speed * Time.deltaTime);
	}

	private void HypeInput ()
	{
		var hypeDown = Input.GetButton (HypeButton);
		if (hypeDown) {
			_isHyping = true;
		}

		if (!hypeDown && _isHyping) {
			_isHyping = false;
			MLGHype ();
		}

		if (_isHyping) {
			BuildHype ();
		}
	}

	private void BuildHype ()
	{
		Debug.Log ("HYPE BUILDUP");
		_currentHype += HypeBuildUp * Time.deltaTime;
	}

	private void MLGHype ()
	{
		// SHOOT THE FAKKING BLOKFOSK INTO THE AIR!
		Debug.LogFormat ("HYPEEEE: {0}/{1}", _currentHype, MaxHype);
		_rb.AddForce (transform.up * _currentHype * 1000f);
		_currentHype = 0f;
	}

	private Vector2 GetJoystickAxis (string horizontal, string vertical)
	{
		var x = Input.GetAxisRaw (horizontal);
		var y = Input.GetAxisRaw (vertical);

		return new Vector2 (x, y);
	}
}
