using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnderWaterData
{
	public float TentacleSpeed = 20f;
	public float TentacleSinkSpeed = 15f;
	public float RotationSpeed = 45f;
}

[System.Serializable]
public class AirData
{
	public float TentacleSpeed = 20f;
	public float TentacleFallSpeed = 25f;
	public float RotationSpeed = 50f;
}

[System.Serializable]
public class TentacleData
{
	public UnderWaterData UnderWater;
	public AirData InAir;
}

[System.Serializable]
public class VelocitySettings
{
	public float Tolerence = 0.5f;
	public float DecaySpeed = 2f;
	[Range (0.0f, 1.0f)]
	public float SplashDecay = 0.5f;
}

public class Blokfosk : MonoBehaviour
{
	#if UNITY_STANDALONE_OSX
	private const string RightStickHorizontal = "RightStickX";
	private const string RightStickVertical = "RightStickY";

	private const string LeftStickHorizontal = "LeftStickX";
	private const string LeftStickVertical = "LeftStickY";

	private const string HypeButton = "Hype";

	private const string LeftRotationButton = "LeftRotation";
	private const string RightRotationButton = "RightRotation";
	#elif UNITY_STANDALONE_WIN
	private const string RightStickHorizontal = "RightStickX_WIN";
	private const string RightStickVertical = "RightStickY_WIN";

	private const string LeftStickHorizontal = "LeftStickX_WIN";
	private const string LeftStickVertical = "LeftStickX_WIN";

	private const string HypeButton = "Fire1";

	private const string LeftRotationButton = "LeftRotation_WIN";
	private const string RightRotationButton = "RightRotation_WIN";
	#endif
	public ShakeSettings HypeShake;

	public TentacleData TentacleSettings;
	public VelocitySettings VelocitySettings;


	public Tentacle LeftTentacle;
	public Tentacle RightTentacle;


	public CircleCollider2D TargetBounds;

	public AnimationCurve HypeCurve;
	public float MaxHype = 5f;
	public float HypeBuildUp = 1f;


	public bool InAir { get; set; }

	private Rigidbody2D _rb;

	private bool _isHyping;
	private bool _inAir;
	private bool _decayVelocity;
	private bool _prevInAir;
	private bool _hypeMode;
	private float _currentHype;

	private FollowCamera _followCamera;
	private FollowCamera.ShakeID _hypeShakeID;

	private void Awake ()
	{
		_rb = GetComponent<Rigidbody2D> ();
	}

	private void Start ()
	{
		_followCamera = Camera.main.GetComponent<FollowCamera> ();
		_hypeShakeID = _followCamera.AddShake (HypeShake, 0f, Vector2.zero);
	}

	private void Update ()
	{
		var rightStick = GetJoystickAxis (RightStickHorizontal, RightStickVertical);
		var leftStick = GetJoystickAxis (LeftStickHorizontal, LeftStickVertical);

		TentacleInput (LeftTentacle, leftStick);
		TentacleInput (RightTentacle, rightStick);

		ApplyTentacleConstraint (LeftTentacle);
		ApplyTentacleConstraint (RightTentacle);

		HypeInput ();

		_followCamera.SetTarget (_rb.position, _rb.velocity);

		if (_isHyping) {
			_currentHype = Mathf.Clamp (_currentHype, 0f, MaxHype);
			_followCamera.UpdateShake (_hypeShakeID, HypeCurve.Evaluate (_currentHype / MaxHype), Vector2.zero);
		} else {
			_followCamera.UpdateShake (_hypeShakeID, 0f, Vector2.zero);
		}

		InAir = transform.position.y > 0f;

		if (InAir) {
			AirRotation ();
		}

		if (_hypeMode && InAir) {
			_rb.gravityScale = 1f;
		} else if (_hypeMode && !InAir && _prevInAir) {
			_rb.velocity *= VelocitySettings.SplashDecay;
			_decayVelocity = true;
		}

		if (_decayVelocity) {
			var xVel = _rb.velocity.x;
			var yVel = _rb.velocity.y;

			_rb.velocity = Vector2.Lerp (_rb.velocity, Vector2.zero, VelocitySettings.DecaySpeed * Time.deltaTime);

			_rb.gravityScale = 0f;

			if (_rb.velocity.magnitude <= VelocitySettings.Tolerence) {
				_rb.velocity = Vector2.zero;
				_hypeMode = false;
				_decayVelocity = false;
			}
		}

		_prevInAir = InAir;
	}

	private void FixedUpdate ()
	{
		if (_isHyping) {
			_rb.MovePosition (_rb.position + Vector2.down * 2f * Time.deltaTime);
		}

//		if (_inAir) {
//			var dir = _rb.velocity;
//			var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
//			var q = Quaternion.AngleAxis (angle, Vector3.forward);
//			transform.rotation = q;
//		}
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

	private void ApplyTentacleConstraint (Tentacle tentacle)
	{
		var center = TargetBounds.bounds.center;
		var radius = TargetBounds.radius;

		var delta = tentacle.Target.position - center;
		if (delta.sqrMagnitude >= radius * radius) {
			
			tentacle.SetTarget (center + delta.normalized * radius);
		}
	}

	private void HypeInput ()
	{
		if (_hypeMode) {
			return;
		}

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
		if (_currentHype <= 0f) {
			StartCoroutine (RotateUp ());
		}

		HypeRotation ();
		_currentHype += HypeBuildUp * Time.deltaTime;
	}

	private IEnumerator RotateUp ()
	{
		var t = 0f;
		var totalT = 1f;
		while (t <= totalT) {
			_rb.rotation = Mathf.LerpAngle (_rb.rotation, 0f, t / totalT);

			t += Time.deltaTime;
			yield return null;
		}
	}

	private void RotationInput (out bool leftRot, out bool rightRot)
	{
		leftRot = Input.GetButton (LeftRotationButton);
		rightRot = Input.GetButton (RightRotationButton);
	}

	private void AirRotation ()
	{
		bool leftRot, rightRot;
		RotationInput (out leftRot, out rightRot);

		var rot = TentacleSettings.InAir.RotationSpeed * Time.deltaTime;

		if (leftRot) {
			_rb.rotation += rot;
		} else if (rightRot) {
			_rb.rotation -= rot;
		}
	}

	private void HypeRotation ()
	{
		bool leftRot, rightRot;
		RotationInput (out leftRot, out rightRot);

		var rot = TentacleSettings.UnderWater.RotationSpeed * Time.deltaTime;

		if (leftRot) {
			_rb.rotation += rot;
		} else if (rightRot) {
			_rb.rotation -= rot;
		}

		_rb.rotation = Mathf.Clamp (_rb.rotation, -50f, 50f);
	}

	private void MLGHype ()
	{
		// SHOOT THE FAKKING BLOKFOSK INTO THE AIR!
		_rb.velocity = transform.up * (_currentHype / MaxHype) * 25f;
		_currentHype = 0f;
		_hypeMode = true;
	}

	private Vector2 GetJoystickAxis (string horizontal, string vertical)
	{
		var x = Input.GetAxisRaw (horizontal);
		var y = Input.GetAxisRaw (vertical);

		return new Vector2 (x, y);
	}
}
