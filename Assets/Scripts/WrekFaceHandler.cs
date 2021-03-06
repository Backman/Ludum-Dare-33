using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WrekFaceHandler : MonoBehaviour
{
	public float RekDuration = 0.08f;
	public float RekZoomDuration = 1.5f;
	[Range(0.0f, 1.0f)]
	public float RekFreezeValue = 0.0f;
	[Range(0.0f, 1.0f)]
	public float RekZoomFreezeValue = 0.0f;
	public Color BlinkColor;
	public float BlinkDuration;
	public float BlokfoskBlinkDuration;
	public AnimationCurve BlinkCurve;
	public float MaxDistance = 10f;
	public float MinDistance = 5f;
	public float ZoomMinDistance = 12f;
	public float ZoomLerpModifier = 20f;

	[System.Serializable]
	public struct ComboTextData
	{
		public int MinCombo;
		public int MaxCombo;
		public GameObject Prefab;
	}

    public DialogSettings.DialogEntry[] ComboDialog;
	public ComboTextData[] ComboData;
	public float ComboTextYOffset;
	public float ComboTextCircleSize;
	bool _HasFreezeTime = false;
	float _LastComboText;

	public RekCombo RekCombo;

	void Awake()
	{
		GameLogic.Instance.OnRekFace += OnRekFace;
		GameLogic.Instance.OnBlokDamage += OnBlokDamage;
		GameLogic.Instance.OnRekComboIncreased += OnRekComboIncreased;

		RekCombo.Init();

	}

	void OnRekComboIncreased(GameObject obj, int combo)
	{
		if (_LastComboText + 0.5f >= Time.unscaledTime)
			return;
		if (Random.Range(0f, 1f) > 0.7f)
			return;
		List<ComboTextData> valid = new List<ComboTextData>();
		for (int i = 0; i < ComboData.Length; i++)
		{
			if (combo >= ComboData[i].MinCombo && combo <= ComboData[i].MaxCombo)
			{
				valid.Add(ComboData[i]);
			}
		}
        
		if (valid.Count > 0)
		{
			ComboTextData comboData = valid[Random.Range(0, valid.Count)];
			Vector2 unitCircle = Random.insideUnitCircle * ComboTextCircleSize;
			Vector2 pos = (Vector2)obj.transform.position + unitCircle + new Vector2(0, ComboTextYOffset);

            DialogSystem.Instance.AddDialog(ComboDialog, 0f, 0.15f);
			TrashMan.spawn(comboData.Prefab, new Vector3(pos.x, pos.y, 30));
			GameLogic.Instance.AddScore(RekCombo.ComboScore);
			_LastComboText = Time.unscaledTime;
		}
	}

	void OnBlokDamage(int dmg)
	{
		BlinkManager.Instance.AddBlink(Blokfosk.Instance.gameObject, BlinkColor, BlokfoskBlinkDuration);
	}

	void OnRekFace(GameObject obj)
	{
		if (obj.activeInHierarchy == false)
			return;
		var distance = Vector2.Distance(obj.transform.position, Blokfosk.Instance.transform.position);

        bool zooming = distance > ZoomMinDistance;
		var rekDuration = RekDuration;
		var freezeValue = zooming ? RekZoomFreezeValue : RekFreezeValue;

        if(zooming)
		{
            float distanceMod = Mathf.Clamp01((distance - MinDistance) / MaxDistance);
            freezeValue += distanceMod * 0.3f;
            rekDuration *= 1 + distanceMod * 0.5f;
        }

		RekCombo.IncreaseRekComboCount(obj);

		BlinkManager.Instance.AddBlink(obj, BlinkColor, BlinkDuration, BlinkCurve);
		if (_HasFreezeTime == false)
		{
			StartCoroutine(FreezeTime(obj, rekDuration, freezeValue, distance));
			_HasFreezeTime = true;
		}
	}

	IEnumerator FreezeTime(GameObject obj, float rekDuration, float freezeValue, float distance)
	{
		float startTime = Time.unscaledTime;
		Time.timeScale = freezeValue;
		bool zooming = false;
		while (startTime + rekDuration > Time.unscaledTime)
		{
			if (!zooming && obj.activeInHierarchy == false)
				break;
			if (distance > ZoomMinDistance)
			{
				zooming = true;
				var followCam = Camera.main.GetComponent<FollowCamera>();
				Vector3 pos = obj.transform.position;
				followCam.SetTarget(pos, Vector2.zero, 0, 1, ZoomLerpModifier);
			}
			yield return null;
		}
		Time.timeScale = 1f;
		_HasFreezeTime = false;
	}

	public void Update()
	{
		RekCombo.Update();
	}
}
