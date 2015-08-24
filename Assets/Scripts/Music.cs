using UnityEngine;
using System.Collections;

public enum SoundSourceType
{
    Unknown ,
    Projectile ,
    WaterSplash,
    Bomb,
    Sub,
    Boat,
    Air,
    InkBoost,
}

public class Music : MonoBehaviour
{

	public Blokfosk Player;
	//private GameObject MusicPlayer;
	private AudioSource[] Songs;
	public float transition = 0;
	[Range (0.0f, 1.0f)]
	public float Musicvolume = 0.33f;
	public float fadespeed = 0.025f;
	[Range (0.0f, 1.0f)]
	public float sfxv = 1f;
	public static Music instance;
	public float PlayClipCooldown = 0.2f;

	private float _currentCooldownCount;

	private static bool CanPlayClipAtPoint
	{
		get { return instance._currentCooldownCount < instance.PlayClipCooldown; }
	}

	// Use this for initialization
	void Start ()
	{
		instance = this;
		Player = FindObjectOfType<Blokfosk> ();
		//MusicPlayer = GameObject.Find ("MusicPlayer");
		Songs = GetComponents<AudioSource> ();

	}

	public static AudioSource PlayClipAtPoint (AudioClip clip, Vector3 pos, float volume, float pitch, SoundSourceType source)
	{
		if (!CanPlayClipAtPoint)
		{
			return null;
		}

		instance._currentCooldownCount = 0f;
		var tempGO = new GameObject ("TempAudio"); // create the temp object
		tempGO.transform.position = pos; // set its position
		var aSource = tempGO.AddComponent<AudioSource> (); // add an audio source
		aSource.clip = clip; // define the clip
		aSource.volume = volume;
		aSource.spatialBlend = 1.0f;
		aSource.minDistance = 5f;
		aSource.maxDistance = 100f;
		aSource.pitch = pitch;
		// set other aSource properties here, if desired
		aSource.Play (); // start the sound
		Destroy (tempGO, clip.length); // destroy object after clip duration
		return aSource; // return the AudioSource reference
	}
	
	// Update is called once per frame
	void Update ()
	{
		_currentCooldownCount -= Time.deltaTime;

		Songs [1].volume = transition * Musicvolume;
		Songs [0].volume = (1 - transition) * Musicvolume;
		Songs [2].volume = transition * Musicvolume;


		float threshold = Player.VelocitySettings.BobThreshold;

		if (Player.transform.position.y < threshold - 0.75f) {
			if (!Songs [2].isPlaying) {
				Songs [2].Play ();
			}
			//Songs[0].volume = (1 - Player.Hype.NormalizedHype) * Musicvolume;
			//Songs[1].volume = Player.Hype.NormalizedHype * Musicvolume;
			transition += fadespeed * Time.deltaTime;
	
		}
		if (Player.transform.position.y > threshold - 0.75f) {
			if (Songs [2].isPlaying && Songs [2].volume == 0f) {
				Songs [2].Stop ();
			}
		
			transition -= 10 * fadespeed * Time.deltaTime;
			//Songs[0].volume = Musicvolume;
			//Songs[1].volume = 0;
		}
		transition = Mathf.Clamp01 (transition);
	}
}
