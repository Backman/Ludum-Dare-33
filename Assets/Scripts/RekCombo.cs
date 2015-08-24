using UnityEngine;
using System.Collections;

[System.Serializable]
public class RekCombo
{
	public float ComboCooldown = 0.5f;

	private float _startTime;
	private int _currentComboCount;

	public bool InComboMode {
		get {
			return _startTime + ComboCooldown < Time.unscaledTime;
		}
	}

	public int CurrentCombo { get { return _currentComboCount; } }

	private GameLogic _gameLogic;

	public RekCombo ()
	{
		_startTime = Time.unscaledTime;
		_gameLogic = GameLogic.Instance;
	}

	public void IncreaseRekComboCount (GameObject rektObject)
	{
		_currentComboCount++;
		_startTime = Time.unscaledTime;

		if (_gameLogic.OnRekComboIncreased != null) {
			_gameLogic.OnRekComboIncreased.Invoke (rektObject, _currentComboCount);
		}
	}

	public void ResetRekComboCount ()
	{
		if (_gameLogic.OnRekComboReset != null) {
			_gameLogic.OnRekComboReset.Invoke (_currentComboCount);
		}

		_currentComboCount = 0;
	}

	public void Update ()
	{
		if (_currentComboCount > 0 && !InComboMode) {
			ResetRekComboCount ();
		}
	}
}
