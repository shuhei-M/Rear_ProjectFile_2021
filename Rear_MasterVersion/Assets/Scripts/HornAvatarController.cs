using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary> ツノスライムの挙動を管理するクラス（部分的に担当） </summary>
/// <summary> 移動方法・衝突検知方法を変更。</summary>
/// <summary> 死亡時赤いエミッションを放つ機能・ジャンプ機能を追加。 </summary>
public class HornAvatarController : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    /// <summary> (松島)ジャンプする際のパラメータ変数群 </summary>
    [SerializeField] float jumpSpeed = 5.0f;   // ジャンプの上方向（y軸）への初速度
    [SerializeField] float gravity = 20.0f;   // 重力
    [SerializeField] float jumpMagnification = 1.1f;   // ジャンプ力
    #endregion

    #region field
    /// <summary> ↓以下、松島が追加</summary>
    /// <summary> 自身にアタッチされたコンポーネントを取得する変数群 </summary>
    CharacterController charaCon;
    Animator animator;
    AnimatorStateInfo stateInfo;
    NavMeshAgent agent;

    GameObject hornStopper;   // これが壁にぶつかった際、ツノの動きを止める

    bool IsJump = false;   // ジャンプ中かどうか
    private Vector3 moveDirection = Vector3.zero;   // ジャンプ中の上方向への動き

    Vector3 GoStraight;   // ツノスライムが直進する際の向き

    Vector3 v = Vector3.zero;   // 初期化用のベクトル
    /// <summary> ↑まで、松島が追加 </summary>

    List<GameObject> hateEnemy;
    float deltaTime = 0f;
    float dieCountDown = 0f;
    float moveSpeed = 2.0f;
    bool stop = false;
    bool dieFlag = false;
    #endregion

    #region property
    public GameObject HateEnemy
    {   // 追跡してきた敵を保管
        set { hateEnemy.Add(value); }
    }
    #endregion

    #region Unity function
    void Start()
    {
        animator = GetComponent<Animator>();
        hateEnemy = new List<GameObject>();

        agent = GetComponent<NavMeshAgent>();
        GoStraight = transform.forward * moveSpeed * Time.deltaTime;

        charaCon = GetComponent<CharacterController>();
        charaCon.enabled = false;

        hornStopper = transform.GetChild(2).gameObject;
    }

    void Update()
    {
        /// <summary> ↓以下、松島が追加 </summary>
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stop)
        {
            deltaTime = 0;
            dieCountDown += Time.deltaTime;
            GoStraight = v;
            agent.isStopped = true;
        }
        else
        {
            deltaTime += Time.deltaTime;
            agent.Move(GoStraight);
        }

        if (IsJump)
        {
            moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
            charaCon.Move((GoStraight / jumpMagnification) + (moveDirection * Time.deltaTime));
            if (charaCon.isGrounded)
            {
                IsJump = false;
                agent.Warp(transform.position);
                agent.updatePosition = true;
                charaCon.enabled = false;
            }
        }

        animator.SetFloat("DeltaTime", deltaTime);
        /// <summary> ↑まで、松島が追加 </summary>

        if (dieCountDown >= 1.5f)
        {
            EnemyTargetReset();
            animator.SetTrigger("toDie");
            Destroy(this.gameObject);
        }

        if(dieFlag)
        {
            if(transform.localScale.y >= 0)
            {
                transform.localScale -= new Vector3(0.0f, 0.01f, 0.0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //// 煙の中に入ったら探知されなくなる
        //if (other.gameObject.tag == "SmokeArea")
        //{
        //    EnemyTargetReset();
        //    this.tag = "Untagged";
        //}

        if (other.gameObject.tag == "Sword")
        {   // 攻撃されたときに処理
            var Enemy = other.transform.root.GetChild(0);
            var ec = Enemy.GetComponent<EnemyController>();

            EnemyTargetReset();

            /// <summary> 以下、松島が追加 </summary>
            //死亡時アニメーションの代替処理
            var ps = GetComponent<ParticleSystem>();
            var ep = new ParticleSystem.EmitParams();
            ep.startColor = Color.red;
            ep.startSize = 0.1f;
            ps.Emit(ep, 1000);

            animator.SetTrigger("toDie");
            Debug.Log("ツノやられた！");
            Destroy(this.gameObject, 0.5f);
            Destroy(hornStopper);
            dieFlag = true;
            GoStraight = v;
            agent.isStopped = true;
        }

        /// <summary> (松島)段差上側のセンサーに触れた </summary>
        if (other.gameObject.tag == "JumpPoint")
        {
            Debug.Log("ツノ飛びます！");

            Jump();
        }
    }

    public void EnemyTargetReset()
    {
        gameObject.tag = "Untagged";
        for (int i = 0; i < hateEnemy.Count; i++)
        {
            var ec = hateEnemy[i].transform.GetComponent<EnemyController>();
            ec.Avatar = null;
            ec.AvatarIsFinded = false;
        }
        hateEnemy.Clear();
    }
    #endregion

    #region public function
    /// <summary>
    /// 制作者：松島
    /// ツノスライムをジャンプさせる。
    /// 段差上側に仕込まれたセンサーに触れた際に呼ばれる
    /// </summary>
    public void Jump()
    {
        Debug.Log("Jump");
        charaCon.enabled = true;   // 移動方法をキャラクターコントローラーに切り替える
        IsJump = true;
        agent.updatePosition = false;
        moveDirection.y = jumpSpeed;
        //playerSE.JumpSE();
    }
    #endregion

    #region private function
    /// <summary>
    /// 制作者：松島
    /// ツノスライムが壁にぶつかり、直進を止める。
    /// </summary>
    private void Stop()
    {
        Debug.Log("ツノ衝突！");
        stop = true;
    }
    #endregion
}
