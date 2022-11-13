using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SearchArea : MonoBehaviour
{
    [SerializeField] SphereCollider searchArea;
    [SerializeField] float searchAngle = 45f;
    [SerializeField] GameObject enemy;

    Animator animator;
    AnimatorStateInfo stateInfo;

    // Start is called before the first frame update
    void Start()
    {
        animator = enemy.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (stateInfo.IsName("Die") || !PlayerController.Alive)
        {
            enemy.SendMessage("Lost");
            return;
        }

        if(other.gameObject.tag == "Hidden")
        {
            enemy.SendMessage("Lost");
            return;
        }

        if (other.gameObject.tag == "HornAvatar")
        {
            // 主人公（分身）の方向
            var Direction = (other.transform.position + new Vector3(0, 0.5f, 0))
                - (transform.position + new Vector3(0, 1.5f, 0));
            // 敵の前方からの主人公（分身）の方向
            var angle = Vector3.Angle(transform.forward, Direction);

            //　サーチする角度内だったら発見
            if (angle <= searchAngle)
            {
                // 方向の取得
                Direction.Normalize();

                RaycastHit hit;

                if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), Direction, out hit, 20f))
                {
                    if (hit.collider.gameObject.tag == "HornAvatar")
                    {   // 分身（ツノスライム）だった場合
                        var ec = enemy.GetComponent<EnemyController>();
                        if (ec.Avatar != null)
                        {
                            other.GetComponent<HornAvatarController>().HateEnemy = enemy;
                        }
                        ec.Avatar = other.gameObject;
                        ec.AvatarIsFinded = true;
                        enemy.SendMessage("Lost");
                    }
                }
            }
        }

        if (other.gameObject.tag == "Player" && enemy.GetComponent<EnemyController>().Avatar == null)
        {
            var eyePosition = new Vector3(
                    transform.position.x,
                    transform.position.y + 1.5f,
                    transform.position.z);

            //　主人公の方向
            var playerDirection = other.transform.position - eyePosition;
            //　敵の前方からの主人公の方向
            var angle = Vector3.Angle(transform.forward, playerDirection);

            //Debug.Log("オントリガーエンター");
            //Debug.Log(angle);

            //　サーチする角度内だったら発見
            if (angle <= searchAngle)
            {
                // 方向の取得
                playerDirection.Normalize();

                RaycastHit hit;

                

                if (Physics.Raycast(eyePosition, playerDirection, out hit, 20f))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        enemy.SendMessage("Find");
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(stateInfo.IsName("Die") || !PlayerController.Alive)
        {
            enemy.SendMessage("Lost");
            return;
        }

        if (other.gameObject.tag == "Player")
        {
            enemy.SendMessage("Lost");
            //Debug.Log("見失った02");
        }
    }

//#if UNITY_EDITOR
//    //　サーチする角度表示
//    private void OnDrawGizmos()
//    {
//        Handles.color = Color.blue;
//        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius);
//    }
//#endif
}