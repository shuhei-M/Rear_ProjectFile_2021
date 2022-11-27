using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 敵の視界範囲を示すライトの挙動を管理するクラス </summary>
public class SearchLight : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    GameObject enemy;   // このライトを持つ、親である敵

    /// <summary> 親である敵のコンポーネント </summary>
    Animator animator;
    AnimatorStateInfo stateInfo;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.gameObject;

        animator = enemy.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 親である敵が死んでいた場合、このオブジェクトを消去する
        if (stateInfo.IsName("Die"))
        {
            Destroy(gameObject, 0.05f);
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
