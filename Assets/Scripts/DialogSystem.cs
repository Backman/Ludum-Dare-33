using UnityEngine;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
	public DialogSettings Settings;

	class DialogState
	{
		public float StartTime;
		public DialogSettings.DialogEntry[] DialogEntries;
		public string[] LastDialogStrings;
		public float StartedTime;
		public float Deadline;
	}

	List<DialogState> _DialogsQueued = new List<DialogState>();
	DialogState _CurrentDialog;
	public DialogBox BlokfoskBox;
	public DialogBox GeneralBox;
	public DialogBox PrivateBox;

	public static DialogSystem Instance;

	private float _time = -1f;

	void Awake()
	{
		Instance = this;
		for (int i = 0; i < Settings.Dialogs.Length; i++)
		{
			var dialog = Settings.Dialogs[i];
			AddDialog(dialog.Entries, dialog.TriggerTime, 0f);
		}
	}

	private void Start()
	{
		GameLogic.Instance.FirstEnemyRekt += (GameObject go) => { _time = 0f; };
	}

	void SetPortraitState(Person person, string text, float alpha)
	{
		DialogBox box = null;
		switch (person)
		{
			case Person.Blokfosk:
				box = BlokfoskBox;
				break;
			case Person.General:
				box = GeneralBox;
				break;
			case Person.Private:
				box = PrivateBox;
				break;

		}
		box.Text.text = text;
		for (int i = 0; i < box.Graphics.Length; i++)
		{
			var graphics = box.Graphics[i];
			var col = graphics.color;
			col.a = alpha;
			graphics.color = col;
		}
	}

	public void AddDialog(DialogSettings.DialogEntry[] entries, float startTime, float deadlineDuration)
	{
		DialogState state = new DialogState();
		state.StartTime = Mathf.Max(_time, startTime);
		state.DialogEntries = entries;
		state.Deadline = deadlineDuration == 0f ? 0f : state.StartTime + deadlineDuration;
		state.LastDialogStrings = new string[entries.Length];
		for (int j = 0; j < state.LastDialogStrings.Length; j++)
			state.LastDialogStrings[j] = string.Empty;
		int pos = 0;
		while (_DialogsQueued.Count > pos && _DialogsQueued[pos].StartTime < startTime)
			pos++;

		_DialogsQueued.Insert(pos, state);
	}


	void Update()
	{
		SetPortraitState(Person.Blokfosk, string.Empty, 0f);
		SetPortraitState(Person.General, string.Empty, 0f);
		SetPortraitState(Person.Private, string.Empty, 0f);
		
		if (_time < 0f)
		{
			return;
		}

		_time += Time.deltaTime;
		if (_CurrentDialog == null && _DialogsQueued.Count > 0)
		{
			var peek = _DialogsQueued[0];
			if (peek.Deadline != 0f && peek.Deadline < _time)
			{
				_DialogsQueued.RemoveAt(0);
			}
			else if (peek.StartTime < _time)
			{
				_CurrentDialog = peek;
				_CurrentDialog.StartTime = _time;
				_DialogsQueued.RemoveAt(0);
			}
		}

		if (_CurrentDialog != null)
		{
			float dialogTime = _time - _CurrentDialog.StartTime;
			float dialogDuration = 0f;
			for (int i = 0; i < _CurrentDialog.DialogEntries.Length; i++)
			{
				var entry = _CurrentDialog.DialogEntries[i];
				float entryDuration = entry.ScrollForwardTime + entry.LingerTime;
				dialogDuration = Mathf.Max(dialogDuration, entryDuration + entry.StartTime);
				if (dialogTime > entry.StartTime && dialogTime <= entry.StartTime + entryDuration)
				{
					float entryTime = dialogTime - entry.StartTime;
					float scrollTime = Mathf.Clamp01(entryTime / entry.ScrollForwardTime);
					int length = Mathf.RoundToInt(entry.Text.Length * scrollTime);
					string text;
					if (_CurrentDialog.LastDialogStrings[i].Length != length)
					{
						text = _CurrentDialog.LastDialogStrings[i] = entry.Text.Substring(0, length);
					}
					else
					{
						text = _CurrentDialog.LastDialogStrings[i];
					}

					float timeSinceLinger = (entry.LingerTime + entry.ScrollForwardTime) - entryTime;
					float a = Mathf.Clamp01(timeSinceLinger * 2.0f);
					SetPortraitState(entry.Person, text, a);
				}
			}
			if (dialogTime > dialogDuration)
			{
				_CurrentDialog = null;
			}
		}
	}
}
