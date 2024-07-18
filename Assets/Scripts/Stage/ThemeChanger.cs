using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeChanger : MonoBehaviour
{
    public SpriteRenderer background;
    public Sprite[] spriteBackground = new Sprite[3];
    public SpriteRenderer polaroid;
    public Sprite[] spritePolaroid = new Sprite[3];

    private string theme;

    private void Start()
    {
        theme = SceneTheme.theme;
        string[] themeForIndex = { "carousel", "ferris_wheel", "roller_coaster" };
        for (int i = 0; i < themeForIndex.Length; i++)
        {
            if (theme == themeForIndex[i])
            {
                background.sprite = spriteBackground[i];
                polaroid.sprite = spritePolaroid[i];
            }
        }
    }
}
