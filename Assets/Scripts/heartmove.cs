using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class heartmove : MonoBehaviour
{
    public float maxSpeed;
    public float maxHeight;
    public float minStep = 0.2f;

    public RectTransform start;
    public RectTransform end;

    public AnimationCurve trajectoryCurve;
    public AnimationCurve axisCorrectionCurve;
    public AnimationCurve speedCurve;

    private float speed;
    private float trajectoryMaxRelativeHeight;

    private Vector2 trajectoryRange;

    private float nextYTrajectoryPosition;
    private float nextXTrajectoryPosition;
    private float nextPositionYCorrectionAbsolute;
    private float nextPositionXCorrectionAbsolute;

    public Action OnTargetReached;
    public bool HasReachedTarget { get; private set; }

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 endPosition;

    public void OnEnable()
    {

        rectTransform = GetComponent<RectTransform>();
 
        HasReachedTarget = false;

        speed = maxSpeed;

        startPosition = start.anchoredPosition;
        endPosition = end.anchoredPosition;

        rectTransform.position = startPosition;

        float xDistanceToTarget = endPosition.x - startPosition.x;
        trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * maxHeight;

        trajectoryRange = endPosition - startPosition;
    }

    public void Update()
    {
        Movement();
    }

    public void Movement()
    {
        if (HasReachedTarget)
            return;

        UpdatePosition();

        Debug.Log(Vector2.Distance(rectTransform.anchoredPosition, endPosition));

        if (Vector2.Distance(rectTransform.anchoredPosition, endPosition) < minStep)
        {
            rectTransform.anchoredPosition = endPosition;

            OnTargetReached?.Invoke();

            HasReachedTarget = true;
            rectTransform.position = startPosition;
            gameObject.SetActive(false);
        }
    }

    protected void UpdatePosition()
    {
        if (Mathf.Abs(trajectoryRange.normalized.x) < Mathf.Abs(trajectoryRange.normalized.y))
        {
            if (trajectoryRange.y < 0)
                speed = -speed;

            UpdatePositionWithXCurve();
        }
        else
        {
            if (trajectoryRange.x < 0)
                speed = -speed;

            UpdatePositionWithYCurve();
        }
    }

    private void UpdatePositionWithYCurve()
    {
        float nextPositionX = rectTransform.anchoredPosition.x + speed * Time.deltaTime;

        float nextPositionXNormalized = (nextPositionX - startPosition.x) / trajectoryRange.x;

        float nextPositionYNormalized = trajectoryCurve.Evaluate(nextPositionXNormalized);
        nextYTrajectoryPosition = nextPositionYNormalized * trajectoryMaxRelativeHeight;

        float nextPositionYCorrectionNormalized = axisCorrectionCurve.Evaluate(nextPositionXNormalized);
        nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;

        float nextPositionY = startPosition.y + nextYTrajectoryPosition + nextPositionYCorrectionAbsolute;

        Vector2 newPosition = new Vector2(nextPositionX, nextPositionY);

        CalculateNextSpeed(nextPositionXNormalized);

        rectTransform.anchoredPosition = newPosition;
    }

    private void UpdatePositionWithXCurve()
    {
        float nextPositionY = rectTransform.anchoredPosition.y + speed * Time.deltaTime;
        float nextPositionYNormalized = (nextPositionY - startPosition.y) / trajectoryRange.y;

        float nextPositionXNormalized = trajectoryCurve.Evaluate(nextPositionYNormalized);
        nextXTrajectoryPosition = nextPositionXNormalized * trajectoryMaxRelativeHeight;

        float nextPositionXCorrectionNormalized = axisCorrectionCurve.Evaluate(nextPositionXNormalized);
        nextPositionXCorrectionAbsolute = nextPositionXCorrectionNormalized * trajectoryRange.x;

        if (trajectoryRange.x > 0 && trajectoryRange.y > 0)
            nextXTrajectoryPosition = -nextXTrajectoryPosition;

        if (trajectoryRange.x < 0 && trajectoryRange.y < 0)
            nextXTrajectoryPosition = -nextXTrajectoryPosition;

        float nextPositionX = startPosition.x + nextXTrajectoryPosition + nextPositionXCorrectionAbsolute;

        Vector2 newPosition = new Vector2(nextPositionX, nextPositionY);

        CalculateNextSpeed(nextPositionYNormalized);

        rectTransform.anchoredPosition = newPosition;
    }

    private void CalculateNextSpeed(float nextPositionXNormalized)
    {
        float NextMoveSpeedNormalized = speedCurve.Evaluate(nextPositionXNormalized);

        speed = NextMoveSpeedNormalized * maxSpeed;
    }
}