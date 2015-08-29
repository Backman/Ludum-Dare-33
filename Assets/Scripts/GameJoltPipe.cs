using UnityEngine;
using System.Collections;

public class GameJoltPipe : Singleton<GameJoltPipe>
{
	private GameJolt.API.Manager _apiManager;
	private GameJolt.UI.Manager _uiManager;

	public GameJolt.API.Objects.User User { get { return _apiManager.CurrentUser; } }
	public bool SigningIn;
	public bool ShowingLeaderboard;

	private void Awake()
	{
		_apiManager = GameJolt.API.Manager.Instance;
		_uiManager = GameJolt.UI.Manager.Instance;
	}

	private void Update()
	{
	}

	public void SignIn()
	{
		SigningIn = true;
		SignOut();
		_uiManager.ShowSignIn(OnSignIn);
	}

	public void SignOut()
	{
		if (User != null)
		{
			User.SignOut();
		}
	}

	public void ShowLeaderboard()
	{
		ShowingLeaderboard = true;
		_uiManager.ShowLeaderboards(OnShowLeaderboard);
	}

	public void AddScore(int value)
	{
		var text = User != null ? User.Name : "Guest";
		GameJolt.API.Scores.Add(value, text, 92417, "", OnAddScore);
	}

	private void OnSignIn(bool success)
	{
		Debug.LogFormat("The user {0} to sign in!", success ? "succeeded" : "failed");
		SigningIn = false;
	}

	private void OnShowLeaderboard(bool success)
	{
		ShowingLeaderboard = false;
	}

	private void OnAddScore(bool sucess)
	{

	}
}
