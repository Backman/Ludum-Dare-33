using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

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

	public System.Action<int, int> OnScoreChanged;
	public System.Action<int> OnBlokDamage;
	public System.Action<GameObject, int> OnRekComboIncreased;
	public System.Action<int> OnRekComboReset;
	public System.Action<GameObject> OnRekFace;
	public System.Action<GameObject> FirstEnemyRekt;
	public System.Action<GameObject> OnEnemyRekt;
	public System.Action OnBlokfoskRIP;

	public bool IsRekking { get; private set; }
	
	private int _score;
	private bool _firstEnemyKilled;

	public int Score {
		get { return _score; }
	}

	private void Awake ()
	{
		OnEnemyRekt += EnemyRekt;
	}

	private void Update ()
	{

	}

	private void OnLevelWasLoaded ()
	{
		if (Application.loadedLevelName == "Uncensored")
		{
			IsRekking = true;
		}
	}

	private void EnemyRekt (GameObject rektObjekt)
	{
		if (!_firstEnemyKilled)
		{
			Music.instance.PlayRegularMusic();
			_firstEnemyKilled = true;
			if (FirstEnemyRekt != null)
			{
				FirstEnemyRekt.Invoke(rektObjekt);
			}
		}
	}

	public void SetScore (int newScore)
	{
		if (OnScoreChanged != null) {
			OnScoreChanged.Invoke (_score, newScore);
		}

		_score = newScore;
	}

	public void AddScore (int value)
	{
		var newValue = _score + value;
		SetScore (newValue);
	}
}
