using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameLogic : MonoBehaviour
{
	private static GameLogic _instance;

	public static GameLogic Instance {
		get {
			if (!_instance) {
				var instances = FindObjectsOfType<GameLogic> ();
				if (instances.Length > 1) {
					for (int i = 1; i < instances.Length; i++) {
						Destroy (instances [i].gameObject);
					}
				}

				GameLogic instance;
				if (instances.Length <= 0) {
					instance = new GameObject ("Game Logic").AddComponent<GameLogic> ();
					_instance = instance;
				} else {
					instance = instances [0];
				}

				_instance = instance;
			}

			return _instance;
		}
	}

	public System.Action OnScoreChanged;
	public System.Action<int> OnBlokDamage;
	public System.Action<GameObject, int> OnRekComboIncreased;
	public System.Action<int> OnRekComboReset;
	public System.Action<GameObject> OnRekFace;
	public System.Action OnEnemyKilled;
	public System.Action OnBlokfoskRIP;

	private int _score;
	private Blokfosk _player;
	private bool _firstEnemyKilled;

	public int Score {
		get { return _score; }
	}

	private void Awake ()
	{
		OnEnemyKilled += EnemyKilled;
	}

	private void Update ()
	{

	}

	private void OnLevelWasLoaded ()
	{

	}

	private void EnemyKilled ()
	{
		if (!_firstEnemyKilled) {
			Music.instance.PlayRegularMusic ();
			_firstEnemyKilled = true;
		}
	}

	public void SetScore (int newScore)
	{
		if (_score == newScore) {
			return;
		}

		_score = newScore;
		if (OnScoreChanged != null) {
			OnScoreChanged.Invoke ();
		}
	}

	public void AddScore (int value)
	{
		var newValue = _score + value;
		SetScore (newValue);
	}
}
