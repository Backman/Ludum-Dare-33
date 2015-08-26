using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TotalScoreText : MonoBehaviour
{
	public void Awake()
	{
		var text = GetComponent<Text>();
		var score = PlayerPrefs.GetInt("TOTAL_SCORE");

		text.text += score.ToString();
	}
}
