using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 敵がプレイヤーを発見した際に表示する！アイコンの向きを調整するクラス </summary>
public class IconController : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    [SerializeField] Camera myCamera;   // メインカメラ
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
        //　カメラと同じ向きに設定
        transform.rotation = myCamera.transform.rotation;
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
