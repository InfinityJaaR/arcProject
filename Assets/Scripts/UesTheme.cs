using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// FIA-UES brand colors from arcProjectWeb/docs/blueprint.md
/// </summary>
public static class UesTheme
{
    public static readonly Color Primary = new Color(0.117f, 0.227f, 0.541f, 1f);       // #1E3A8A
    public static readonly Color Background = new Color(0.941f, 0.957f, 0.973f, 1f); // #F0F4F8
    public static readonly Color Accent = new Color(0.902f, 0.467f, 0f, 1f);           // #E67700
    public static readonly Color TextOnPrimary = Color.white;
    public static readonly Color TextMuted = new Color(0.35f, 0.4f, 0.45f, 1f);

    public static void ApplyPanel(Image panelImage, bool isPrimary = true)
    {
        if (panelImage == null) return;
        panelImage.color = isPrimary ? Primary : Background;
    }

    public static void ApplyButton(Button button, bool isAccent = false)
    {
        if (button == null) return;
        var colors = button.colors;
        colors.normalColor = isAccent ? Accent : Primary;
        colors.highlightedColor = isAccent ? Accent * 1.1f : Primary * 1.1f;
        colors.pressedColor = isAccent ? Accent * 0.85f : Primary * 0.85f;
        button.colors = colors;
    }

    public static void ApplyTitle(TextMeshProUGUI text)
    {
        if (text == null) return;
        text.color = TextOnPrimary;
        text.fontStyle = FontStyles.Bold;
    }

    public static void ApplyBody(TextMeshProUGUI text)
    {
        if (text == null) return;
        text.color = TextMuted;
    }
}
