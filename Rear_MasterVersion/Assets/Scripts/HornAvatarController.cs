using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary> �c�m�X���C���̋������Ǘ�����N���X�i�����I�ɒS���j </summary>
/// <summary> �ړ����@�E�Փˌ��m���@��ύX�B</summary>
/// <summary> ���S���Ԃ��G�~�b�V��������@�\�E�W�����v�@�\��ǉ��B </summary>
public class HornAvatarController : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    /// <summary> (����)�W�����v����ۂ̃p�����[�^�ϐ��Q </summary>
    [SerializeField] float jumpSpeed = 5.0f;   // �W�����v�̏�����iy���j�ւ̏����x
    [SerializeField] float gravity = 20.0f;   // �d��
    [SerializeField] float jumpMagnification = 1.1f;   // �W�����v��
    #endregion

    #region field
    /// <summary> ���ȉ��A�������ǉ�</summary>
    /// <summary> ���g�ɃA�^�b�`���ꂽ�R���|�[�l���g���擾����ϐ��Q </summary>
    CharacterController charaCon;
    Animator animator;
    AnimatorStateInfo stateInfo;
    NavMeshAgent agent;

    GameObject hornStopper;   // ���ꂪ�ǂɂԂ������ہA�c�m�̓������~�߂�

    bool IsJump = false;   // �W�����v�����ǂ���
    private Vector3 moveDirection = Vector3.zero;   // �W�����v���̏�����ւ̓���

    Vector3 GoStraight;   // �c�m�X���C�������i����ۂ̌���

    Vector3 v = Vector3.zero;   // �������p�̃x�N�g��
    /// <summary> ���܂ŁA�������ǉ� </summary>

    List<GameObject> hateEnemy;
    float deltaTime = 0f;
    float dieCountDown = 0f;
    float moveSpeed = 2.0f;
    bool stop = false;
    bool dieFlag = false;
    #endregion

    #region property
    public GameObject HateEnemy
    {   // �ǐՂ��Ă����G��ۊ�
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
        /// <summary> ���ȉ��A�������ǉ� </summary>
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
        /// <summary> ���܂ŁA�������ǉ� </summary>

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
        //// ���̒��ɓ�������T�m����Ȃ��Ȃ�
        //if (other.gameObject.tag == "SmokeArea")
        //{
        //    EnemyTargetReset();
        //    this.tag = "Untagged";
        //}

        if (other.gameObject.tag == "Sword")
        {   // �U�����ꂽ�Ƃ��ɏ���
            var Enemy = other.transform.root.GetChild(0);
            var ec = Enemy.GetComponent<EnemyController>();

            EnemyTargetReset();

            /// <summary> �ȉ��A�������ǉ� </summary>
            //���S���A�j���[�V�����̑�֏���
            var ps = GetComponent<ParticleSystem>();
            var ep = new ParticleSystem.EmitParams();
            ep.startColor = Color.red;
            ep.startSize = 0.1f;
            ps.Emit(ep, 1000);

            animator.SetTrigger("toDie");
            Debug.Log("�c�m���ꂽ�I");
            Destroy(this.gameObject, 0.5f);
            Destroy(hornStopper);
            dieFlag = true;
            GoStraight = v;
            agent.isStopped = true;
        }

        /// <summary> (����)�i���㑤�̃Z���T�[�ɐG�ꂽ </summary>
        if (other.gameObject.tag == "JumpPoint")
        {
            Debug.Log("�c�m��т܂��I");

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
    /// ����ҁF����
    /// �c�m�X���C�����W�����v������B
    /// �i���㑤�Ɏd���܂ꂽ�Z���T�[�ɐG�ꂽ�ۂɌĂ΂��
    /// </summary>
    public void Jump()
    {
        Debug.Log("Jump");
        charaCon.enabled = true;   // �ړ����@���L�����N�^�[�R���g���[���[�ɐ؂�ւ���
        IsJump = true;
        agent.updatePosition = false;
        moveDirection.y = jumpSpeed;
        //playerSE.JumpSE();
    }
    #endregion

    #region private function
    /// <summary>
    /// ����ҁF����
    /// �c�m�X���C�����ǂɂԂ���A���i���~�߂�B
    /// </summary>
    private void Stop()
    {
        Debug.Log("�c�m�ՓˁI");
        stop = true;
    }
    #endregion
}
