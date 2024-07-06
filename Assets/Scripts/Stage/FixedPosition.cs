using UnityEngine;

public class FixedPosition : MonoBehaviour
{
    private Vector3 initialPosition;
    private Animator animator;
    private OpenSceneHandler hadlerScript;

    void Start()
    {
        // 초기 위치 저장
        initialPosition = transform.position;
        animator = GetComponent<Animator>();
        hadlerScript = GetComponent<OpenSceneHandler>();
    }

    void LateUpdate()
    {
        // y 값만 애니메이션화하고 x와 z 값은 고정
        Vector3 animatedPosition = transform.position;
        transform.position = new Vector3(initialPosition.x, animatedPosition.y, initialPosition.z);

    }
}
