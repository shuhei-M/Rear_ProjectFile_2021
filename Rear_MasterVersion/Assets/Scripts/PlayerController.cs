using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary> �v���C���[�̋������Ǘ�����N���X </summary>
public class PlayerController : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    /// <summary> �ʏ�ړ��p�̕ϐ��Q </summary>
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotationSpeed = 5.0f;

    [SerializeField] GameObject headButt;   // �U���̓����蔻��I�u�W�F�N�g
    [SerializeField] GameObject adObj;   // ���g�̃f�[�^��ێ�����I�u�W�F�N�g
    [SerializeField] Camera myCamera;   // ���C���J����

    [SerializeField] Slider slider;   // �_�f�Q�[�WUI

    /// <summary> �W�����v�ړ��p�̕ϐ��Q </summary>
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float jumpMagnification = 1.0f;

    [SerializeField] GameObject SneakImageObj;   // �B�ꎞ�A���E���Â�����
    [SerializeField] GameObject playerSEObj;
    #endregion

    #region field
    /// <summary> ���g�ɃA�^�b�`���ꂽ�R���|�[�l���g���擾����ϐ��Q </summary>
    NavMeshAgent agent;
    CharacterController charaCon;
    Animator animator;
    AnimatorStateInfo stateInfo;

    float deltaTime = 0f;   // �A�i���O�X�e�B�b�N���X���Ă��鎞��

    Vector3 velocity;   // �ړ��p�x�N�g��

    AvatarData avatarData;   // ���g�̃f�[�^���擾

    /// <summary> �P�����X���C���̓��˗\�����́A�\���E��\���Ɏg�p���邷��ϐ��Q </summary>
    bool smokeDownFlag = false;
    bool smokeUpFlag = true;

    float breatheTime;   // �c��̎_�f��

    public static bool Alive;   // �����Ă��邩�ǂ���

    public static bool IsJump;   // �W�����v�����ǂ���
    private Vector3 moveDirection = Vector3.zero;   // �W�����v����Y�������ւ̓���

    /// <summary> ���ނ�ɉB��Ă��鎞�̎��E�������Â�����ׂ̕ϐ��Q </summary>
    Image SneakImage;
    float fadeSpeed = 0.8f;
    float red, green, blue, alfa;

    PlayerSEScript playerSE;

    bool attackFlag = false;   // �U�������ǂ���
    bool sliderBool = true;   // �X���C�_�[��\�����Ă��邩�ǂ���
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        avatarData = adObj.GetComponent<AvatarData>();

        agent = GetComponent<NavMeshAgent>();
        charaCon = GetComponent<CharacterController>();
        charaCon.enabled = false;

        playerSE = playerSEObj.GetComponent<PlayerSEScript>();

        animator = GetComponent<Animator>();

        breatheTime = 10.0f;

        Alive = true;

        IsJump = false;

        SneakImage = SneakImageObj.GetComponent<Image>();
        red = SneakImage.color.r;
        green = SneakImage.color.g;
        blue = SneakImage.color.b;
        alfa = SneakImage.color.a;

        //agent.autoTraverseOffMeshLink = false;
    }

    // Update is called once per frame
    void Update()
    {
        // �X�^�[�g�R�[�������Ă��邩�|�[�Y��ʂ�\�����Ă�����Update�̏��������Ȃ�
        if (Mathf.Approximately(Time.timeScale, 0f) || !Timer.timeStart)
        {
            return;
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // �B���Ԃł͂Ȃ����A�_�f�Q�[�W�����^���Ȃ�A�_�f�Q�[�W���\���ɂ���
        if (!stateInfo.IsName("Hide") && sliderBool && slider.value == 1)
        {
            slider.gameObject.SetActive(false);
            sliderBool = false;
        }

        // �U���A���S�A�W�����v��ԂłȂ���΁A�ړ����������s
        if (!(stateInfo.IsName("Attack") || stateInfo.IsName("Die") || IsJump))
        {
            MovePlayer();
        }

        // �A�C�h���A�ړ���Ԃł���΁A�������s���B
        if((stateInfo.IsName("Idle") || stateInfo.IsName("Run")) && !IsJump)
        {
            // �A�^�b�N�{�^���������ꂽ�B�U����ԂɈڍs
            if((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 1"))
              && !attackFlag)
            {
                animator.SetTrigger("toAttack");
                playerSE.AtttackSE();
                attackFlag = true;
            }

            // ���E�̈Â��𒲐�
            if (alfa > 0f)
            {
                SneakFadeIn();
            }
        }

        // �A�C�h���A�ړ��A�B���Ԃł���΁A�������s���B
        // �P�����X���C���̓��˗\�����́A�\���E��\���Ɋւ��鏈��
        if ((stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide")) 
        && !IsJump)
        {
            AdjustSmokeOrbit();
        }

        // �U����Ԃł���΁A�U���p�̓����蔻���L���ɂ���
        if (stateInfo.IsName("Attack"))
        {
            headButt.SetActive(true);
            attackFlag = false;
        }
        // �U����ԂłȂ���΁A�U���p�̓����蔻��𖳌��ɂ���
        else
        {
            headButt.SetActive(false);
        }

        // �B���Ԃł���΁A�_�f�Q�[�W�����X�ɍ��B
        if (stateInfo.IsName("Hide"))
        {
            if(!sliderBool)
            {
                slider.gameObject.SetActive(true);
                sliderBool = true;
            }

            if (breatheTime > 0)
            {
                breatheTime -= Time.deltaTime;
            }

            if (alfa < 1f)
            {
                SneakFadeOut();
            }
        }
        // �B���ԂłȂ���΁A�A�_�f�Q�[�W�����X�ɉ񕜂�����B
        else
        {
            if(breatheTime < 10 && Alive)
            {
                breatheTime += Time.deltaTime;
            }
        }
        slider.value = breatheTime / 10.0f;   // �_�f�Q�[�W�ɒl��ۑ�

        // �i�B��Ă��āj�_�f�Q�[�W��0�S�����ꍇ�́A���S
        if(breatheTime < 0)
        {
            animator.SetTrigger("toDie");
            Alive = false;
        }

        // �s��ŁA�X�e�[�W�̉��ɗ����Ă��܂����ꍇ�A�V�[�����ă��[�h
        if(transform.position.y < -50)
        {
            Debug.Log("Out");
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name);
        }

        // �W�����v���ł����
        if (IsJump)
        {
            moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
            charaCon.Move((velocity / jumpMagnification) + (moveDirection * Time.deltaTime));
            if (charaCon.isGrounded)
            {
                IsJump = false;
                agent.Warp(transform.position);
                agent.updatePosition = true;
                charaCon.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �G�̌��ɓ��������ꍇ�́A���S����B
        if (other.gameObject.tag == "Sword")
        {
            Alive = false;

            //���S���A�j���[�V�����̑�֏���
            var ps = GetComponent<ParticleSystem>();
            var ep = new ParticleSystem.EmitParams();
            ep.startColor = Color.red;
            ep.startSize = 0.1f;
            ps.Emit(ep, 1000);

            animator.SetTrigger("toDie");
            playerSE.DieSE();
        }

        // ���ނ�ɓ���i�B���j
        if (other.gameObject.tag == "Grass")
        {
            this.gameObject.tag = "Hidden";
            animator.SetTrigger("toHide");
            Debug.Log("�B��܂��B");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // �i���㑤�Ɏd���܂ꂽ�Z���T�[�ɐG�ꂽ�ꍇ�A�W�����v������
        if (other.gameObject.tag == "JumpPoint" && !IsJump)
        {
            var jumpPointAngle = other.gameObject.transform.rotation.eulerAngles;
            var playerAngle = transform.rotation.eulerAngles;
            var gapAngle = Mathf.Abs(playerAngle.y - jumpPointAngle.y);
            if (gapAngle <= 45)
            {
                Debug.Log(gapAngle);
                Jump();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���ނ炩��o����i�B���̂��~�߂���j
        if (other.gameObject.tag == "Grass")
        {
            this.gameObject.tag = "Player";
            animator.SetTrigger("toRun");
            Debug.Log("�o�܂����B");
        }
    }
    #endregion

    #region public function
    /// <summary>
    /// �c�m�X���C�������{�^�����������ۂ̏���
    /// </summary>
    public void ClickHornButton()
    {
        // �ȉ��̏����ł́A����ȍ~�̏����͍s��Ȃ��B
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;
        if (!avatarData.HornLife) return;

        Debug.Log("�c�m�X���C���I");
        avatarData.SendMessage("HornSkill");
        playerSE.AvatarInstantiateSE();
    }

    /// <summary>
    /// �P�����X���C�������{�^�����������ۂ̏���
    /// </summary>
    public void ClickSmokeButton()
    {
        // �ȉ��̏����ł́A����ȍ~�̏����͍s��Ȃ��B
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;
        if (!avatarData.SmokeLife) return;

        avatarData.SendMessage("SmokeSkill");
        smokeDownFlag = false;
        smokeUpFlag = true;
        playerSE.AvatarInstantiateSE();
    }

    /// <summary>
    /// �I���v�X���C�������{�^�����������ۂ̏���
    /// </summary>
    public void ClickNoteButton()
    {
        // �ȉ��̏����ł́A����ȍ~�̏����͍s��Ȃ��B
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;

        Debug.Log("�I���v�X���C���I");
        avatarData.SendMessage("NoteSkill");
        playerSE.AvatarInstantiateSE();
    }

    /// <summary>
    /// �v���C���[�����W�����v������B
    /// �i���㑤�Ɏd���܂ꂽ�Z���T�[�ɐG�ꂽ�ۂɌĂ΂��
    /// </summary>
    public void Jump()
    {
        Debug.Log("Jump");
        charaCon.enabled = true;
        IsJump = true;
        agent.updatePosition = false;
        moveDirection.y = jumpSpeed;
        playerSE.JumpSE();
    }
    #endregion

    #region private function
    /// <summary>
    /// ���͂���Ƀv���C���[���ړ�������
    /// </summary>
    void MovePlayer()
    {
        var horizontalRotation = Quaternion.AngleAxis(myCamera.transform.eulerAngles.y, Vector3.up);

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");

        Vector3 moveDelta = new Vector3(xDelta, 0, zDelta);
        //velocity = moveDelta.normalized * moveSpeed * Time.deltaTime;
        velocity = horizontalRotation * moveDelta.normalized * moveSpeed * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            deltaTime += Time.deltaTime;
        }
        else
        {
            deltaTime = 0f;
        }

        animator.SetFloat("DeltaTime", deltaTime);

        agent.Move(velocity);

        if (velocity.magnitude > 0)
        {
            Quaternion look = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed);
            headButt.transform.rotation =
                Quaternion.Slerp(headButt.transform.rotation, look, rotationSpeed);
        }
    }

    /// <summary>
    /// �P�����X���C���̓��˗\�����́A�\���E��\���Ɋւ��鏈��
    /// </summary>
    void AdjustSmokeOrbit()
    {
        // �X�L��UI�ŃP�����X���C����I�����Ă���ꍇ
        if (EventSystem.current.currentSelectedGameObject != null
            && EventSystem.current.currentSelectedGameObject.name == "Smoke"
            && avatarData.SmokeLife)
        {
            // ���傤�ǑI�������΂���ł���΁A���˗\�����𐶐�
            if (!smokeDownFlag && smokeUpFlag)
            {
                Debug.Log("�O����\�����܂��B");
                avatarData.SendMessage("SmokeOrbitInstantiate");
                smokeDownFlag = true;
                smokeUpFlag = false;
            }
            // �I�𒆂ł���΁A���˗\�����̊p�x�𒲐�
            else if (smokeDownFlag && !smokeUpFlag)
            {
                avatarData.SendMessage("SmokeOrbit");
            }
        }
        // �X�L��UI�ŃP�����X���C�����I������Ă��Ȃ��ꍇ
        else if (smokeDownFlag && avatarData.SmokeLife)
        {
            avatarData.SendMessage("DestroySmokeOrbit");
            smokeDownFlag = false;
            smokeUpFlag = true;
        }
    }

    /// <summary>
    /// ���ނ�o���ۂɎ��s����
    /// ���E�̖��邳�����X�Ɍ��Ɍ��ɖ߂�
    /// </summary>
    void SneakFadeOut()
    {
        alfa += Time.deltaTime / fadeSpeed;
        SneakImage.color = new Color(red, green, blue, alfa);
    }

    /// <summary>
    /// ���ނ�ɉB��Ă���Œ��Ɏ��s����
    /// ���E�����X�ɏ����Â�����
    /// </summary>
    void SneakFadeIn()
    {
        alfa -= Time.deltaTime / fadeSpeed;
        SneakImage.color = new Color(red, green, blue, alfa);
    }
    #endregion
}