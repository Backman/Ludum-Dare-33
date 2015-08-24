﻿using UnityEngine;
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

			StartCoroutine (StartLoadScene ());

			_loadScene = true;
		}

		if (_loadScene && FadeSettings.DoneFading) {
			Application.LoadLevel (SceneToLoad);
		}
	}

	private IEnumerator StartLoadScene ()
	{
		Fader.Instance.Fade (FadeSettings);


		while (!FadeSettings.DoneFading) {
			yield return null;
		}

		yield return new WaitForSeconds (0.1f);

		Application.LoadLevel (SceneToLoad);
	}
}
