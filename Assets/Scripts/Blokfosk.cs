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
	public float MaxVelocity = 10f;
	public float Tolerence = 0.5f;
	public float DecaySpeed = 2f;
	public float BobThreshold = -1f;
	[Range (0.0f, 1.0f)]
	public float SplashDecay = 0.5f;
	public float ApplySplashDecayThreshold = 10f;

	public float SqrApplySplashDecayThreshold {
		get { return ApplySplashDecayThreshold * ApplySplashDecayThreshold; }
	}
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
	public float PushDown = 10f;

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
		CurrentHype = Mathf.Min (CurrentHype, MaxHype);
	}

	public void ResetHype ()
	{
		CurrentHype = 0f;
	}
}

public static class InputManager
{
	#if UNITY_STANDALONE_OSX
	private const string RightStickHorizontal = "RightStickX";
	private const string RightStickVertical = "RightStickY";

	private const string LeftStickHorizontal = "LeftStickX";
	private const string LeftStickVertical = "LeftStickY";

	private const string HypeButton = "Hype";

	private const string LeftTriggerButton = "LeftTrigger";
	private const string RightTriggerButton = "RightTrigger";
	#elif UNITY_STANDALONE_WIN
	private const string RightStickHorizontal = "RightStickX_WIN";
	private const string RightStickVertical = "RightStickY_WIN";

	private const string LeftStickHorizontal = "LeftStickX_WIN";
	private const string LeftStickVertical = "LeftStickX_WIN";

	private const string HypeButton = "Hype_WIN";

	private const string LeftTriggerButton = "LeftTrigger_WIN";
	private const string RightTriggerButton = "RightTrigger_WIN";
	#endif

	public static Vector2 GetLeftJoystick ()
	{
		var x = Input.GetAxisRaw (LeftStickHorizontal);
		var y = Input.GetAxisRaw (LeftStickVertical);

		return new Vector2 (x, y);
	}

	public static Vector2 GetRightJoystick ()
	{

		var x = Input.GetAxisRaw (RightStickHorizontal);
		var y = Input.GetAxisRaw (RightStickVertical);

		return new Vector2 (x, y);
	}

	public static bool LeftRotation ()
	{
		return Input.GetButton (LeftTriggerButton);
	}

	public static bool RightRotation ()
	{
		return Input.GetButton (RightTriggerButton);
	}

	public static bool GetTriggers ()
	{
		var left = Input.GetButton (LeftTriggerButton);
		var right = Input.GetButton (RightTriggerButton);
		return left && right;
	}
}

public class Blokfosk : MonoBehaviour
{


	public static Blokfosk Instance;
	public int MaxHealth = 100;

	public Hype Hype;

	public GameObject InkParticle;

	public TentacleData TentacleSettings;
	public VelocitySettings VelocitySettings;

	public Tentacle LeftTentacle;
	public Tentacle RightTentacle;

	public float ResetRotationSpeed = 10f;
	public float InkBoost = 20f;

	public CircleCollider2D TargetBounds;

	public Vector2 RotationThreshold = new Vector2 (45f, 45f);

	public int Health { get; set; }

	public bool InAir { get; set; }

	public bool UnderThresholdLevel { get; set; }

	private Rigidbody2D _rb;
	private Animator _animator;

	private FollowCamera _followCamera;

	private bool _inAir;
	private bool _prevInAir;
	private bool _usedInkBoost;

	private void Awake ()
	{
		_rb = GetComponent<Rigidbody2D> ();
		Instance = this;
		_animator = GetComponentInChildren<Animator> ();
	}

	private void Start ()
	{
		_followCamera = Camera.main.GetComponent<FollowCamera> ();
		Hype.AddShakeCamera (_followCamera);
	}

	private void Update ()
	{
		var rightStick = InputManager.GetRightJoystick ();
		var leftStick = InputManager.GetLeftJoystick ();

		TentacleInput (LeftTentacle, leftStick);
		TentacleInput (RightTentacle, rightStick);

		ApplyTentacleConstraint (LeftTentacle);
		ApplyTentacleConstraint (RightTentacle);

		Health = Mathf.Clamp (Health, 0, MaxHealth);

		_followCamera.SetTarget (_rb.position, _rb.velocity, Hype.NormalizedHype * 20f - _rb.velocity.magnitude / 3f, 0);

		HypeInput ();
		Hype.Update ();

		if (InAir) {
			InkBoostInput ();
		}

		ApplyRotation ();
	}

