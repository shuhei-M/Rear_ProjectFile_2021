using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    AnimatorStateInfo stateInfo;

    [SerializeField] bool IsPatrolMode;

    [SerializeField] float searchAngle = 120f;

    [SerializeField] Transform[] points;
    private int destPoint = 0;

    GameObject searchArea;

    GameObject player;
    GameObject avatar;
    GameObject tracked;
    [SerializeField] float stopDistance = 1.5f;
    [SerializeField] GameObject Sword;

    [SerializeField] GameObject quotationMark;

    float stunTime = 0f;

    bool Alive;
    [SerializeField] GameObject lifeText;

    [SerializeField] GameObject SEObj;

    const float walkSpeed = 1.5f;
    float runSpeed = 3.0f;
    const float statusUpRatio = 0.4f;   // 敵が強化する時の総数の割合

    int ollEnemyLife;

    bool IsFinded;
    bool avatarIsFinded;

    bool isSmoked;

    bool fullEnemyCountFlag = false;
    bool powerUpFlag = false;

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

    float attackStateFrame = 0.0f;

    bool IsStun = false;

    bool IsSetMoveRotation = true;
    bool CanMoveRotation = false;
    float step;
    Vector3 startAngle;

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
            PatrolMode_Update();
        }
        else
        {
            StayMode_Update();
        }
    }

    //--- StayMode（駐在モード）用のアップデート関数 ---//
    void StayMode_Update()
    {   
        if(!stateInfo.IsName("Run")) quotationMark.SetActive(false);

        if (!stateInfo.IsName("Attack"))
        {
            SwordActive(false);
            if(attackStateFrame != 0.0f)
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

            if(!IsSetMoveRotation)
            {
                transform.position = points[points.Length - 1].position;
                var v=transform.rotation.eulerAngles;
                var speed = Mathf.Abs(startAngle.y - v.y);
                step = speed * Time.deltaTime / 2;   //2秒かける
                IsSetMoveRotation = true;
                CanMoveRotation = true;
            }

            if(CanMoveRotation)
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
        else if(stateInfo.IsName("Walk"))
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

    //--- PatrolMode（巡回モード）用のアップデート関数 ---//
    void PatrolMode_Update()
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
                if(!isSmoked || tracked.tag == "NoteAvatar")
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

    void GotoNextPoint()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Head")
        {
            if (!stateInfo.IsName("Die"))
            {
                //　主人公の方向
                var headDirection = other.transform.position - transform.position;
                //　敵の前方からの主人公の方向
                var angle = Vector3.Angle(transform.forward, headDirection);

                if (angle >= searchAngle)
                {
                    Debug.Log("Hit!");
                    animator.SetTrigger("toDie");
                    SEObj.GetComponent<EnemySEScript>().DieSE();
                    Invoke("SEStop", 1.1f);
                }
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

    void Stun()
    {
        IsStun = true;
        if(IsStun) Debug.Log("敵痺れた！");
        //animator.SetTrigger("toStun");
        animator.SetBool("IsStun", true);
        animator.ResetTrigger("toWalk");
        searchArea.SetActive(false);
        SEObj.GetComponent<EnemySEScript>().StunSE();
    }

    void Find()
    {
        IsFinded = true;
    }

    void Lost()
    {
        IsFinded = false;
    }

    void SwordActive(bool flag)
    {
        Sword.GetComponent<Collider>().enabled = flag;
        Sword.SetActive(flag);
    }

    void SEStop()
    {
        SEObj.GetComponent<AudioSource>().Stop();
    }
}
