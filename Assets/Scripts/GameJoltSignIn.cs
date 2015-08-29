using UnityEngine;

public class GameJoltSignIn : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !GameJoltPipe.Instance.SigningIn)
		{
			GameJoltPipe.Instance.SignIn();
		}
	}
}
