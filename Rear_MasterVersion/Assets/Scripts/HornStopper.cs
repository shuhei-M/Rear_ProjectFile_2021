using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> �c�m�X���C�����ǂɓ����������ǂ������肷��Z���T�[�N���X </summary>
public class HornStopper : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    GameObject parent;   // �c�m�X���C��
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor")
        {
            parent.SendMessage("Stop");   // �ǂ⏰�ɃZ���T�[���G�ꂽ�ꍇ�A��~���߂��c�m�ɑ���
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
