using UnityEngine;

public class SubtitleStyle
{
    public Color OutlineColour;
    public Color NameColour;
    public Color TextColour;
    public Font fontSyle;
    public int SubtitleSize;
    public float OutLineSize;

    public SubtitleStyle()
    {
    }

    /// <summary>
    /// struct for storing all subtitle style data.
    /// </summary>
    /// <param name="outline">Subtutle outline colour</param>
    /// <param name="name">Character Speaking</param>
    /// <param name="text">Character Dialog</param>
    /// <param name="subtitleSize">Text font size</param>
    /// <param name="outLineSize">Size of the text outline</param>
    public SubtitleStyle(Color outline, Color name, Color text, Font font, int subtitleSize, float outLineSize)
    {
        OutlineColour = outline;
        fontSyle = font;
        NameColour = name;
        TextColour = text;
        SubtitleSize = subtitleSize;
        OutLineSize = outLineSize;
    }
}
