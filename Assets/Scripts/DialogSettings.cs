using UnityEngine;
using System.Collections;

public enum Person
{
    General,
    Private,
    Blokfosk,
}

[CreateAssetMenu]
public class DialogSettings : ScriptableObject
{
    [System.Serializable]
    public struct DialogEntry
    {
        public Person Person;
        [Multiline]
        public string Text;
        public float StartTime;
        public float ScrollForwardTime;
        public float LingerTime;
    }

    [System.Serializable]
    public struct Dialog
    {
        public string Name;
        public float TriggerTime;
        public DialogEntry[] Entries;
    }
    public Dialog[] Dialogs;
}
