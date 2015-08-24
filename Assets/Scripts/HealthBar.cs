using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
	public Image Bar;
	public Image Blink;
	public float BlinkDuration = 0.5f;

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
		StartCoroutine (DoBlink ());
	}

	private IEnumerator DoBlink ()
	{
		var t = 0f;
		var color = Blink.color;
		color.a = 1f;
		Blink.color = color;

		yield return new WaitForSeconds (BlinkDuration);

		color.a = 0f;
		Blink.color = color;
	}
}
