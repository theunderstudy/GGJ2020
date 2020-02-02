using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subtitle_Manager : Singleton<Subtitle_Manager>
{
    private UI_Subtitle_Controller Subtitle_Controller { get { return FindObjectOfType<UI_Subtitle_Controller>(); } }

    public Color speakerNameColor = new Color(225, 225, 225, 1f), dialougColour = new Color(225, 225, 225, 1f), outlineColour = new Color(0, 0, 0, 1f);
    public int size = 30;

    public float textOutlineSize = 1f;
    public Font font;

    public List<string> Test = new List<string>();

    public delegate void ShowSubs(Color speakerColour, string name, string dialouge);
    public static event ShowSubs DisplaySubtitle;

    private List<Tuple<Color, string, string, int>> dialogueQueue = new List<Tuple<Color, string, string, int>>();

    protected override void Awake()
    {
        SetStyle();
    }

    //Call this to set stubtitle styles.
    public void SetStyle()
    {
        Subtitle_Controller.SubtitleStyle.NameColour = speakerNameColor;
        Subtitle_Controller.SubtitleStyle.TextColour = dialougColour;
        Subtitle_Controller.SubtitleStyle.OutlineColour = outlineColour;
        Subtitle_Controller.SubtitleStyle.SubtitleSize = size;
        Subtitle_Controller.SubtitleStyle.fontSyle = font;
        Subtitle_Controller.SubtitleStyle.OutLineSize = textOutlineSize;
        Subtitle_Controller.SetSubtitleStyle();
    }

    /// <summary>
    /// Used to send any game dialouge to the subtitle system
    /// </summary>
    /// <param name="speakerColor">The text colour of the speaker</param>
    /// <param name="name">The source of the dialouge</param>
    /// <param name="dialouge">The Dalouge</param>
    public void SendDialouge(Color speakerColor, string name, string dialouge, int displaySeconds = 3)
    {
        this.dialogueQueue.Add(Tuple.Create(speakerColor, name, dialouge, displaySeconds));
        if (dialogueQueue.Count == 1) StartCoroutine(ProcessQueuedDialouge());
    }

    IEnumerator ProcessQueuedDialouge()
    {
        while (this.dialogueQueue.Count > 0) {
            Debug.Log("ProcessQueuedDialouge");
            Tuple<Color, string, string, int> nextDialogue = this.dialogueQueue[0];
            DisplaySubtitle(nextDialogue.Item1, nextDialogue.Item2, nextDialogue.Item3);
            this.dialogueQueue.RemoveAt(0);
            yield return new WaitForSecondsRealtime((float) nextDialogue.Item4);
        }
    }

}
