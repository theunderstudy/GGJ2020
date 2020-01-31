using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SubtitleTest : MonoBehaviour
{
    public Color speakerName, dialog, outline;
    public int size = 30;
    public UI_Subtitle_Controller _Subtitle_Controller;
    public float textOutlineSize;
    public Font font;

    private void Awake()
    {
        _Subtitle_Controller = FindObjectOfType<UI_Subtitle_Controller>();
        TestStyle();
    }

    void TestStyle()
    {
        _Subtitle_Controller.SubtitleStyle.fontSyle = font;
        _Subtitle_Controller.SubtitleStyle.NameColour = speakerName;
        _Subtitle_Controller.SubtitleStyle.TextColour = dialog;
        _Subtitle_Controller.SubtitleStyle.OutlineColour = outline;
        _Subtitle_Controller.SubtitleStyle.SubtitleSize = size;
        _Subtitle_Controller.SubtitleStyle.OutLineSize = textOutlineSize;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            TestStyle();
            _Subtitle_Controller.SetSubtitleStyle();
        }
    }
}
