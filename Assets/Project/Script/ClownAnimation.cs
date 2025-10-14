using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownAnimation : MonoBehaviour
{
    //===== 定義領域 =====
    private Animator anim;  //Animatorをanimという変数で定義する

    //===== 初期処理 =====
    void Start()
    {
        //変数animに、Animatorコンポーネントを設定する
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("bSurprise", true);
    }
}
