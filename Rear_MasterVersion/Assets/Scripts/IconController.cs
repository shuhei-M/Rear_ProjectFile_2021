using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> �G���v���C���[�𔭌������ۂɕ\������I�A�C�R���̌����𒲐�����N���X </summary>
public class IconController : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    [SerializeField] Camera myCamera;   // ���C���J����
    #endregion

    #region field
    
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        //�@�J�����Ɠ��������ɐݒ�
        transform.rotation = myCamera.transform.rotation;
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
