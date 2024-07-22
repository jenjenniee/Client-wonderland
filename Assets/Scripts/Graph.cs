using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Graph : MonoBehaviour
{
    [SerializeField] private float pointSize = 10f;
    [SerializeField] private float yPadding = 0.1f;
    [SerializeField] private TextMeshProUGUI[] dateLabels; // X축 날짜 레이블을 위한 Text 컴포넌트 배열

    public void DrawGraphs(List<float> averageList, List<float> bestList, RectTransform graphContainer)
    {
        ClearGraph(graphContainer);

        float graphWidth = graphContainer.rect.width;
        float graphHeight = graphContainer.rect.height;

        float minValue = Mathf.Min(Mathf.Min(averageList.ToArray()), Mathf.Min(bestList.ToArray()));
        float maxValue = Mathf.Max(Mathf.Max(averageList.ToArray()), Mathf.Max(bestList.ToArray()));
        float valueRange = maxValue - minValue;

        minValue -= valueRange * yPadding;
        maxValue += valueRange * yPadding;
        valueRange = maxValue - minValue;

        //float xStep = graphWidth / (averageList.Count - 1);
        // 날짜 수 7로 고정
        float xStep = graphWidth / 6;

        DrawGraph(averageList, graphContainer, Color.red, xStep, graphHeight, minValue, valueRange);
        DrawGraph(bestList, graphContainer, Color.blue, xStep, graphHeight, minValue, valueRange);

        // X축 날짜 레이블 업데이트
        UpdateDateLabels(averageList.Count);
    }

    private void DrawGraph(List<float> valueList, RectTransform graphContainer, Color color, float xStep, float graphHeight, float minValue, float valueRange)
    {
        GameObject lastPointObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPos = i * xStep;
            float yPos = ((valueList[i] - minValue) / valueRange) * graphHeight;
            var pointObject = CreatePoint(new Vector2(xPos, yPos), graphContainer, color);
            if (lastPointObject != null)
            {
                Vector2 pointA = lastPointObject.GetComponent<RectTransform>().anchoredPosition;
                Vector2 pointB = pointObject.GetComponent<RectTransform>().anchoredPosition;
                CreateConnection(pointA, pointB, graphContainer, color);
            }
            CreateScoreText(new Vector2(xPos, yPos), graphContainer, valueList[i]);
            lastPointObject = pointObject;
        }
    }

    private void UpdateDateLabels(int dataCount)
    {
        if (dateLabels.Length < dataCount)
        {
            Debug.LogWarning("날짜 레이블의 수가 데이터 포인트의 수보다 적습니다.");
            return;
        }

        for (int i = 0; i < dataCount; i++)
        {
            dateLabels[i].text = System.DateTime.Now.AddDays(-dataCount + i + 1).ToString("MM/dd");
        }

        for (int i = dataCount; i < dateLabels.Length; i++)
        {
            dateLabels[i].gameObject.SetActive(false);
        }
    }

    private void ClearGraph(RectTransform graphContainer)
    {
        foreach (Transform child in graphContainer)
        {
            Destroy(child.gameObject);
        }
    }
    private void CreateScoreText(Vector2 anchoredPosition, RectTransform graphContainer, float value)
    {
        GameObject textObject = new GameObject("pointScore", typeof(TextMeshProUGUI));
        textObject.transform.SetParent(graphContainer, false);
        RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
        textRectTransform.anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + 4f);
        textRectTransform.sizeDelta = new Vector2(50, 20);
        textRectTransform.anchorMin = new Vector2(0, 0);
        textRectTransform.anchorMax = new Vector2(0, 0);
        textRectTransform.pivot = new Vector2(0, 0);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = string.Format("{0:F1}", value);
        text.fontSize = 14f;
        text.color = Color.black;
    }

    private GameObject CreatePoint(Vector2 anchoredPosition, RectTransform graphContainer, Color color)
    {
        GameObject gameObject = new GameObject("point", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(pointSize, pointSize);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        Image image = gameObject.GetComponent<Image>();
        image.sprite = CreateCircleSprite(32);

        return gameObject;
    }

    private void CreateConnection(Vector2 dotPositionA, Vector2 dotPositionB, RectTransform graphContainer, Color color)
    {
        GameObject gameObject = new GameObject("connection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    private float GetAngleFromVectorFloat(Vector2 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    private Sprite CreateCircleSprite(int resolution)
    {
        Texture2D texture = new Texture2D(resolution, resolution);
        float centerX = resolution / 2f;
        float centerY = resolution / 2f;
        float radius = resolution / 2f;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 100f);
    }
}