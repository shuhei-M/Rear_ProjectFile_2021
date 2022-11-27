using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 煙幕の有効範囲を示し、視界を遮った敵を記録するクラス </summary>
/// <summary> EnemySmokedReset()関数以外の全てを作成 </summary>
public class SmokeArea : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    List<GameObject> smokedEnemy;   // 煙幕で視界を遮っている敵を記録する
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        smokedEnemy = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        // 敵が煙幕の有効範囲に入った場合
        if (other.gameObject.tag == "Enemy")
        {
            var Enemy = other.gameObject;
            var ec = Enemy.GetComponent<EnemyController>();

            //　ケムリの方向
            var smokeDirection = transform.position - Enemy.transform.position;
            //　敵の前方からのケムリの方向
            var angle = Vector3.Angle(Enemy.transform.forward, smokeDirection);

            if (angle <= 90f)
            {
                ec.IsSmoked = true;
                Debug.Log("ケムリで前が見えない！");

                for (int i = 0; i < smokedEnemy.Count; i++)
                {
                    if (smokedEnemy[i] == Enemy)
                    {
                        return;
                    }
                }
                smokedEnemy.Add(Enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            var Enemy = other.gameObject;
            var ec = Enemy.GetComponent<EnemyController>();
            ec.IsSmoked = false;
            Debug.Log("ケムリから出ました。");

            smokedEnemy.Remove(Enemy);
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function
    void EnemySmokedReset()
    {
        for (int i = 0; i < smokedEnemy.Count; i++)
        {
            var ec = smokedEnemy[i].transform.GetComponent<EnemyController>();
            ec.IsSmoked = false;
        }
    }
    #endregion
}
