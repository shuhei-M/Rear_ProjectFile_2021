using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> �Q�[���̌o�ߎ��Ԃ��L�^����N���X </summary>
public class Timer : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

    #endregion

    #region public static
    public static float time;   // �N���A����܂ł̃Q�[���̌o�ߎ���
    public static bool timeStart = false;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.Alive && (StartAndEndGame.EnemyLife > 0) && timeStart)
        {
            time += Time.deltaTime;
        }

        int t = Mathf.FloorToInt(time);
        Text uiText = GetComponent<Text>();
        uiText.text = " : " + t;
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
