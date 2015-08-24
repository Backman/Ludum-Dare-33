using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WrekFaceHandler : MonoBehaviour
{
	public float RekDuration = 0.08f;
	[Range (0.0f, 1.0f)]
	public float FreezeValue = 0.0f;
	public Color BlinkColor;
	public float BlinkDuration;
	public float BlokfoskBlinkDuration;
	public AnimationCurve BlinkCurve;
	public float MaxDistance = 10f;
	public float MinDistance = 5f;
	public float ZoomLerpModifier = 20f;
	bool _HasFreezeTime = false;

	public RekCombo RekCombo;

	void Awake ()
	{
		GameLogic.Instance.OnRekFace += OnRekFace;
		GameLogic.Instance.OnBlokDamage += OnBlokDamage;

		RekCombo.Init ();
	}

	void OnBlokDamage (int currentHealth)
	{
		BlinkManager.Instance.AddBlink (Blokfosk.Instance.gameObject, BlinkColor, BlokfoskBlinkDuration);
	}

	void OnRekFace (GameObject obj)
	{
		var distance = Vector2.Distance (obj.transform.position, Blokfosk.Instance.transform.position);

		var rekDuration = RekDuration;
		var freezeValue = FreezeValue;

		float distanceMod = Mathf.Clamp01 ((distance - MinDistance) / MaxDistance);
		freezeValue *= 1 - distanceMod;
		rekDuration *= 1 + distanceMod;

		RekCombo.IncreaseRekComboCount (obj);

		BlinkManager.Instance.AddBlink (obj, BlinkColor, BlinkDuration, BlinkCurve);
		if (_HasFreezeTime == false) {
			StartCoroutine (FreezeTime (obj, rekDuration, freezeValue));
			_HasFreezeTime = true;
		}
	}

	IEnumerator FreezeTime (GameObject obj, float rekDuration, float freezeValue)
	{
		float startTime = Time.unscaledTime;
		Time.timeScale = freezeValue;
		while (startTime + rekDuration > Time.unscaledTime) {
			var followCam = Camera.main.GetComponent<FollowCamera> ();
			Vector3 pos = obj.transform.position;
			followCam.SetTarget (pos, Vector2.zero, 0, 1, ZoomLerpModifier);
			yield return null;
		}
		Time.timeScale = 1f;
		_HasFreezeTime = false;
	}

	public void Update ()
	{
		RekCombo.Update ();
	}
}
