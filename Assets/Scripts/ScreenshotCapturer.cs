using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

public class ScreenshotCapturer : MonoBehaviour
{
	public KeyCode Key = KeyCode.Print;
	public int SuperSize = 0;

	public static ScreenshotCapturer Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyDown(Key))
		{
			TakeScreenshot(SuperSize);
		}
	}

	public void TakeScreenshot(int superSize)
	{
		var now = DateTime.Now;
		var filename = string.Format(@"Screenshot_{0:MM/dd/yy_H-mm-ss}.png", now);
		Application.CaptureScreenshot(filename, superSize);
	}
}
