using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
	public Image Bar;
	public Image Blink;

	private float _originWidth;
	private int _maxHealth;

	private void Awake ()
	{
		_originWidth = Bar.rectTransform.rect.width;
	}

	private void Start ()
	{
		_maxHealth = Blokfosk.Instance.MaxHealth;
		GameLogic.Instance.OnBlokDamage += OnBlokDamage;
	}

	private void OnBlokDamage (int currentHealth)
	{
		var healthPercentage = currentHealth / _maxHealth;

		_originWidth *= healthPercentage;

	}
}
