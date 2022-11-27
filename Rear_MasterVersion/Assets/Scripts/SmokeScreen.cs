using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> �����̃G�t�F�N�g�𔭐�������N���X </summary>
public class SmokeScreen : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    [SerializeField] Material targetMaterial;   // �����p�}�e���A��

    /// <summary> �����̖͗l���������� </summary>
    [SerializeField] float scrollX;
    [SerializeField] float scrollY;

    [SerializeField] float scaleUpSpeed;   // �������g�傷��X�s�[�h
    #endregion

    #region field
    Vector2 offset;   // �����͗l�X�N���[���p

    float scale = 1.0f;   // �����G�t�F�N�g�̑傫��
    #endregion

    #region property

    #endregion

    #region Unity function
    private void Awake()
    {
        offset = targetMaterial.mainTextureOffset;
    }

    private void Update()
    {
        offset.x += scrollX * Time.deltaTime;
        offset.y += scrollY * Time.deltaTime;
        targetMaterial.mainTextureOffset = offset;

        // �傫��10�ɓ��B����܂ŁA�����G�t�F�N�g�͏��X�ɑ傫���Ȃ�
        if (scale < 10.0f)
        {
            scale += scaleUpSpeed;
            if (scale > 10.0f) scale = 10.0f;
            this.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
