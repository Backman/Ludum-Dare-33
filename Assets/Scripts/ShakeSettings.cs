using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ShakeSettings : ScriptableObject
{
	public AnimationCurve X;
	public AnimationCurve Y;
	public AnimationCurve Lifetime;
}
