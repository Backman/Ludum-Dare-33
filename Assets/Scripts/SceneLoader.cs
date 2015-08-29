using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
	public string SceneToLoad;
	private AudioSource _audioSource;
	public bool DestroyMenuMusic = false;

	public FadeSettings FadeSettings;

	private bool _loadScene;

	private void Awake ()
	{
		var menuMusic = GameObject.Find ("MenuMusic");
		if (menuMusic) {
			_audioSource = menuMusic.GetComponent<AudioSource> ();
			if (!DestroyMenuMusic) {
				DontDestroyOnLoad (menuMusic);
			}
		}
	}

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || GameJoltPipe.Instance.SigningIn || GameJoltPipe.Instance.ShowingLeaderboard)
		{
			return;
		}

		if (Input.anyKeyDown) {
			Fader.Instance.Fade (FadeSettings);

			StartCoroutine (StartLoadScene ());

			_loadScene = true;
		}

		if (_loadScene && FadeSettings.DoneFading) {
			Application.LoadLevel (SceneToLoad);
		}
	}

	private IEnumerator StartLoadScene ()
	{

		if (DestroyMenuMusic) {
			StartCoroutine (FadeMusic (FadeSettings.Duration));
		}
		Fader.Instance.Fade (FadeSettings);

		while (!FadeSettings.DoneFading) {
			yield return null;
		}

		yield return new WaitForSeconds (0.1f);

		Application.LoadLevel (SceneToLoad);
	}

	private IEnumerator FadeMusic (float duration)
	{
		var t = duration;
		while (t > 0f) {
			_audioSource.volume = t / duration;
			t -= Time.deltaTime;
			yield return null;
		}
	}
}
