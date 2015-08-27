using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
	private const string ScoreString = "Score: ";
	public Text ScoreText;
	public Text ScoreTextDropShadow;
	public float TimeUntilAddingScore = 0.8f;
	public float ScoreTickSpeed = 0.05f;

	public AnimationCurve BlinkSpeedCurve;
	public Gradient ColorGradient;
	public float TextIntensityBuildUpSpeed = 1f;

	public AnimationCurve XShakeCurve;
	public AnimationCurve YShakeCurve;

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

		var scoreTextPos = ScoreText.transform.position;
		var dropShadowPos = ScoreTextDropShadow.transform.position;

		var scoreTextOriginPos = scoreTextPos;
		var dropShadowOriginPos = dropShadowPos;

		while (scoreToAdd > 0)
		{
			t += Time.deltaTime * TextIntensityBuildUpSpeed;
			_blinkColor = ColorGradient.Evaluate(t);
			_blinkSpeed = BlinkSpeedCurve.Evaluate(t);

			scoreTextPos.x += XShakeCurve.Evaluate(t);
			scoreTextPos.y += YShakeCurve.Evaluate(t);

			dropShadowPos.x += XShakeCurve.Evaluate(t);
			dropShadowPos.y += YShakeCurve.Evaluate(t);

			ScoreText.transform.position = scoreTextPos;
			ScoreTextDropShadow.transform.position = dropShadowPos;

			_currentScore += 1;
			scoreToAdd -= 1;
			var text = _currentScore.ToString();
			ScoreText.text = text;
			ScoreTextDropShadow.text = text;
			yield return new WaitForSeconds(ScoreTickSpeed);

			ScoreText.transform.position = scoreTextOriginPos;
			ScoreTextDropShadow.transform.position = dropShadowOriginPos;
		}
		_addingScore = false;
	}

	private IEnumerator TextBlink(int scoreToAdd)
	{
		var scoreTextColor = ScoreText.color;
		var dropShadowColor = ScoreTextDropShadow.color;
		var dropShadowAlpha = dropShadowColor.a;
		var blink = true;

		while (_addingScore)
		{
			var col = blink ? _blinkColor : dropShadowColor;
			col.a = dropShadowAlpha;
			ScoreTextDropShadow.color = col;
			ScoreText.color = blink ? _blinkColor : scoreTextColor;
			blink = !blink;
			yield return new WaitForSeconds(_blinkSpeed);
		}

		ScoreText.color = scoreTextColor;
	}
}
