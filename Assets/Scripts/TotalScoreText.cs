using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class TotalScoreText : MonoBehaviour
{
	public void Awake()
	{
		var text = GetComponent<Text>();
		var score = PlayerPrefs.GetInt("TOTAL_SCORE");

		var numberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
		numberFormat.NumberGroupSeparator = " ";

		var scoreText = "Total Score: " + score.ToString("#,#", numberFormat);
		text.text = scoreText;
	}
}
