using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> �G�̎��E�͈͂��������C�g�̋������Ǘ�����N���X </summary>
public class SearchLight : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    GameObject enemy;   // ���̃��C�g�����A�e�ł���G

    /// <summary> �e�ł���G�̃R���|�[�l���g </summary>
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

        // �e�ł���G������ł����ꍇ�A���̃I�u�W�F�N�g����������
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
