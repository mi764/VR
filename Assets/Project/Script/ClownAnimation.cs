using UnityEngine;

public class ClownAnimation : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        // Animatorコンポーネントを取得
        anim = GetComponent<Animator>();

        // アニメーションを再生
        if (anim != null)
        {
            anim.SetBool("bSurprise", true);
        }
    }
}