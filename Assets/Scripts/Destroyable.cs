using UnityEngine;
using System.Collections;

public class Destroyable : MonoBehaviour
{
	public void Destroy ()
	{
		Destroy (gameObject);
	}
}
