using UnityEngine;

public class FixedPosition : MonoBehaviour
{
    [SerializeField]
    private float xPosition;
    private Animator animator;
    private OpenSceneHandler hadlerScript;

    void Start()
    {
        // 초기 위치 저장
        animator = GetComponent<Animator>();
        hadlerScript = GetComponent<OpenSceneHandler>();
    }

    void LateUpdate()
    {
        // y 값만 애니메이션화하고 x와 z 값은 고정
        Vector3 animatedPosition = transform.position;
        transform.position = new Vector3(xPosition, animatedPosition.y, 0f);

    }
}
