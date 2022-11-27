using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> DeadEffect()�֐��̂ݒǉ� </summary>

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

            // �U�����Ă����G���X�^����Ԃɂ���
            ec.SendMessage("Stun");
            ec.Avatar = null;
            // �|���ɗ��Ă����G�S�Ẵ^�[�Q�e�B���O�����Z�b�g
            var soundArea = transform.Find("SoundArea");
            soundArea.gameObject.SendMessage("EnemyTargetReset");

            // SoundArea�̔���������ĉ���炷�̂���߂�
            soundArea.gameObject.SendMessage("NoSounding");

            //���S���A�j���[�V�����̑�֏���
            DeadEffect();

            animator.SetTrigger("toDie");
            Destroy(this.gameObject, 3.0f);

            dieFlag = true;
        }
    }

    public void NoteDieOperation()
    {
        // �|���ɗ��Ă����G�S�Ẵ^�[�Q�e�B���O�����Z�b�g
        transform.Find("SoundArea").gameObject.SendMessage("EnemyTargetReset");
    }

    /// <summary>
    /// ����ҁF����
    /// �G�Ɏa��ꎀ�S�����ۂɁA�Ԃ��G�~�b�V�������܂��U�炷
    /// </summary>
    private void DeadEffect()
    {
        //���S���A�j���[�V�����̑�֏���
        var ps = GetComponent<ParticleSystem>();
        var ep = new ParticleSystem.EmitParams();
        ep.startColor = Color.red;
        ep.startSize = 0.1f;
        ps.Emit(ep, 1000);
    }
}
