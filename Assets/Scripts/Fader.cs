using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FadingType
{
	FadeIn,
	FadeOut
}


[System.Serializable]
public class FadeSettings
{
	public FadingType Type;
	public float Duration = -1f;

	[HideInInspector]
	public bool DoneFading = true;

	public bool IsValid ()
	{
		return Duration > 0f && DoneFading;
	}
}

public class Fader : MonoBehaviour
{
	public static Fader Instance;
	public FadeSettings StartFadeSettings;
	public bool FadeOnStart = false;

	private Image _image;

	private void Awake ()
	{
		if (Instance != null) {
			Destroy (gameObject);
			return;
		}

		Instance = this;
		_image = GetComponent<Image> ();
	}

	public void Start ()
	{
		if (FadeOnStart) {
			var color = _image.color;
			if (StartFadeSettings.Type == FadingType.FadeIn) {
				color.a = 0f;
			} else if (StartFadeSettings.Type == FadingType.FadeOut) {
				color.a = 1f;
			}
			_image.color = color;

			Fade (StartFadeSettings);
		}
	}

	public void Fade (FadeSettings settings)
	{
		StartCoroutine (DoFade (settings));
	}

	private IEnumerator DoFade (FadeSettings settings)
	{
		if (!settings.IsValid ()) {
			yield break;
		}

		settings.DoneFading = false;

		var duration = settings.Duration;
		var type = settings.Type;
		float t;
		if (type == FadingType.FadeIn) {
			t = 0f;
			while (t < duration) {
				var color = _image.color;

				color.a = t / duration;

				_image.color = color;

				t += Time.deltaTime;
				yield return null;
			}
		} else if (type == FadingType.FadeOut) {
			t = duration;
			while (t > 0f) {
				var color = _image.color;

				color.a = t / duration;

				_image.color = color;

				t -= Time.deltaTime;
				yield return null;
			}
		}

		settings.DoneFading = true;
	}
}
