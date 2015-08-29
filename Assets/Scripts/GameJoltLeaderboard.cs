using UnityEngine;
using System.Collections;

public class GameJoltLeaderboard : MonoBehaviour
{
	private GameJolt.API.Objects.Score _score;

	private void Start()
	{
		Invoke("AddScore", 2f);
	}

	private void Update()
	{
		if (!GameJoltPipe.Instance.ShowingLeaderboard && Input.GetKeyDown(KeyCode.Escape))
		{
			GameJoltPipe.Instance.ShowLeaderboard();
		}
	}

	private void AddScore()
	{
		GameJoltPipe.Instance.AddScore(PlayerPrefs.GetInt("TOTAL_SCORE"));
	}
}
