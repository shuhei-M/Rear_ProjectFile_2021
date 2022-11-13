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
    const float statusUpRatio = 0.4f;   // �G���������鎞�̑����̊���

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

        // �G���㔼�ɋ���
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

    //--- StayMode�i���݃��[�h�j�p�̃A�b�v�f�[�g�֐� ---//
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

        // �X�^����ԂŖ�����Ώ�ɒT�m�ł���悤�ɂ���
        // ��������Ȃ���΁A�X�^�����Ɍ������G�����ɒǂ������n�߂Ă��܂�
        if (!stateInfo.IsName("Stun"))
        {
            if (avatarIsFinded)
            {   //���g�𔭌�������D��I�ɒǐՏ�Ԃɐ؂�ւ�
                tracked = avatar;
            }
            else if (IsFinded)
            {   //�v���C���[�𔭌�������ǐՏ�Ԃɐ؂�ւ�
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

        //--- �ҋ@��ԁiIdle�X�e�[�g�j ---//
        if (stateInfo.IsName("Idle"))
        {
            agent.isStopped = true;

            if(!IsSetMoveRotation)
            {
                transform.position = points[points.Length - 1].position;
                var v=transform.rotation.eulerAngles;
                var speed = Mathf.Abs(startAngle.y - v.y);
                step = speed * Time.deltaTime / 2;   //2�b������
                IsSetMoveRotation = true;
                CanMoveRotation = true;
            }

            if(CanMoveRotation)
            {
                //�w�肵�������ɂ�������]����ꍇ
                transform.rotation = Quaternion.RotateTowards
                    (transform.rotation,
                    Quaternion.Euler(0, startAngle.y, 0), step);
            }

            //if (!isSmoked && (avatarIsFinded || IsFinded) && !IsStun/*stateInfo.IsName("Stun")*/)
            //{   
            //    // �ǐՃ^�[�Q�b�g������ΒǐՏ�Ԃɐ؂�ւ�
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
        //--- �ǐՏ�ԁiRun�X�e�[�g�j ---//
        else if (stateInfo.IsName("Run"))
        {
            agent.isStopped = false;

            if (tracked != null)
            {
                //�v���C���[������������A�H��Ԃɐ؂�ւ�
                if ((isSmoked || (!IsFinded && !avatarIsFinded)) && tracked.tag != "NoteAvatar")
                {
                    animator.SetTrigger("toWalk");
                    agent.speed = walkSpeed;
                }

                agent.destination = tracked.transform.position;
                quotationMark.SetActive(true);

                float distance = Vector3.Distance(transform.position, tracked.transform.position); //��

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
        //--- �A�H��ԁiWalk�X�e�[�g�j ---//
        else if(stateInfo.IsName("Walk"))
        {
            agent.isStopped = false;

            //agent.destination = startPoint.position;
            agent.destination = points[points.Length - 1].position;

            if (!isSmoked && (avatarIsFinded || IsFinded))
            {
                // �ǐՃ^�[�Q�b�g������ΒǐՏ�Ԃɐ؂�ւ�
                animator.SetTrigger("toRun");
                agent.speed = runSpeed;
            }

            //�A�҂�����
            if (agent.remainingDistance < 0.1f/*transform.position == points[points.Length - 1].position*/)
            {
                IsSetMoveRotation = false;
                //�A�҂�����ҋ@��Ԃɐ؂�ւ�
                animator.SetTrigger("toIdle");
            }
        }
        //--- ���S��ԁiDie�X�e�[�g�j ---//
        else if (stateInfo.IsName("Die"))
        {
            //agent.isStopped = true;
            if (Alive)
            {
                agent.isStopped = true;
                StartAndEndGame.EnemyLife--;
                Text uiText = lifeText.GetComponent<Text>();
                uiText.text = " �~ " + StartAndEndGame.EnemyLife;
                agent.enabled = false;
            }
            Alive = false;
        }
        //--- �X�^����ԁiStun�X�e�[�g�j---//
        else if (stateInfo.IsName("Stun"))
        {
            agent.isStopped = true;
            if (stunTime < 5f)
            {
                stunTime += Time.deltaTime;
            }
            else
            {   // 5�b�o�߂ŃX�^������
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
        //--- �U����ԁiAttack�X�e�[�g�j ---//
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

    //--- PatrolMode�i���񃂁[�h�j�p�̃A�b�v�f�[�g�֐� ---//
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

        // �X�^����ԂŖ�����Ώ�ɒT�m�ł���悤�ɂ���
        // ��������Ȃ���΁A�X�^�����Ɍ������G�����ɒǂ������n�߂Ă��܂�
        if (!stateInfo.IsName("Stun"))
        {
            if (avatarIsFinded)
            {   //���g�𔭌�������D��I�ɒǐՏ�Ԃɐ؂�ւ�
                tracked = avatar;
            }
            else if (IsFinded)
            {   //�v���C���[�𔭌�������ǐՏ�Ԃɐ؂�ւ�
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

        //--- �p�j��ԁiWalk�X�e�[�g�j ---//
        if (stateInfo.IsName("Walk"))
        {
            agent.isStopped = false;

            //if (!isSmoked && (avatarIsFinded || IsFinded) && !IsStun/*stateInfo.IsName("Stun")*/)
            //{   // �ǐՃ^�[�Q�b�g������ΒǐՏ�Ԃɐ؂�ւ�
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

            // �G�[�W�F���g�����ڕW�n�_�ɋ߂Â��Ă�����A
            // ���̖ڕW�n�_��I�����܂�
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GotoNextPoint();
            }
        }

        //--- �ǐՏ�ԁiRun�X�e�[�g�j ---//
        if (stateInfo.IsName("Run"))
        {
            if (tracked != null)
            {
                //�v���C���[������������p�j��Ԃɐ؂�ւ�
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

                    float distance = Vector3.Distance(transform.position, tracked.transform.position); //��

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

        //--- ���S��ԁiDie�X�e�[�g�j ---//
        if (stateInfo.IsName("Die"))
        {
            //agent.isStopped = true;
            if (Alive)
            {
                agent.isStopped = true;
                StartAndEndGame.EnemyLife--;
                Text uiText = lifeText.GetComponent<Text>();
                uiText.text = " �~ " + StartAndEndGame.EnemyLife;
                agent.enabled = false;
            }
            Alive = false;
        }

        //--- �X�^����ԁiStun�X�e�[�g�j---//
        if (stateInfo.IsName("Stun"))
        {
            if (stunTime < 5f)
            {
                stunTime += Time.deltaTime;
            }
            else
            {   // 5�b�o�߂ŃX�^������
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

        //--- �U����ԁiAttack�X�e�[�g�j ---//
        if (stateInfo.IsName("Attack"))
        {
            agent.isStopped = true;
            Sword.SetActive(true);
        }
        else
        {
            Sword.SetActive(false);
        }

        //--- �ҋ@��ԁiIdle�X�e�[�g�j ---//
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
        // �n�_���Ȃɂ��ݒ肳��Ă��Ȃ��Ƃ��ɕԂ��܂�
        if (points.Length == 0)
            return;

        // �G�[�W�F���g�����ݐݒ肳�ꂽ�ڕW�n�_�ɍs���悤�ɐݒ肵�܂�
        agent.destination = points[destPoint].position;

        // �z����̎��̈ʒu��ڕW�n�_�ɐݒ肵�A
        // �K�v�Ȃ�Ώo���n�_�ɂ��ǂ�܂�
        destPoint = (destPoint + 1) % points.Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Head")
        {
            if (!stateInfo.IsName("Die"))
            {
                //�@��l���̕���
                var headDirection = other.transform.position - transform.position;
                //�@�G�̑O������̎�l���̕���
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
        if(IsStun) Debug.Log("�GჂꂽ�I");
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
