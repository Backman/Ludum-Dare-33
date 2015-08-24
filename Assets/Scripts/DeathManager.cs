using UnityEngine;
using System.Collections;

public class DeathManager : MonoBehaviour
{
	public string DeathScreenScene;

	private void Awake ()
	{
		GameLogic.Instance.OnBlokfoskRIP += OnRIP;
	}

	public void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			OnRIP ();
		}
	}

	private void OnRIP ()
	{
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
