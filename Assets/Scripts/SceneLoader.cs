using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
	public string SceneToLoad;

	public FadeSettings FadeSettings;

	private bool _loadScene;

	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Fader.Instance.Fade (FadeSettings);
			_loadScene = true;
		}

		if (_loadScene && FadeSettings.DoneFading) {
			Application.LoadLevel (SceneToLoad);
		}
	}
}
