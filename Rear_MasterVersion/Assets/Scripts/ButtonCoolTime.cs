using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCoolTime : MonoBehaviour
{
    [SerializeField] GameObject objButton;	// Image�{Button�̃I�u�W�F�N�g���A�T�C��
    [SerializeField] Text lblText;			// �c�莞�Ԃ�\������Text�I�u�W�F�N�g���A�T�C��
    [SerializeField] int count;
    [SerializeField] bool note;

    GameObject player;
    Animator playerAnimator;
    AnimatorStateInfo playerStateInfo;

    Image imgButton;
    Button btnButton;

    // ���b�Ń{�^�����ăA�N�e�B�u�ɂȂ邩
    //const int COUNT = 10;
    int countTime;

    // �����̃^�C�}�[
    float timer;

    bool noteSoundFlag = true;

    private void Awake()
    {
        imgButton = objButton.GetComponent<Image>();
        btnButton = objButton.GetComponent<Button>();

        player = GameObject.Find("Player");
        playerAnimator = player.GetComponent<Animator>();
    }

    /// <summary>
    /// �{�^�������������̏���
    /// </summary>
    public void OnClickButton()
    {
        

        playerStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (!(playerStateInfo.IsName("Idle") || playerStateInfo.IsName("Run"))) return;

        if (imgButton.fillAmount < 1 || !noteSoundFlag)
        {
            // �I���v�X���C��������炷���߂ɂ��̃{�^���������ꂽ�̂Ȃ�
            // �N�[���^�C�����������Ȃ��̂�UI�̏��������Ȃ��B
            // ���N�[���^�C�����I������^�C�~���O�ŃI���v�X���C�����o���ꍇ��
            // UI�̕\�L���ς��悤�Ƀt���O�Ǘ�������
            if (!noteSoundFlag && note)
            {
                noteSoundFlag = true;
            }

            return;
        }

        //countTime = COUNT;
        countTime = count;

        // �I���v�X���C����UI�������ꍇ�̏���
        if(note)
        {
            noteSoundFlag = false;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        // ���b����
        if (timer > 1f)
        {
            timer = 0f;
            if (countTime > 0)
            {
                countTime--;
                lblText.text = countTime.ToString();
                imgButton.fillAmount = 1 - (float)countTime / (float)count;
                //btnButton.interactable = false;
            }
            else
            {
                lblText.text = "";
                //btnButton.interactable = true;
            }
        }
    }
}
