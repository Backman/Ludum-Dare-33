using UnityEngine;
using System.Collections.Generic;

public class FollowCamera : MonoBehaviour
{
	struct ActiveShakeData
	{
		public ShakeSettings Settings;
		public float StartTime;
		public float Strength;
		public Vector2 Direction;
		public int ID;
	}

	public float LerpModifier;

	Vector2 _CurrentPos;
	Vector2 _TargetPos;
	Vector2 _TargetVelocity;
	int _ShakeID;

	List<ActiveShakeData> _ActiveShakes = new List<ActiveShakeData> ();

	void Update ()
	{
		Vector2 targetLookpos = _TargetPos + _TargetVelocity * Time.deltaTime;

		Vector2 shakeAdd = Vector2.zero;

		for (int i = _ActiveShakes.Count - 1; i >= 0; i--) {
			var shake = _ActiveShakes [i];
			var settings = shake.Settings;
			float t = Time.time - shake.StartTime;

			float lifetime = settings.Lifetime.Evaluate (t);
			if (t > lifetime && lifetime != 0f) {
				_ActiveShakes.RemoveAt (i);
				continue;
			}

			float normalizedT = t;
			if (lifetime != 0f)
				normalizedT = t / lifetime;

			shakeAdd.x += settings.X.Evaluate (normalizedT) * shake.Strength;
			shakeAdd.y += settings.Y.Evaluate (normalizedT) * shake.Strength;
		}
		_CurrentPos = Vector2.Lerp (_CurrentPos, targetLookpos, LerpModifier * Time.deltaTime);

		transform.position = _CurrentPos + shakeAdd;
	}

	public void SetTarget (Vector2 position, Vector2 velocity)
	{
		_TargetPos = position;
		_TargetVelocity = velocity;
	}

	public struct ShakeID
	{
		public int ID;
	}

	public ShakeID AddShake (ShakeSettings shake, float strength, Vector2 dir)
	{
		ActiveShakeData data;
		data.Settings = shake;
		data.Strength = strength;
		data.Direction = dir;
		data.StartTime = Time.time;
		data.ID = _ShakeID++;
		_ActiveShakes.Add (data);
		return new ShakeID () { ID = data.ID };
	}

	public void UpdateShake (ShakeID id, float strength, Vector2 dir)
	{
		for (int i = 0; i < _ActiveShakes.Count; i++) {
			var shake = _ActiveShakes [i];
			if (shake.ID == id.ID) {
				shake.Strength = strength;
				shake.Direction = dir;
				_ActiveShakes [i] = shake;
			}
		}
	}

	public void StopShake (ShakeID id)
	{
		for (int i = 0; i < _ActiveShakes.Count; i++) {
			var shake = _ActiveShakes [i];
			if (shake.ID == id.ID) {
				_ActiveShakes.RemoveAt (i);
				break;
			}
		}
	}
}