	private void FixedUpdate ()
	{
		InAir = _rb.position.y > 0.5f;
		UnderThresholdLevel = _rb.position.y < VelocitySettings.BobThreshold;

		if (Hype.IsHyping) {
			_rb.velocity = Vector2.zero;
			_rb.position += Vector2.down * Hype.PushDown * Time.deltaTime;
		}

		HolyShitballs ();

		_rb.velocity = Vector2.ClampMagnitude (_rb.velocity, VelocitySettings.MaxVelocity);

		_prevInAir = InAir;
	}

	public void TakeDamage (int amount)
	{
		StartCoroutine (PlayDamageAnimation ());

		Health -= amount;
		Health = Mathf.Min (0, MaxHealth);
		GameLogic.Instance.OnBlokDamage.Invoke ();
	}


	public void Heal (int amount)
	{
		Health -= amount;
		Health = Mathf.Min (0, MaxHealth);
	}

	private IEnumerator PlayDamageAnimation ()
	{
		if (_animator.GetBool ("TakeDamage")) {
			yield break;
		}

		_animator.SetBool ("TakeDamage", true);
		yield return new WaitForSeconds (0.2f);
		_animator.SetBool ("TakeDamage", false);
	}

	private void HolyShitballs ()
	{
		if (InAir) {
			_rb.gravityScale = 1f;
			if (Hype.InHypeMode) {
				_animator.SetBool ("HypeMode", false);
			}
		} else if (UnderThresholdLevel) {
			if (!Hype.IsHyping && !Hype.InHypeMode) {
				_rb.gravityScale = -0.7f;
				_rb.velocity = Vector2.Lerp (_rb.velocity, Vector2.zero, VelocitySettings.DecaySpeed * Time.deltaTime);
			} else {
				_rb.gravityScale = 0f;
			}
		} else {
			_rb.gravityScale = 0.4f;
			if (_prevInAir) {
				_usedInkBoost = false;
				Hype.InHypeMode = false;
				if (_rb.velocity.magnitude > VelocitySettings.ApplySplashDecayThreshold) {
					_rb.velocity *= VelocitySettings.SplashDecay;
				}
			}
		}

		if (!InAir && Hype.InHypeMode) {
			var rot = transform.up;
			var vel = _rb.velocity;
			_rb.velocity = rot.normalized * vel.magnitude;
			_rb.gravityScale = 0f;
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

	private void InkBoostInput ()
	{
		if (_usedInkBoost) {
			return;
		}

		var boost = InputManager.GetTriggers ();

		if (boost) {
			var forward = transform.up;
			var rot = new Vector2 (forward.x, forward.y);
			_rb.velocity += rot.normalized * InkBoost;
			_usedInkBoost = true;
			if (InkParticle) {
				Instantiate (InkParticle, transform.position, transform.rotation);
			}
		}
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

		var hypeDown = InputManager.GetTriggers ();

		if (!Hype.IsHyping && hypeDown) {
			_animator.SetTrigger ("Hyping");
			Hype.IsHyping = true;
		}

		if (!hypeDown && Hype.IsHyping) {
			Hype.IsHyping = false;
			MLGHype ();
		}
	}

	private void ApplyRotation ()
	{
		if (Hype.IsHyping) {
			return;
		}

		var leftRot = InputManager.LeftRotation ();
		var rightRot = InputManager.RightRotation ();

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
			_rb.rotation = Mathf.Clamp (_rb.rotation, RotationThreshold.x, RotationThreshold.y);
		}
	}

	private void MLGHype ()
	{
		// SHOOT THE FAKKING BLOKFOSK INTO THE AIR!
		var hype = Hype.BaseVelocity + (Hype.NormalizedHype * Hype.HypeBoost);

		_rb.velocity = transform.up * hype;
		Hype.ResetHype ();
		Hype.InHypeMode = true;
		_animator.SetBool ("HypeMode", Hype.InHypeMode);
		Instantiate (InkParticle, transform.position, transform.rotation);
	}

	private Vector2 GetJoystickAxis (string horizontal, string vertical)
	{
		var x = Input.GetAxisRaw (horizontal);
		var y = Input.GetAxisRaw (vertical);

		return new Vector2 (x, y);
	}

	void WaterSurfaceEnter (object obj)
	{
		var velocity = _rb.velocity;
		if (velocity.magnitude > 1f) {
			WaterSurface surface = obj as WaterSurface;
			surface.DoSplash (gameObject, transform.position);
		}
	}
}
