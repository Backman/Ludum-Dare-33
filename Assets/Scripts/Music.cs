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
	[Range(0.0f, 1.0f)]
	public float sfxv = 1f;
	public static Music instance;
	// Use this for initialization
	void Start () {
		instance = this;
		Player = FindObjectOfType<Blokfosk> ();
		MusicPlayer = GameObject.Find ("MusicPlayer");
		Songs = GetComponents<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		Songs [1].volume = transition * Musicvolume;
		Songs [0].volume =(1- transition) * Musicvolume;
		Songs [2].volume = transition * Musicvolume;



		if (Player.Hype.IsHyping) {
			if(!Songs[2].isPlaying){
			Songs[2].Play();
			}
			//Songs[0].volume = (1 - Player.Hype.NormalizedHype) * Musicvolume;
			//Songs[1].volume = Player.Hype.NormalizedHype * Musicvolume;
			transition += fadespeed *Time.deltaTime;
	
		}
		if (!Player.Hype.IsHyping) {
			if(Songs[2].isPlaying && Songs[2].volume == 0f){
				Songs[2].Stop();
			}
		
			transition -= 10*fadespeed *Time.deltaTime;
			//Songs[0].volume = Musicvolume;
			//Songs[1].volume = 0;
		}
		transition = Mathf.Clamp01(transition);
	}
}
