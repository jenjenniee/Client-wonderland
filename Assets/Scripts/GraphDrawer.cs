using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GraphDrawer : MonoBehaviour
{
    public Color[] graphColors = new Color[] {
        Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan, Color.black, Color.gray
    }; // Predefined color array

    private GameObject linePrefab; // Line prefab (created dynamically)

    private void Awake()
    {
        Debug.Log("Awake called");
        CreateLinePrefab();
    }

    private void CreateLinePrefab()
    {
        Debug.Log("CreateLinePrefab called");
        linePrefab = new GameObject("LineSegment");
        linePrefab.AddComponent<RectTransform>();
        Image lineImage = linePrefab.AddComponent<Image>();
        lineImage.color = Color.white; // Set default color
        Debug.Log("LinePrefab created");
    }

    public void DrawGraphs(Dictionary<string, List<Vector2>> themeBestData, Dictionary<string, List<Vector2>> themeAverageData, RectTransform bestGraphPanel, RectTransform averageGraphPanel)
    {
        Debug.Log("DrawGraphs called in GraphDrawer");
        ClearExistingGraphs(bestGraphPanel);
        ClearExistingGraphs(averageGraphPanel);

        DrawGraphSeries(themeBestData, "Best", bestGraphPanel);
        DrawGraphSeries(themeAverageData, "Average", averageGraphPanel);
    }

    private void DrawGraphSeries(Dictionary<string, List<Vector2>> themeData, string dataType, RectTransform graphPanel)
    {
        double minX = double.MaxValue, maxX = double.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        // Find data range
        foreach (var data in themeData.Values)
        {
            if (data.Count == 0) continue;
            minX = System.Math.Min(minX, data.Min(p => (double)p.x));
            maxX = System.Math.Max(maxX, data.Max(p => (double)p.x));
            minY = Mathf.Min(minY, data.Min(p => p.y));
            maxY = Mathf.Max(maxY, data.Max(p => p.y));
        }

        // Handle the case when minX and maxX are equal or minY and maxY are equal
        if (minX == maxX) maxX = minX + 1; // Prevent division by zero
        if (minY == maxY) maxY = minY + 1; // Prevent division by zero

        Debug.Log($"Calculated Min/Max X: {minX}, {maxX}, Min/Max Y: {minY}, {maxY}");

        // New: Calculate rangeX and rangeY only once
        float rangeX = (float)(maxX - minX);
        float rangeY = maxY - minY;

        // Ensure that rangeX and rangeY are not zero
        if (rangeX == 0) rangeX = 1;
        if (rangeY == 0) rangeY = 1;

        int colorIndex = 0;
        foreach (var theme in themeData.Keys)
        {
            var filteredData = themeData[theme];
            Debug.Log($"Theme: {theme}, DataType: {dataType}, Points to draw: {filteredData.Count}");
            if (filteredData.Count > 1)
            {
                Debug.Log($"Drawing {dataType} line for theme {theme} with {filteredData.Count} points");
                // Check if graphColors array is empty
                Color lineColor = graphColors.Length > 0 ? graphColors[colorIndex % graphColors.Length] : Color.white;
                // Updated: Pass rangeX and rangeY to DrawLine method
                DrawLine(theme, dataType, filteredData, graphPanel, lineColor, minX, maxX, minY, maxY, rangeX, rangeY);
                colorIndex++;
            }
            else
            {
                Debug.Log($"Not enough points to draw a {dataType} line for theme {theme}");
            }
        }
    }

    // Updated: Added theme and dataType parameters and rangeX and rangeY parameters
    private void DrawLine(string theme, string dataType, List<Vector2> points, RectTransform graphPanel, Color color, double minX, double maxX, float minY, float maxY, float rangeX, float rangeY)
    {
        Debug.Log($"DrawLine called for theme {theme} ({dataType})");
        GameObject lineObj = new GameObject($"LineObject ({theme} - {dataType})");
        lineObj.transform.SetParent(graphPanel);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localScale = Vector3.one;

        for (int i = 0; i < points.Count - 1; i++)
        {
            GameObject segmentObj = Instantiate(linePrefab, lineObj.transform);
            RectTransform segmentRect = segmentObj.GetComponent<RectTransform>();
            segmentObj.name = $"Segment {i} ({theme} - {dataType})";

            // Updated: Use rangeX and rangeY in NormalizePoint method
            Vector2 start = NormalizePoint(points[i], minX, maxX, minY, maxY, rangeX, rangeY);
            Vector2 end = NormalizePoint(points[i + 1], minX, maxX, minY, maxY, rangeX, rangeY);

            segmentRect.anchorMin = new Vector2(start.x, start.y);
            segmentRect.anchorMax = new Vector2(start.x, start.y);
            // Adjust the length to fit within the panel
            float distance = Vector2.Distance(start, end) * 0.9f * graphPanel.rect.width;
            segmentRect.sizeDelta = new Vector2(distance, 5); // Adjust thickness
            segmentRect.anchoredPosition = Vector2.zero;

            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
            segmentRect.localRotation = Quaternion.Euler(0, 0, angle);

            Debug.Log($"Theme: {theme} ({dataType}), Segment {i}: Start {start}, End {end}, Angle {angle}, Length {segmentRect.sizeDelta.x}");
        }
    }

    // Updated: Added rangeX and rangeY parameters
    private Vector2 NormalizePoint(Vector2 point, double minX, double maxX, float minY, float maxY, float rangeX, float rangeY)
    {
        float normalizedX = (float)((double)point.x - minX) / rangeX;
        float normalizedY = (point.y - minY) / rangeY;

        return new Vector2(normalizedX, normalizedY);
    }

    private void ClearExistingGraphs(RectTransform graphPanel)
    {
        Debug.Log("ClearExistingGraphs called");
        foreach (Transform child in graphPanel)
        {
            Destroy(child.gameObject);
        }
    }
}
