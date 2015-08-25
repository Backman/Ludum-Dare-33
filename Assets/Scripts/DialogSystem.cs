using UnityEngine;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
    public DialogSettings Settings;

    class DialogState
    {
        public float StartTime;
        public DialogSettings.Dialog Dialog;
        public string[] LastDialogStrings;
    }
    Queue<DialogState> _DialogsQueued = new Queue<DialogState>();
    DialogState _CurrentDialog;
    public DialogBox BlokfoskBox;
    public DialogBox GeneralBox;
    public DialogBox PrivateBox;

    void Awake()
    {
        for (int i = 0; i < Settings.Dialogs.Length; i++)
        {
            var dialog = Settings.Dialogs[i];
            DialogState state = new DialogState();
            state.StartTime = dialog.TriggerTime;
            state.Dialog = dialog;
            state.LastDialogStrings = new string[dialog.Entries.Length];
            for (int j = 0; j < state.LastDialogStrings.Length; j++)
                state.LastDialogStrings[j] = string.Empty;
            _DialogsQueued.Enqueue(state);
        }
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


    void Update()
    {
        if (_DialogsQueued.Count == 0)
            return;
        float time = Time.timeSinceLevelLoad;
        var peek = _DialogsQueued.Peek();
        if (peek.StartTime < time)
        {
            _CurrentDialog = peek;
            _DialogsQueued.Dequeue();
        }
        SetPortraitState(Person.Blokfosk, string.Empty, 0f);
        SetPortraitState(Person.General, string.Empty, 0f);
        SetPortraitState(Person.Private, string.Empty, 0f);

        if (_CurrentDialog != null)
        {
            float dialogTime = time - _CurrentDialog.StartTime;
            for (int i = 0; i < _CurrentDialog.Dialog.Entries.Length; i++)
            {
                var entry = _CurrentDialog.Dialog.Entries[i];
                float entryDuration = entry.ScrollForwardTime + entry.LingerTime;
                if (dialogTime > entry.StartTime && dialogTime <= entry.StartTime + entryDuration)
                {
                    float entryTime = dialogTime - entry.StartTime;
                    float scrollTime = Mathf.Clamp01(entryTime / entry.ScrollForwardTime);
                    int length = Mathf.RoundToInt(entry.Text.Length * scrollTime);
                    string text;
                    if (_CurrentDialog.LastDialogStrings[i].Length != length)
                    {
                        text = _CurrentDialog.LastDialogStrings[i]  = entry.Text.Substring(0, length);
                    }
                    else
                    {
                        text = _CurrentDialog.LastDialogStrings[i];
                    }

                    float timeSinceLinger = (entry.LingerTime + entry.ScrollForwardTime)  - entryTime;
                    float a = Mathf.Clamp01(timeSinceLinger);
                    SetPortraitState(entry.Person, text, a);
                }
            }
        }
    }
}
