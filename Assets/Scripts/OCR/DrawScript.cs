using UnityEngine;
using UnityEngine.UI;

public class DrawScript : MonoBehaviour
{
    public RawImage drawArea;
    private Texture2D drawTexture;
    private bool isDrawing = false;
    private bool canDrawing = false;
    private Vector2 previousPos;

    void Start()
    {
        InitializeTexture();
    }

    public void CanDrawing()
    {
        canDrawing = true;
    }
    public void CannotDrawing()
    {
        canDrawing = false;
    }

    public void InitializeTexture()
    {
        int width = (int)drawArea.rectTransform.rect.width;
        int height = (int)drawArea.rectTransform.rect.height;

        drawTexture = new Texture2D(width, height);
        drawTexture.filterMode = FilterMode.Point;
        // Fill the texture with a white background
        Color32[] colors = new Color32[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            //colors[i] = new Color(242/255f, 247/255f, 252/255f, 1f);
            colors[i].r = 242;
            colors[i].g = 247;
            colors[i].b = 252;
            if (i % 600 >= 300 || i % 600 < 900)
                colors[i].a = 1;
            else
                colors[i].a = 0;
        }

        drawTexture.SetPixels32(colors);
        drawTexture.Apply();

        drawArea.texture = drawTexture;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = GetMousePosition();
            if (IsWithinDrawArea(pos))
            {
                previousPos = pos;
                isDrawing = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }

        if (isDrawing && canDrawing)
        {
            Vector2 currentPos = GetMousePosition();
            if (IsWithinDrawArea(currentPos))
            {
                DrawLine(previousPos, currentPos);
                previousPos = currentPos;
                drawTexture.Apply();
            }
        }
    }

    Vector2 GetMousePosition()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawArea.rectTransform, Input.mousePosition, null, out localPoint);
        Vector2 pivotAdjustedPoint = new Vector2(
            localPoint.x + drawArea.rectTransform.rect.width * 0.5f,
            localPoint.y + drawArea.rectTransform.rect.height * 0.5f
        );
        return pivotAdjustedPoint;
    }

    bool IsWithinDrawArea(Vector2 pos)
    {
        return pos.x >= 300f && pos.x < drawArea.rectTransform.rect.width - 300f && pos.y >= 100f && pos.y < drawArea.rectTransform.rect.height - 100f;
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            for (int i = x0 - 2; i <= x0 + 2; i++)
            {
                for (int j = y0 - 2; j <= y0 + 2; j++)
                {
                    drawTexture.SetPixel(i, j, Color.black);
                }
            }
            if (x0 == x1 && y0 == y1) break;
            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}
