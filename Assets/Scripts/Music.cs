using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	public Blokfosk Player;
	private GameObject MusicPlayer;
	private AudioSource[] Songs;
	[Range(0.0f, 1.0f)]
	public float Musicvolume = 0.33f;

	// Use this for initialization
	void Start () {
		Player = FindObjectOfType<Blokfosk> ();
		MusicPlayer = GameObject.Find ("MusicPlayer");
		Songs = GetComponents<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Player.Hype.IsHyping) {
			Songs[0].volume = (1 - Player.Hype.NormalizedHype) * Musicvolume;
			Songs[1].volume = Player.Hype.NormalizedHype * Musicvolume;
		}
		if (!Player.Hype.IsHyping) {
			Songs[0].volume = Musicvolume;
			Songs[1].volume = 0;
		}
	}
}
