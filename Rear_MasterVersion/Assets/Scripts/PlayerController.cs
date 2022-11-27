using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary> プレイヤーの挙動を管理するクラス </summary>
public class PlayerController : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    /// <summary> 通常移動用の変数群 </summary>
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotationSpeed = 5.0f;

    [SerializeField] GameObject headButt;   // 攻撃の当たり判定オブジェクト
    [SerializeField] GameObject adObj;   // 分身のデータを保持するオブジェクト
    [SerializeField] Camera myCamera;   // メインカメラ

    [SerializeField] Slider slider;   // 酸素ゲージUI

    /// <summary> ジャンプ移動用の変数群 </summary>
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float jumpMagnification = 1.0f;

    [SerializeField] GameObject SneakImageObj;   // 隠れ時、視界を暗くする
    [SerializeField] GameObject playerSEObj;
    #endregion

    #region field
    /// <summary> 自身にアタッチされたコンポーネントを取得する変数群 </summary>
    NavMeshAgent agent;
    CharacterController charaCon;
    Animator animator;
    AnimatorStateInfo stateInfo;

    float deltaTime = 0f;   // アナログスティックを傾けている時間

    Vector3 velocity;   // 移動用ベクトル

    AvatarData avatarData;   // 分身のデータを取得

    /// <summary> ケムリスライムの投射予測線の、表示・非表示に使用するする変数群 </summary>
    bool smokeDownFlag = false;
    bool smokeUpFlag = true;

    float breatheTime;   // 残りの酸素量

    public static bool Alive;   // 生きているかどうか

    public static bool IsJump;   // ジャンプ中かどうか
    private Vector3 moveDirection = Vector3.zero;   // ジャンプ中のY軸方向への動き

    /// <summary> 草むらに隠れている時の視界を少し暗くする為の変数群 </summary>
    Image SneakImage;
    float fadeSpeed = 0.8f;
    float red, green, blue, alfa;

    PlayerSEScript playerSE;

    bool attackFlag = false;   // 攻撃中かどうか
    bool sliderBool = true;   // スライダーを表示しているかどうか
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
        // スタートコールをしているかポーズ画面を表示していたらUpdateの処理をしない
        if (Mathf.Approximately(Time.timeScale, 0f) || !Timer.timeStart)
        {
            return;
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 隠れ状態ではない且つ、酸素ゲージが満タンなら、酸素ゲージを非表示にする
        if (!stateInfo.IsName("Hide") && sliderBool && slider.value == 1)
        {
            slider.gameObject.SetActive(false);
            sliderBool = false;
        }

        // 攻撃、死亡、ジャンプ状態でなければ、移動処理を実行
        if (!(stateInfo.IsName("Attack") || stateInfo.IsName("Die") || IsJump))
        {
            MovePlayer();
        }

        // アイドル、移動状態であれば、処理を行う。
        if((stateInfo.IsName("Idle") || stateInfo.IsName("Run")) && !IsJump)
        {
            // アタックボタンが押された。攻撃状態に移行
            if((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 1"))
              && !attackFlag)
            {
                animator.SetTrigger("toAttack");
                playerSE.AtttackSE();
                attackFlag = true;
            }

            // 視界の暗さを調節
            if (alfa > 0f)
            {
                SneakFadeIn();
            }
        }

        // アイドル、移動、隠れ状態であれば、処理を行う。
        // ケムリスライムの投射予測線の、表示・非表示に関する処理
        if ((stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide")) 
        && !IsJump)
        {
            AdjustSmokeOrbit();
        }

        // 攻撃状態であれば、攻撃用の当たり判定を有効にする
        if (stateInfo.IsName("Attack"))
        {
            headButt.SetActive(true);
            attackFlag = false;
        }
        // 攻撃状態でなければ、攻撃用の当たり判定を無効にする
        else
        {
            headButt.SetActive(false);
        }

        // 隠れ状態であれば、酸素ゲージを徐々に削る。
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
        // 隠れ状態でなければ、、酸素ゲージを徐々に回復させる。
        else
        {
            if(breatheTime < 10 && Alive)
            {
                breatheTime += Time.deltaTime;
            }
        }
        slider.value = breatheTime / 10.0f;   // 酸素ゲージに値を保存

        // （隠れていて）酸素ゲージが0担った場合は、死亡
        if(breatheTime < 0)
        {
            animator.SetTrigger("toDie");
            Alive = false;
        }

        // 不具合で、ステージの下に落ちてしまった場合、シーンを再ロード
        if(transform.position.y < -50)
        {
            Debug.Log("Out");
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name);
        }

        // ジャンプ中であれば
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
        // 敵の件に当たった場合は、死亡する。
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

        // 草むらに入る（隠れる）
        if (other.gameObject.tag == "Grass")
        {
            this.gameObject.tag = "Hidden";
            animator.SetTrigger("toHide");
            Debug.Log("隠れます。");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 段差上側に仕込まれたセンサーに触れた場合、ジャンプさせる
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
        // 草むらから出たら（隠れるのを止めたら）
        if (other.gameObject.tag == "Grass")
        {
            this.gameObject.tag = "Player";
            animator.SetTrigger("toRun");
            Debug.Log("出ました。");
        }
    }
    #endregion

    #region public function
    /// <summary>
    /// ツノスライム生成ボタンを押した際の処理
    /// </summary>
    public void ClickHornButton()
    {
        // 以下の条件では、それ以降の処理は行わない。
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;
        if (!avatarData.HornLife) return;

        Debug.Log("ツノスライム！");
        avatarData.SendMessage("HornSkill");
        playerSE.AvatarInstantiateSE();
    }

    /// <summary>
    /// ケムリスライム生成ボタンを押した際の処理
    /// </summary>
    public void ClickSmokeButton()
    {
        // 以下の条件では、それ以降の処理は行わない。
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;
        if (!avatarData.SmokeLife) return;

        avatarData.SendMessage("SmokeSkill");
        smokeDownFlag = false;
        smokeUpFlag = true;
        playerSE.AvatarInstantiateSE();
    }

    /// <summary>
    /// オンプスライム生成ボタンを押した際の処理
    /// </summary>
    public void ClickNoteButton()
    {
        // 以下の条件では、それ以降の処理は行わない。
        if (!(stateInfo.IsName("Idle") || stateInfo.IsName("Run") || stateInfo.IsName("Hide"))) return;
        if (IsJump) return;

        Debug.Log("オンプスライム！");
        avatarData.SendMessage("NoteSkill");
        playerSE.AvatarInstantiateSE();
    }

    /// <summary>
    /// プレイヤーををジャンプさせる。
    /// 段差上側に仕込まれたセンサーに触れた際に呼ばれる
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
    /// 入力を基にプレイヤーを移動させる
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
    /// ケムリスライムの投射予測線の、表示・非表示に関する処理
    /// </summary>
    void AdjustSmokeOrbit()
    {
        // スキルUIでケムリスライムを選択している場合
        if (EventSystem.current.currentSelectedGameObject != null
            && EventSystem.current.currentSelectedGameObject.name == "Smoke"
            && avatarData.SmokeLife)
        {
            // ちょうど選択したばかりであれば、投射予測線を生成
            if (!smokeDownFlag && smokeUpFlag)
            {
                Debug.Log("軌道を表示します。");
                avatarData.SendMessage("SmokeOrbitInstantiate");
                smokeDownFlag = true;
                smokeUpFlag = false;
            }
            // 選択中であれば、投射予測線の角度を調整
            else if (smokeDownFlag && !smokeUpFlag)
            {
                avatarData.SendMessage("SmokeOrbit");
            }
        }
        // スキルUIでケムリスライムが選択されていない場合
        else if (smokeDownFlag && avatarData.SmokeLife)
        {
            avatarData.SendMessage("DestroySmokeOrbit");
            smokeDownFlag = false;
            smokeUpFlag = true;
        }
    }

    /// <summary>
    /// 草むら出た際に実行する
    /// 視界の明るさを徐々に元に元に戻す
    /// </summary>
    void SneakFadeOut()
    {
        alfa += Time.deltaTime / fadeSpeed;
        SneakImage.color = new Color(red, green, blue, alfa);
    }

    /// <summary>
    /// 草むらに隠れている最中に実行する
    /// 視界を徐々に少し暗くする
    /// </summary>
    void SneakFadeIn()
    {
        alfa -= Time.deltaTime / fadeSpeed;
        SneakImage.color = new Color(red, green, blue, alfa);
    }
    #endregion
}