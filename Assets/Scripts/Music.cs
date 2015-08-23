using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	public Blokfosk Player;
	private GameObject MusicPlayer;
	private AudioSource[] Songs;
	public float transition= 0;
	[Range(0.0f, 1.0f)]
	public float Musicvolume = 0.33f;
	public float fadespeed = 0.025f;

	// Use this for initialization
	void Start () {
		Player = FindObjectOfType<Blokfosk> ();
		MusicPlayer = GameObject.Find ("MusicPlayer");
		Songs = GetComponents<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		Songs [1].volume = transition * Musicvolume;
		Songs [0].volume =(1- transition) * Musicvolume;


		if (Player.Hype.IsHyping) {
			//Songs[0].volume = (1 - Player.Hype.NormalizedHype) * Musicvolume;
			//Songs[1].volume = Player.Hype.NormalizedHype * Musicvolume;
			transition += fadespeed *Time.deltaTime;
	
		}
		if (!Player.Hype.IsHyping) {

			transition -= 10*fadespeed *Time.deltaTime;
			//Songs[0].volume = Musicvolume;
			//Songs[1].volume = 0;
		}
		transition = Mathf.Clamp01(transition);
	}
}
