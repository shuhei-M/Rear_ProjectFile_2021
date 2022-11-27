using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> ツノスライムが壁に当たったかどうか判定するセンサークラス </summary>
public class HornStopper : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    GameObject parent;   // ツノスライム
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
            parent.SendMessage("Stop");   // 壁や床にセンサーが触れた場合、停止命令をツノに送る
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
