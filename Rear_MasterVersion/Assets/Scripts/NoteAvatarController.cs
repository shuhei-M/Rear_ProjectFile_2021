using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> DeadEffect()関数のみ追加 </summary>

public class NoteAvatarController : MonoBehaviour
{
    Animator animator;
    AnimatorStateInfo stateInfo;
    AvatarData avatarData;
    float soundTime = 0f;
    bool soundLife = true;
    bool dieFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        avatarData = GameObject.Find("AvatarDataObj").GetComponent<AvatarData>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (soundLife)
        {
            if (!avatarData.NoteSoundFlag)
            {
                if (soundTime <= 5f)
                {
                    soundTime += Time.deltaTime;
                }
                else
                {
                    soundTime = 0;
                    soundLife = false;
                }
            }
        }

        animator.SetFloat("SoundTime", soundTime);

        if (dieFlag)
        {
            if (transform.localScale.y >= 0)
            {
                transform.localScale -= new Vector3(0.0f, 0.01f, 0.0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sword")
        {
            var Enemy = other.transform.root.GetChild(0);
            var ec = Enemy.GetComponent<EnemyController>();

            // 攻撃してきた敵をスタン状態にする
            ec.SendMessage("Stun");
            ec.Avatar = null;
            // 倒しに来ていた敵全てのターゲティングをリセット
            var soundArea = transform.Find("SoundArea");
            soundArea.gameObject.SendMessage("EnemyTargetReset");

            // SoundAreaの判定を消して音を鳴らすのをやめる
            soundArea.gameObject.SendMessage("NoSounding");

            //死亡時アニメーションの代替処理
            DeadEffect();

            animator.SetTrigger("toDie");
            Destroy(this.gameObject, 3.0f);

            dieFlag = true;
        }
    }

    public void NoteDieOperation()
    {
        // 倒しに来ていた敵全てのターゲティングをリセット
        transform.Find("SoundArea").gameObject.SendMessage("EnemyTargetReset");
    }

    /// <summary>
    /// 制作者：松島
    /// 敵に斬られ死亡した際に、赤いエミッションをまき散らす
    /// </summary>
    private void DeadEffect()
    {
        //死亡時アニメーションの代替処理
        var ps = GetComponent<ParticleSystem>();
        var ep = new ParticleSystem.EmitParams();
        ep.startColor = Color.red;
        ep.startSize = 0.1f;
        ps.Emit(ep, 1000);
    }
}
