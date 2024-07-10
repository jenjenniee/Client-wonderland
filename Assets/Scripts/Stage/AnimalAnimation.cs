using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimation : MonoBehaviour
{
    public Sprite[] newSprite; // 새로 적용할 스프라이트
    public AnimationClip[] newIdleAnimationClip; // 새로 적용할 idle 애니메이션 클립

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int index = 0;

    void Awake()
    {
        // 게임 오브젝트의 SpriteRenderer와 Animator 컴포넌트를 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        spriteRenderer.sprite = newSprite[index];
    }

    void OnEnable()
    {
        index = Random.Range(0, newSprite.Length);
        // SpriteRenderer의 sprite를 새 스프라이트로 변경합니다.
        animator.SetInteger("Index", index);
        spriteRenderer.sprite = newSprite[index];
        Debug.Log($"Index: {index}, newSprite[index]: {newSprite[index]}");

        // SpriteRenderer와 Animator가 존재하는지 확인합니다.
        if (spriteRenderer != null && animator != null)
        {
            ChangeSpriteAndIdleAnimation();
        }
        else
        {
            Debug.LogError("SpriteRenderer or Animator component is missing on this game object.");
        }
    }

    private void ChangeSpriteAndIdleAnimation()
    {
        /*
        // 기존 Animator Controller를 기반으로 Animator Override Controller를 생성합니다.
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        // "idle" 상태의 애니메이션 클립을 새 클립으로 변경합니다.
        overrideController["Idle"] = newIdleAnimationClip[index];

        // Animator의 Runtime Animator Controller를 Override Controller로 설정합니다.
        animator.runtimeAnimatorController = overrideController;

        Debug.Log(newIdleAnimationClip[index]);
        */
    }
}
