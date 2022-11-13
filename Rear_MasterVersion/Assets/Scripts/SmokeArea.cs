using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeArea : MonoBehaviour
{
    List<GameObject> smokedEnemy;

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

            //for (int i = 0; i < smokedEnemy.Count; i++)
            //{
            //    if (smokedEnemy[i] == Enemy)
            //    {
            //        smokedEnemy.Remove()
            //    }
            //}
        }
    }

    void EnemySmokedReset()
    {
        for (int i = 0; i < smokedEnemy.Count; i++)
        {
            var ec = smokedEnemy[i].transform.GetComponent<EnemyController>();
            ec.IsSmoked = false;
        }
    }
}
