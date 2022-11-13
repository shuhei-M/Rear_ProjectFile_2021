using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotationSpeed = 5.0f;
    [SerializeField] GameObject headButt;
    [SerializeField] GameObject adObj;
    [SerializeField] Camera myCamera;

    AvatarData avatarData;
    Vector3 velocity;
    NavMeshAgent agent;
    CharacterController charaCon;

    Animator animator;
    AnimatorStateInfo stateInfo;
    float deltaTime = 0f;

    bool smokeDownFlag = false;
    bool smokeUpFlag = true;

    //[SerializeField] GameObject breatheText;
    float breatheTime;

    [SerializeField] Slider slider;

    public static bool Alive;

    public static bool IsJump;
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float jumpMagnification = 1.0f;
    [SerializeField] GameObject SneakImageObj;
    [SerializeField] GameObject playerSEObj;
    Image SneakImage;
    PlayerSEScript playerSE;
    private Vector3 moveDirection = Vector3.zero;
    float fadeSpeed = 0.8f;
    float red, green, blue, alfa;

    bool attackFlag = false;
    bool sliderBool = true;

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
        // スタートコールをしているかポーズ画面を表示していたらUpdateの処理をしない
        if (Mathf.Approximately(Time.timeScale, 0f) || !Timer.timeStart)
        {
            return;
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Hide") && sliderBool && slider.value == 1)
        {
            slider.gameObject.SetActive(false);
            sliderBool = false;
        }

        if (!(stateInfo.IsName("Attack") || stateInfo.IsName("Die") || IsJump))
        {
            MovePlayer();
            //agent.SetDestination(transform.position + velocity);
            //Debug.Log(agent.isOnOffMeshLink);
        }

        if((stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide")) 
        && !IsJump)
        {
            if ((stateInfo.IsName("Idle") || stateInfo.IsName("Run"))
              && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 1")) 
              && !attackFlag)
            {
                animator.SetTrigger("toAttack");
                playerSE.AtttackSE();
                attackFlag = true;
            }
            //if (/*Input.GetAxis("Attack") > 0*/
            //    Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 1"))
            //{
            //    animator.SetTrigger("toAttack");
            //    playerSE.AtttackSE();
            //}

            if ((stateInfo.IsName("Idle") || stateInfo.IsName("Run"))
                && alfa > 0f)
            {
                SneakFadeIn();
            }

            if (EventSystem.current.currentSelectedGameObject != null
                && EventSystem.current.currentSelectedGameObject.name == "Smoke"
                && avatarData.SmokeLife)
            {
                if (!smokeDownFlag && smokeUpFlag)
                {
                    Debug.Log("軌道を表示します。");
                    avatarData.SendMessage("SmokeOrbitInstantiate");
                    smokeDownFlag = true;
                    smokeUpFlag = false;
                }
                else if (smokeDownFlag && !smokeUpFlag)
                {
                    avatarData.SendMessage("SmokeOrbit");
                }
            }
            else if(smokeDownFlag && avatarData.SmokeLife)
            {
                avatarData.SendMessage("DestroySmokeOrbit");
                smokeDownFlag = false;
                smokeUpFlag = true;
            }
        }

        if (stateInfo.IsName("Attack"))
        {
            headButt.SetActive(true);
            attackFlag = false;
        }
        else
        {
            headButt.SetActive(false);
        }

        if(stateInfo.IsName("Hide"))
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

            //if (smokeDownFlag && !smokeUpFlag)
            //{
            //    avatarData.SendMessage("DestroySmokeOrbit");
            //    smokeDownFlag = false;
            //    smokeUpFlag = true;
            //}
        }
        else
        {
            if(breatheTime < 10 && Alive)
            {
                breatheTime += Time.deltaTime;
            }
        }
        slider.value = breatheTime / 10.0f;
        //int t = Mathf.FloorToInt(breatheTime);
        //Text uiText = breatheText.GetComponent<Text>();
        //uiText.text = "";

        if(breatheTime < 0)
        {
            animator.SetTrigger("toDie");
            Alive = false;
        }
        //agent.SetDestination(transform.position + velocity);
        //Debug.Log(agent.isOnOffMeshLink);

        if(transform.position.y < -50)
        {
            Debug.Log("Out");
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name);
        }

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

        //agent.SetDestination(transform.position + velocity);
        //Debug.Log(agent.isOnOffMeshLink);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sword")
        {
            Alive = false;

            //死亡時アニメーションの代替処理
            var ps = GetComponent<ParticleSystem>();
            var ep = new ParticleSystem.EmitParams();
            ep.startColor = Color.red;
            ep.startSize = 0.1f;
            ps.Emit(ep, 1000);

            animator.SetTrigger("toDie");
            playerSE.DieSE();
        }

        if (other.gameObject.tag == "Grass")
        {
            this.gameObject.tag = "Hidden";
            animator.SetTrigger("toHide");
            Debug.Log("隠れます。");
        }

        //if (other.gameObject.tag == "JumpPoint")
        //{
        //    Jump();
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "JumpPoint" && !IsJump)
        {
            var jumpPointAngle = other.gameObject.transform.rotation.eulerAngles;
            var playerAngle = transform.rotation.eulerAngles;
            var gapAngle = Mathf.Abs(playerAngle.y - jumpPointAngle.y);
            if(gapAngle <= 45)
            {
                Debug.Log(gapAngle);
                Jump();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Grass")
        {
            this.gameObject.tag = "Player";
            animator.SetTrigger("toRun");
            Debug.Log("出ました。");
        }
    }

    public void ClickHornButton()
    {
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;
        if (!avatarData.HornLife) return;

        Debug.Log("ツノスライム！");
        avatarData.SendMessage("HornSkill");
        playerSE.AvatarInstantiateSE();
    }

    public void ClickSmokeButton()
    {
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;
        if (!avatarData.SmokeLife) return;

        avatarData.SendMessage("SmokeSkill");
        smokeDownFlag = false;
        smokeUpFlag = true;
        playerSE.AvatarInstantiateSE();
    }

    public void ClickNoteButton()
    {
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;

        Debug.Log("オンプスライム！");
        avatarData.SendMessage("NoteSkill");
        playerSE.AvatarInstantiateSE();
    }

    public void Jump()
    {
        Debug.Log("Jump");
        charaCon.enabled = true;
        IsJump = true;
        agent.updatePosition = false;
        moveDirection.y = jumpSpeed;
        playerSE.JumpSE();
    }

    void SneakFadeOut()
    {
        alfa += Time.deltaTime / fadeSpeed;
        SneakImage.color = new Color(red, green, blue, alfa);
    }

    void SneakFadeIn()
    {
        alfa -= Time.deltaTime / fadeSpeed;
        SneakImage.color = new Color(red, green, blue, alfa);
    }
}