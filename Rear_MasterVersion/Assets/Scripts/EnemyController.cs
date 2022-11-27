using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary> 敵一体の挙動を管理するクラス </summary>
public class EnemyController : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    /// <summary> 徘徊モード・駐在モードを切り替える。企画担当のレベルデザイン用 </summary>
    [SerializeField] bool IsPatrolMode;

    [SerializeField] float searchAngle = 120f;   // 敵に対して、プレイヤーの攻撃が当たる範囲

    [SerializeField] Transform[] points;   // 徘徊モード時の、巡回するポイント

    [SerializeField] float stopDistance = 1.5f;   // 攻撃を開始するプレイヤーとの距離。
    [SerializeField] GameObject Sword;   // 敵が振るう剣。当たり判定付き。

    [SerializeField] GameObject quotationMark;   // プレイヤーを見つけた際に頭上に表示する！アイコン

    [SerializeField] GameObject lifeText;   // 敵の残機数を表示するテキストUI

    [SerializeField] GameObject SEObj;   // SE
    #endregion

    #region field
    /// <summary> 自身にアタッチされたコンポーネントを取得する変数群 </summary>
    NavMeshAgent agent;
    Animator animator;
    AnimatorStateInfo stateInfo;

    private int destPoint = 0;   // 現在向かうべき巡回ポイントの添え字

    GameObject searchArea;   // 視認距離を示すスフィア

    GameObject player;   // プレイヤー
    GameObject avatar;   // 追跡対象となる分身
    GameObject tracked;   // 追跡対象

    float stunTime = 0f;   // スタン経過時間

    bool Alive;   // 生きているかどうか

    const float walkSpeed = 1.5f;   // 徘徊時のスピード
    float runSpeed = 3.0f;          // 追跡時のスピード
    const float statusUpRatio = 0.4f;   // 敵の残機が、最初から何割になったら強化モードに移行するか

    int ollEnemyLife;   // 敵の残機

    bool IsFinded;   // プレイヤーを見つけたかどうか
    bool avatarIsFinded;   // 分身を見つけたかどうか

    bool isSmoked;   // 煙幕をたかれ、視界が遮られているかどうか

    bool fullEnemyCountFlag = false;   // 敵が全員生き残っているかどうか
    bool powerUpFlag = false;   // 敵が強化モードかどうか

    float attackStateFrame = 0.0f;   // 攻撃時の経過フレーム

    bool IsStun = false;   // スタン状態かどうか

    /// <summary> 駐在モードが、元の場所に戻った際、向きを初期状態に戻すためのする変数群 </summary>
    bool IsSetMoveRotation = true;
    bool CanMoveRotation = false;
    float step;
    Vector3 startAngle;
    #endregion

    #region property
    public GameObject Avatar
    {
        get { return avatar; }
        set { avatar = value; }
    }

    public bool AvatarIsFinded
    {
        set { avatarIsFinded = value; }
    }

    public bool IsSmoked
    {
        get { return isSmoked; }
        set { isSmoked = value; }
    }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        player = GameObject.Find("Player");

        agent.autoBraking = false;

        Alive = true;

        IsFinded = false;

        isSmoked = false;

        if (IsPatrolMode)
        {
            animator.SetTrigger("toWalk");
            GotoNextPoint();
        }

        //step = 45.0f * Time.deltaTime;
        startAngle = points[points.Length - 1].rotation.eulerAngles;

        searchArea = transform.GetChild(5).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fullEnemyCountFlag)
        {
            ollEnemyLife = StartAndEndGame.EnemyLife;
            fullEnemyCountFlag = true;
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 敵が後半に強化
        if(StartAndEndGame.EnemyLife < ollEnemyLife * statusUpRatio &&
           Alive && !powerUpFlag)
        {
            runSpeed *= 2f;
            var enemyLight = transform.GetChild(6).GetChild(0).gameObject;
            enemyLight.GetComponent<Light>().color = new Color(1, 0, 0, 1);
            lifeText.GetComponent<Text>().color = new Color(1, 0, 0, 1);
            powerUpFlag = true;
        }

        if (IsPatrolMode)
        {
            // 徘徊モード
            PatrolMode_Update();
        }
        else
        {
            // 駐在モード
            StayMode_Update();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーから攻撃を喰らったら
        if (other.gameObject.tag == "Head")
        {
            // 既に自身が死んでいれば、以下の処理は行わない。
            if (stateInfo.IsName("Die")) return;

            //　主人公の方向
            var headDirection = other.transform.position - transform.position;
            //　敵の前方からの主人公の方向
            var angle = Vector3.Angle(transform.forward, headDirection);

            // プレイヤーの攻撃が、有効範囲内に当たっていれば、敵は死ぬ
            if (angle >= searchAngle)
            {
                Debug.Log("Hit!");
                animator.SetTrigger("toDie");
                SEObj.GetComponent<EnemySEScript>().DieSE();
                Invoke("SEStop", 1.1f);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "SmokeArea")
        {
            IsFinded = false;
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// StayMode（駐在モード）用のアップデート関数
    /// </summary>
    private void StayMode_Update()
    {
        if (!stateInfo.IsName("Run")) quotationMark.SetActive(false);

        if (!stateInfo.IsName("Attack"))
        {
            SwordActive(false);
            if (attackStateFrame != 0.0f)
            {
                Debug.Log(attackStateFrame);
                attackStateFrame = 0.0f;
            }
        }

        if ((!stateInfo.IsName("Die")) && (!PlayerController.Alive))
        {
            if (!stateInfo.IsName("Idle")) animator.SetTrigger("toIdle");
            return;
        }

        // スタン状態で無ければ常に探知できるようにする
        // これをしなければ、スタン中に見つけた敵を次に追いかけ始めてしまう
        if (!stateInfo.IsName("Stun"))
        {
            if (avatarIsFinded)
            {   //分身を発見したら優先的に追跡状態に切り替え
                tracked = avatar;
            }
            else if (IsFinded)
            {   //プレイヤーを発見したら追跡状態に切り替え
                tracked = player;
            }
            else
            {
                tracked = null;
            }
        }

        if (isSmoked && avatarIsFinded && avatar.tag == "HornAvatar")
        {
            avatar = null;
            avatarIsFinded = false;
        }

        //--- 待機状態（Idleステート） ---//
        if (stateInfo.IsName("Idle"))
        {
            agent.isStopped = true;

            if (!IsSetMoveRotation)
            {
                transform.position = points[points.Length - 1].position;
                var v = transform.rotation.eulerAngles;
                var speed = Mathf.Abs(startAngle.y - v.y);
                step = speed * Time.deltaTime / 2;   //2秒かける
                IsSetMoveRotation = true;
                CanMoveRotation = true;
            }

            if (CanMoveRotation)
            {
                //指定した方向にゆっくり回転する場合
                transform.rotation = Quaternion.RotateTowards
                    (transform.rotation,
                    Quaternion.Euler(0, startAngle.y, 0), step);
            }

            //if (!isSmoked && (avatarIsFinded || IsFinded) && !IsStun/*stateInfo.IsName("Stun")*/)
            //{   
            //    // 追跡ターゲットがいれば追跡状態に切り替え
            //    animator.SetTrigger("toRun");
            //    agent.speed = runSpeed;
            //    SEObj.GetComponent<EnemySEScript>().DiscoverSE();
            //}

            if ((avatarIsFinded || IsFinded) && !IsStun)
            {
                if (!isSmoked || tracked.tag == "NoteAvatar")
                {
                    animator.SetTrigger("toRun");
                    agent.speed = runSpeed;
                    SEObj.GetComponent<EnemySEScript>().DiscoverSE();
                }
            }

        }
        //--- 追跡状態（Runステート） ---//
        else if (stateInfo.IsName("Run"))
        {
            agent.isStopped = false;

            if (tracked != null)
            {
                //プレイヤーを見失ったら帰路状態に切り替え
                if ((isSmoked || (!IsFinded && !avatarIsFinded)) && tracked.tag != "NoteAvatar")
                {
                    animator.SetTrigger("toWalk");
                    agent.speed = walkSpeed;
                }

                agent.destination = tracked.transform.position;
                quotationMark.SetActive(true);

                float distance = Vector3.Distance(transform.position, tracked.transform.position); //☆

                var playerDirection = tracked.transform.position - transform.position;
                var angle = Vector3.Angle(transform.forward, playerDirection);
                if (PlayerController.Alive && distance < stopDistance && angle <= 30)
                {
                    animator.SetTrigger("toAttack");
                    SEObj.GetComponent<EnemySEScript>().FightSE();
                }
            }
            else
            {
                animator.SetTrigger("toWalk");
                agent.speed = walkSpeed;
            }
        }
        //--- 帰路状態（Walkステート） ---//
        else if (stateInfo.IsName("Walk"))
        {
            agent.isStopped = false;

            //agent.destination = startPoint.position;
            agent.destination = points[points.Length - 1].position;

            if (!isSmoked && (avatarIsFinded || IsFinded))
            {
                // 追跡ターゲットがいれば追跡状態に切り替え
                animator.SetTrigger("toRun");
                agent.speed = runSpeed;
            }

            //帰還したら
            if (agent.remainingDistance < 0.1f/*transform.position == points[points.Length - 1].position*/)
            {
                IsSetMoveRotation = false;
                //帰還したら待機状態に切り替え
                animator.SetTrigger("toIdle");
            }
        }
        //--- 死亡状態（Dieステート） ---//
        else if (stateInfo.IsName("Die"))
        {
            //agent.isStopped = true;
            if (Alive)
            {
                agent.isStopped = true;
                StartAndEndGame.EnemyLife--;
                Text uiText = lifeText.GetComponent<Text>();
                uiText.text = " × " + StartAndEndGame.EnemyLife;
                agent.enabled = false;
            }
            Alive = false;
        }
        //--- スタン状態（Stunステート）---//
        else if (stateInfo.IsName("Stun"))
        {
            agent.isStopped = true;
            if (stunTime < 5f)
            {
                stunTime += Time.deltaTime;
            }
            else
            {   // 5秒経過でスタン解除
                stunTime = 0f;
                IsStun = false;
                tracked = null;
                IsFinded = false;
                avatarIsFinded = false;
                searchArea.SetActive(true);
                animator.ResetTrigger("toAttack");
                animator.ResetTrigger("toRun");
                animator.SetTrigger("toWalk");
                animator.SetBool("IsStun", false);
                agent.speed = walkSpeed;
                SEObj.GetComponent<EnemySEScript>().UnStunSE();
            }
        }
        //--- 攻撃状態（Attackステート） ---//
        else if (stateInfo.IsName("Attack"))
        {
            agent.isStopped = true;
            attackStateFrame += Time.deltaTime;
            //Sword.SetActive(true);
            if (attackStateFrame > 0.13f && attackStateFrame < 0.21f)
            {
                SwordActive(true);
            }
            else
            {
                SwordActive(false);
            }
        }
    }

    /// <summary>
    /// PatrolMode（巡回モード）用のアップデート関数
    /// </summary>
    private void PatrolMode_Update()
    {
        if (!stateInfo.IsName("Die"))
        {
            if (!PlayerController.Alive)
            {
                animator.SetTrigger("toIdle");
                return;
            }
        }

        // スタン状態で無ければ常に探知できるようにする
        // これをしなければ、スタン中に見つけた敵を次に追いかけ始めてしまう
        if (!stateInfo.IsName("Stun"))
        {
            if (avatarIsFinded)
            {   //分身を発見したら優先的に追跡状態に切り替え
                tracked = avatar;
            }
            else if (IsFinded)
            {   //プレイヤーを発見したら追跡状態に切り替え
                tracked = player;
            }
            else
            {
                tracked = null;
            }
        }

        if (isSmoked && avatarIsFinded && avatar.tag == "HornAvatar")
        {
            avatar = null;
            avatarIsFinded = false;
        }

        //--- 徘徊状態（Walkステート） ---//
        if (stateInfo.IsName("Walk"))
        {
            agent.isStopped = false;

            //if (!isSmoked && (avatarIsFinded || IsFinded) && !IsStun/*stateInfo.IsName("Stun")*/)
            //{   // 追跡ターゲットがいれば追跡状態に切り替え
            //    animator.SetTrigger("toRun");
            //    agent.speed = runSpeed;
            //    SEObj.GetComponent<EnemySEScript>().DiscoverSE();
            //}

            if ((avatarIsFinded || IsFinded) && !IsStun)
            {
                if (!isSmoked || tracked.tag == "NoteAvatar")
                {
                    animator.SetTrigger("toRun");
                    agent.speed = runSpeed;
                    SEObj.GetComponent<EnemySEScript>().DiscoverSE();
                }
            }

            // エージェントが現目標地点に近づいてきたら、
            // 次の目標地点を選択します
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GotoNextPoint();
            }
        }

        //--- 追跡状態（Runステート） ---//
        if (stateInfo.IsName("Run"))
        {
            if (tracked != null)
            {
                //プレイヤーを見失ったら徘徊状態に切り替え
                if ((isSmoked || (!IsFinded && !avatarIsFinded)) && tracked.tag != "NoteAvatar")
                {
                    //agent.destination = points[0].position;
                    animator.SetTrigger("toWalk");
                    agent.speed = walkSpeed;
                    agent.destination = points[0].position;
                }
                else
                {
                    agent.destination = tracked.transform.position;
                    quotationMark.SetActive(true);

                    float distance = Vector3.Distance(transform.position, tracked.transform.position); //☆

                    var playerDirection = tracked.transform.position - transform.position;
                    var angle = Vector3.Angle(transform.forward, playerDirection);
                    if (PlayerController.Alive && distance < stopDistance && angle <= 30)
                    {
                        animator.SetTrigger("toAttack");
                        SEObj.GetComponent<EnemySEScript>().FightSE();
                    }
                }
            }
            else
            {
                animator.SetTrigger("toWalk");
                agent.speed = walkSpeed;
            }
        }
        else
        {
            quotationMark.SetActive(false);
        }

        //--- 死亡状態（Dieステート） ---//
        if (stateInfo.IsName("Die"))
        {
            //agent.isStopped = true;
            if (Alive)
            {
                agent.isStopped = true;
                StartAndEndGame.EnemyLife--;
                Text uiText = lifeText.GetComponent<Text>();
                uiText.text = " × " + StartAndEndGame.EnemyLife;
                agent.enabled = false;
            }
            Alive = false;
        }

        //--- スタン状態（Stunステート）---//
        if (stateInfo.IsName("Stun"))
        {
            if (stunTime < 5f)
            {
                stunTime += Time.deltaTime;
            }
            else
            {   // 5秒経過でスタン解除
                stunTime = 0f;
                IsStun = false;
                tracked = null;
                IsFinded = false;
                avatarIsFinded = false;
                searchArea.SetActive(true);
                animator.ResetTrigger("toAttack");
                animator.ResetTrigger("toRun");
                animator.SetTrigger("toWalk");
                animator.SetBool("IsStun", false);
                agent.speed = walkSpeed;
                SEObj.GetComponent<EnemySEScript>().UnStunSE();
            }
        }

        //--- 攻撃状態（Attackステート） ---//
        if (stateInfo.IsName("Attack"))
        {
            agent.isStopped = true;
            Sword.SetActive(true);
        }
        else
        {
            Sword.SetActive(false);
        }

        //--- 待機状態（Idleステート） ---//
        if (stateInfo.IsName("Idle"))
        {
            agent.isStopped = true;
            if (PlayerController.Alive)
            {
                animator.SetTrigger("toWalk");
                agent.speed = walkSpeed;
            }
            else
            {
                agent.isStopped = true;
            }
        }

        if (!PlayerController.Alive)
        {
            animator.SetTrigger("toIdle");
        }
    }

    /// <summary>
    /// 敵を指定された場所を順繰りに徘徊させる
    /// </summary>
    private void GotoNextPoint()
    {
        // 地点がなにも設定されていないときに返します
        if (points.Length == 0)
            return;

        // エージェントが現在設定された目標地点に行くように設定します
        agent.destination = points[destPoint].position;

        // 配列内の次の位置を目標地点に設定し、
        // 必要ならば出発地点にもどります
        destPoint = (destPoint + 1) % points.Length;
    }

    /// <summary>
    /// オンプスライムを切った際の、スタン状態に移行する処理
    /// </summary>
    void Stun()
    {
        IsStun = true;
        if (IsStun) Debug.Log("敵痺れた！");
        //animator.SetTrigger("toStun");
        animator.SetBool("IsStun", true);
        animator.ResetTrigger("toWalk");
        searchArea.SetActive(false);
        SEObj.GetComponent<EnemySEScript>().StunSE();
    }

    /// <summary>
    /// 敵が追跡対象を見つけた
    /// </summary>
    private void Find()
    {
        IsFinded = true;
    }

    /// <summary>
    /// 敵が追跡対象を見失った
    /// </summary>
    private void Lost()
    {
        IsFinded = false;
    }

    /// <summary>
    /// 自身の剣の与ダメ有効・無効を切り替える
    /// 攻撃時のみ与ダメ有効にする
    /// </summary>
    /// <param name="flag">ON・OFF</param>
    private void SwordActive(bool flag)
    {
        Sword.GetComponent<Collider>().enabled = flag;
        Sword.SetActive(flag);
    }

    private void SEStop()
    {
        SEObj.GetComponent<AudioSource>().Stop();
    }
    #endregion
}
