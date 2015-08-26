using UnityEngine;
using System.Collections;

public class DeathManager : MonoBehaviour
{
	public string DeathScreenScene;

	private void Awake ()
	{
		GameLogic.Instance.OnBlokfoskRIP += OnRIP;
	}

	private void OnRIP ()
	{
		PlayerPrefs.SetInt("TOTAL_SCORE", GameLogic.Instance.Score);
		StartCoroutine (DoRIP ());
	}

	private IEnumerator DoRIP ()
	{
		var fade = new FadeSettings () {
			Type = FadingType.FadeIn,
			Duration = 0.8f
		};

		Fader.Instance.Fade (fade);
		while (!fade.DoneFading) {
			yield return null;
		}

		Application.LoadLevel (DeathScreenScene);
	}
}
