using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Explodable : MonoBehaviour
{
	public GameObject[] Explosions;
    public SoundSourceType SoundSource;

	public AudioClip ExplodeClip;
	public EnemySpawner Spawner;

    float _LastAudioPlay;

	public void Explode (Transform t, bool shouldDestroy)
	{
		Explode (t.position, t.rotation, shouldDestroy);
	}

	public void Explode (Vector3 pos, bool shouldDestroy)
	{
		Explode (pos, Quaternion.identity, shouldDestroy);
	}

	public void Explode(Vector3 pos, Quaternion rotation, bool shouldDestroy)
	{
		if (ExplodeClip && _LastAudioPlay + 0.4f < Time.unscaledTime)
		{
            _LastAudioPlay = Time.unscaledTime;
			Music.PlayClipAtPoint(ExplodeClip, pos, Music.instance.sfxv, Random.Range(0.50f, 1.50f), SoundSource);
		}

		for (int i = 0; i < Explosions.Length; i++)
		{
			TrashMan.spawn(Explosions[i], pos, rotation);
		}

		if (!shouldDestroy)
		{
			return;
		}

		if (Spawner != null)
		{
			Spawner.Despawn(gameObject);
		}
		else
		{
			TrashMan.despawn(gameObject);
		}
	}
}
