using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour
{
	public string DeathScreen;

	private void Awake ()
	{
		GameLogic.Instance.OnBlokDamage += HandleDamage;
	}

	private void HandleDamage (int currentHealth)
	{
		if (currentHealth <= 0) {
			
		}
	}
}
