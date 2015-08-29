using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance;
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();

				if (_instance == null)
				{
					_instance = new GameObject("Singleton<" + typeof(T).Name + ">").AddComponent<T>();
					//Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
				}
			}

			return _instance;
		}
	}

	private void Awake()
	{
		if (ShouldPersist())
		{
			Initialize();
		}
	}

	private bool ShouldPersist()
	{
		if (_instance == null)
		{
			_instance = this as T;
		}
		else if (_instance != this)
		{
			Destroy(gameObject);
			return false;
		}

		DontDestroyOnLoad(gameObject);

		return true;
	}

	protected virtual void Initialize() { }
}
