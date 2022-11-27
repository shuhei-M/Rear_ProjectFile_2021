using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> �����̗L���͈͂������A���E���Ղ����G���L�^����N���X </summary>
/// <summary> EnemySmokedReset()�֐��ȊO�̑S�Ă��쐬 </summary>
public class SmokeArea : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    List<GameObject> smokedEnemy;   // �����Ŏ��E���Ղ��Ă���G���L�^����
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
        // �G�������̗L���͈͂ɓ������ꍇ
        if (other.gameObject.tag == "Enemy")
        {
            var Enemy = other.gameObject;
            var ec = Enemy.GetComponent<EnemyController>();

            //�@�P�����̕���
            var smokeDirection = transform.position - Enemy.transform.position;
            //�@�G�̑O������̃P�����̕���
            var angle = Vector3.Angle(Enemy.transform.forward, smokeDirection);

            if (angle <= 90f)
            {
                ec.IsSmoked = true;
                Debug.Log("�P�����őO�������Ȃ��I");

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
            Debug.Log("�P��������o�܂����B");

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
