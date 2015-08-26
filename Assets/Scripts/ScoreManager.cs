using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
	private Queue<int> _scoreQueue = new Queue<int>();
	
	public Text ScoreText;
	public float TimeUntilAddingScore = 0.8f;
	public float ScoreTickSpeed = 0.05f;

	private int _currentScore;
	private bool _addingScore;

	private float _timer;
	private int _scoreToAdd;

	private void Start()
	{
		GameLogic.Instance.OnScoreChanged += OnScoreChanged;
	}

	private void Update()
	{
		if (!_addingScore && _scoreToAdd > 0 && _timer + TimeUntilAddingScore < Time.unscaledTime)
		{
			StartCoroutine(AddScore(_scoreToAdd));
			_scoreToAdd = 0;
		}
	}

	private void OnScoreChanged(int oldScore, int newScore)
	{
		_scoreToAdd += newScore - oldScore;
		_timer = Time.unscaledTime;
	}

	private IEnumerator AddScore(int scoreToAdd)
	{
		_addingScore = true;



		while (scoreToAdd > 0)
		{
			_currentScore += 1;
			scoreToAdd -= 1;
			ScoreText.text = _currentScore.ToString();
			yield return new WaitForSeconds(ScoreTickSpeed);
		}
		_addingScore = false;
	}
}
