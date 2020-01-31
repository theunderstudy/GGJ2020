using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text)), RequireComponent(typeof(Outline))]
public class UI_Subtitle_Controller : MonoBehaviour
{
    //Subtitle Preview
    [Header("Subtitle Preview")]
    [TextArea(5, 30)]
    public string subtitle;
    public bool debugActive;

    //Subtitle Display Properties
    public SubtitleStyle SubtitleStyle = new SubtitleStyle();
    private string namecolHex, dialogcolHex;

    //Time out settings
    public float textDisplayTimeOut = 5f;
    [SerializeField] private float currentDisplayTime;

    //Component Refrences
    private Text uiText;
    private Outline textOutline;

    private void Awake()
    {
        textOutline = GetComponentInChildren<Outline>();
        uiText = transform.GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        SetSubtitleStyle();
        UITextClear();
    }

    //Sets the Subtitle Style.
    public void SetSubtitleStyle()
    {
        namecolHex = ColorUtility.ToHtmlStringRGB(SubtitleStyle.NameColour);
        uiText.font = SubtitleStyle.fontSyle;
        dialogcolHex = ColorUtility.ToHtmlStringRGB(SubtitleStyle.TextColour);
        textOutline.effectColor = SubtitleStyle.OutlineColour;
        textOutline.effectDistance = new Vector2(SubtitleStyle.OutLineSize, -SubtitleStyle.OutLineSize);
        uiText.fontSize = SubtitleStyle.SubtitleSize;
        if (debugActive) //Debug display of subtitle settings
        {
            Debug.LogFormat("Name Colour is {0} " +
                            "Dialog Colour is: {1} " +
                            "Outline Colour is: {2} " +
                            "Outline Size is: {3} " +
                            "Font Size is: {4} ",
                            namecolHex.ToString(),
                            dialogcolHex.ToString(),
                            textOutline.effectColor.ToString(),
                            textOutline.effectDistance.ToString(),
                            uiText.fontSize.ToString());
        }
    }

    //Clears the text fields
    private void UITextClear()
    {
        uiText.text = "";
        subtitle = "";
    }

    /// <summary>
    /// Takes two passed in strings and formats them for display in the game.
    /// </summary>
    /// <param name="name">Name of the Speaking Character</param>
    /// <param name="dialog">Dialog of the speaking Character</param>
    public void UpdateText(string name, string dialog)
    {
        subtitle = string.Format("<color=#{0}><b>{1}</b></color>: <color=#{2}>{3}</color>",
                                    namecolHex,
                                    name,
                                    dialogcolHex,
                                    dialog);
        uiText.text = subtitle;
        currentDisplayTime = 0;
    }

    //Returns true if text is empty
    private bool ActiveText()
    {
        if (subtitle == "")
        {
            return false;
        }
        return true;
    }

    ///Times out the text if no new text is passed in.
    private void TextTimeOut()
    {
        if (ActiveText() == true)
        {
            currentDisplayTime += Time.deltaTime;
            if (currentDisplayTime > textDisplayTimeOut)
            {
                currentDisplayTime = textDisplayTimeOut;
                UITextClear();
                currentDisplayTime = 0;
            }
        }
    }

    private void Update()
    {
        TextTimeOut();
    }
}

