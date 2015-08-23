using UnityEngine;
using UnityEngine.Events;
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

[System.Serializable]
public class Hype
{
	public ShakeSettings HypeShake;
	public AnimationCurve Curve;
	public float BaseVelocity = 25f;
	public float MaxHype = 100f;
	public float HypeBuildUpSpeed = 25f;
	public float HypeBoost = 100f;

	private FollowCamera.ShakeID _hypeShakeID;

	public FollowCamera HypeCamera { get; set; }

	public float CurrentHype { get; set; }

	public bool IsHyping { get; set; }

	public bool InHypeMode { get; set; }

	public float NormalizedHype {
		get { return CurrentHype / MaxHype; }
	}

	public void Update ()
	{
		if (IsHyping) {
			CurrentHype = Mathf.Clamp (CurrentHype, 0f, MaxHype);
			HypeCamera.UpdateShake (_hypeShakeID, Curve.Evaluate (NormalizedHype), Vector2.zero);
		} else {
			HypeCamera.UpdateShake (_hypeShakeID, 0f, Vector2.zero);
		}

		if (IsHyping) {
			DoBuildUp ();
		}
	}

	public void AddShakeCamera (FollowCamera followCamera)
	{
		HypeCamera = followCamera;
		_hypeShakeID = HypeCamera.AddShake (HypeShake, 0f, Vector2.zero);
	}

	public void DoBuildUp ()
	{
		CurrentHype += HypeBuildUpSpeed * Time.deltaTime;
	}

	public void ResetHype ()
	{
		CurrentHype = 0f;
	}
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

	private const string HypeButton = "Hype_WIN";

	private const string LeftRotationButton = "LeftRotation_WIN";
	private const string RightRotationButton = "RightRotation_WIN";
	#endif

    public static Blokfosk Instance;

	public Hype Hype;

	public TentacleData TentacleSettings;
	public VelocitySettings VelocitySettings;

	public Tentacle LeftTentacle;
	public Tentacle RightTentacle;

	public float ResetRotationSpeed = 10f;

	public CircleCollider2D TargetBounds;

	public UnityEvent OnLand;
	public UnityEvent OnHypeReleased;


	public bool InAir { get; set; }

	private Rigidbody2D _rb;

	private FollowCamera _followCamera;
	
	private bool _inAir;
	private bool _prevInAir;

	private void Awake ()
	{
		_rb = GetComponent<Rigidbody2D> ();
        Instance = this;
	}

	private void Start ()
	{
		_followCamera = Camera.main.GetComponent<FollowCamera> ();
		Hype.AddShakeCamera (_followCamera);
	}

	private void Update ()
	{
		var rightStick = GetJoystickAxis (RightStickHorizontal, RightStickVertical);
		var leftStick = GetJoystickAxis (LeftStickHorizontal, LeftStickVertical);

		TentacleInput (LeftTentacle, leftStick);
		TentacleInput (RightTentacle, rightStick);

		ApplyTentacleConstraint (LeftTentacle);
		ApplyTentacleConstraint (RightTentacle);

		InAir = transform.position.y > 0f;

		_followCamera.SetTarget (_rb.position, _rb.velocity);

		HypeInput ();
		Hype.Update ();

		ApplyRotation ();

		if (InAir) {
			_rb.gravityScale = 1f;
		} else if (Hype.InHypeMode && !InAir && _prevInAir) {
			OnLand.Invoke ();
			_rb.velocity *= VelocitySettings.SplashDecay;
		} else if (!InAir) {
			_rb.velocity = Vector2.Lerp (_rb.velocity, Vector2.zero, VelocitySettings.DecaySpeed * Time.deltaTime);
			_rb.gravityScale = 0.1f;
		}

		if (!InAir && !Hype.IsHyping && Hype.InHypeMode) {
			if (_rb.velocity.magnitude <= VelocitySettings.Tolerence) {
				Hype.InHypeMode = false;
			}
		}


		_prevInAir = InAir;
	}

	private void FixedUpdate ()
	{
		if (Hype.IsHyping) {
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
		if (Hype.InHypeMode) {
			return;
		}

		var hypeDown = Input.GetAxisRaw (HypeButton) > 0.1f;
		if (hypeDown) {
			Hype.IsHyping = true;
		}

		if (!hypeDown && Hype.IsHyping) {
			Hype.IsHyping = false;
			MLGHype ();
		}
	}

	private void ApplyRotation ()
	{
		var leftRot = Input.GetButton (LeftRotationButton);
		var rightRot = Input.GetButton (RightRotationButton);
	
		var air = TentacleSettings.InAir;
		var water = TentacleSettings.UnderWater;

		var rot = InAir ? air.RotationSpeed : water.RotationSpeed;
		rot *= Time.deltaTime;

		if (leftRot) {
			_rb.rotation += rot;
		} else if (rightRot) {
			_rb.rotation -= rot;
		} else if (!InAir && !Hype.IsHyping && !Hype.InHypeMode) {
			_rb.rotation = Mathf.MoveTowardsAngle (_rb.rotation, 0f, ResetRotationSpeed * Time.deltaTime);
		}

		if (!InAir && (leftRot || rightRot)) {
			_rb.rotation = Mathf.Clamp (_rb.rotation, -80f, 80f);
		}
	}

	private void MLGHype ()
	{
		// SHOOT THE FAKKING BLOKFOSK INTO THE AIR!
		var hype = Hype.BaseVelocity + (Hype.NormalizedHype * Hype.HypeBoost);
		OnHypeReleased.Invoke ();

		_rb.velocity = transform.up * hype;
		Hype.ResetHype ();
		Hype.InHypeMode = true;
	}

	private Vector2 GetJoystickAxis (string horizontal, string vertical)
	{
		var x = Input.GetAxisRaw (horizontal);
		var y = Input.GetAxisRaw (vertical);

		return new Vector2 (x, y);
	}
}
