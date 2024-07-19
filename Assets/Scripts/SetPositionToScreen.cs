using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPositionToScreen : MonoBehaviour
{
    public GameObject worldObject;
    public Vector2 offset;


    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldObject.transform.position);

        rect.position = screenPosition + offset;
    }

    //public float offsetY;
    //public float offsetX;
    //
    //public Transform target;
    //
    //private RectTransform rect;
    //
    //private void Start()
    //{
    //    rect = GetComponent<RectTransform>();
    //
    //    var targetScreenPosition = GetScreenPosition(target.position);
    //    var position = targetScreenPosition + new Vector2(offsetX, offsetY);
    //
    //    Debug.Log($"{targetScreenPosition.x}, {targetScreenPosition.y}");
    //
    //    SetUIPosition(position);
    //}
    //
    ///// <summary>
    ///// 타겟의 월드 좌표를 스크린좌표로 변환
    ///// </summary>
    //private Vector2 GetScreenPosition(Vector3 worldPosition)
    //    => Camera.main.WorldToScreenPoint(worldPosition);
    //
    ////private Vector2 SetScreenPosition(Vector2 screenPosition)
    ////    => rect.anchoredPosition = screenPosition;
    //
    //private void SetUIPosition(Vector2 screenPosition)
    //{
    //    var localPos = Vector2.zero;
    //
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPosition, Camera.main, out localPos);
    //
    //    GameObject obj = new GameObject("Test Point");
    //    obj.transform.SetParent(transform);
    //
    //    var image = obj.AddComponent<Image>();
    //
    //    RectTransform testImageRect = image.rectTransform;
    //    testImageRect.anchorMin = testImageRect.anchorMax = new Vector2(.5f, .5f);
    //    testImageRect.sizeDelta = new Vector2(100, 100);
    //
    //
    //    rect.localPosition = localPos;
    //}


    //public Canvas canvas;
    //public Transform target;
    //public Vector2 offset = new Vector2(.0f, 50.0f);
    //
    //private RectTransform rect;
    //
    //
    //private void Start()
    //{
    //    rect = GetComponent<RectTransform>();
    //
    //    SetPosition();
    //}
    //
    //private void SetPosition()
    //{
    //    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);
    //
    //    Vector2 localPoint;
    //
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
    //        screenPoint,
    //        canvas.worldCamera,
    //        out localPoint);
    //
    //    localPoint += offset;
    //
    //    rect.anchoredPosition = localPoint;
    //}
}
