using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ThemeInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI themeInfoText;

    public void DisplayThemeInfo(List<score.ThemeResult> themeResults)
    {
        string displayText = "Theme-specific information:\n\n";

        foreach (var themeResult in themeResults)
        {
            displayText += $"Theme: {themeResult.theme}\n";
            displayText += $"play count: {themeResult.totalPlayRecord}\n";
            displayText += $"best record: {themeResult.totalBestRecord:F2}\n\n";
        }

        themeInfoText.text = displayText;
    }
}