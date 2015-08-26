using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
	public Text ScoreCountText;
	public Text ScoreText;
	public float TimeUntilAddingScore = 0.8f;
	public float ScoreTickSpeed = 0.05f;

	public float BlinkSpeedIncrease = 1f;
	public AnimationCurve BlinkSpeedCurve;
	public bool BlinkWholeText = false;
	public float ColorGradientSpeed = 1f;
	public Gradient ColorGradient;

	private int _currentScore;
	private bool _addingScore;

	private float _timer;
	private int _scoreToAdd;
	private Color _blinkColor;
	private float _blinkSpeed;

	private float _blinkTime;

	private void Start()
	{
		GameLogic.Instance.OnScoreChanged += OnScoreChanged;
	}

	private void Update()
	{
		if (!_addingScore && _scoreToAdd > 0 && _timer + TimeUntilAddingScore < Time.unscaledTime)
		{
			StartCoroutine(AddScore(_scoreToAdd));
			StartCoroutine(TextBlink(_scoreToAdd));
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
		var t = 0f;
		_addingScore = true;

		while (scoreToAdd > 0)
		{
			t += Time.deltaTime * ColorGradientSpeed;
			_blinkColor = ColorGradient.Evaluate(t);
			_blinkSpeed = BlinkSpeedCurve.Evaluate(t);
			_currentScore += 1;
			scoreToAdd -= 1;
			ScoreCountText.text = _currentScore.ToString();
			yield return new WaitForSeconds(ScoreTickSpeed);
		}
		_addingScore = false;
	}

	private IEnumerator TextBlink(int scoreToAdd)
	{
		var scoreCountColor = ScoreCountText.color;
		var scoreTextColor = ScoreText.color; 
		var blink = true;

		while (_addingScore)
		{
			ScoreCountText.color = blink ? _blinkColor : scoreCountColor;
			ScoreText.color = blink ? _blinkColor : scoreTextColor;
			blink = !blink;
			yield return new WaitForSeconds(_blinkSpeed);
		}

		ScoreCountText.color = scoreCountColor;
		ScoreText.color = scoreTextColor;
	}
}
