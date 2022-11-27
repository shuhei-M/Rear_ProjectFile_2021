using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> ESCキーを押すことでゲームを強制終了させるクラス </summary>
public class ManagerScript : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

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
        if (Input.GetKey(KeyCode.Escape))
        {
            UnityEngine.Application.Quit();
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion

}
