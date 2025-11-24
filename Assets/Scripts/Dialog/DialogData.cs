using System;
using UnityEngine;

[Serializable]
public class DialogData
{
    public string npcName;
    public string npcPortrait;
    public float typingSpeed = 0.05f;
    public DialogEntry[] dialog;
}

[Serializable]
public class DialogEntry
{
    public string text;
    public string playerChoice;
    public string[] npcReply;
    public string[] choices;
    public bool endAfter;
}
