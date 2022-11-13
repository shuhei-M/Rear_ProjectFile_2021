using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCoolTime : MonoBehaviour
{
    [SerializeField] GameObject objButton;	// Image＋Buttonのオブジェクトをアサイン
    [SerializeField] Text lblText;			// 残り時間を表示するTextオブジェクトをアサイン
    [SerializeField] int count;
    [SerializeField] bool note;

    GameObject player;
    Animator playerAnimator;
    AnimatorStateInfo playerStateInfo;

    Image imgButton;
    Button btnButton;

    // 何秒でボタンが再アクティブになるか
    //const int COUNT = 10;
    int countTime;

    // ただのタイマー
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
    /// ボタンを押した時の処理
    /// </summary>
    public void OnClickButton()
    {
        

        playerStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (!(playerStateInfo.IsName("Idle") || playerStateInfo.IsName("Run"))) return;

        if (imgButton.fillAmount < 1 || !noteSoundFlag)
        {
            // オンプスライムが音を鳴らすためにこのボタンが押されたのなら
            // クールタイムが発動しないのでUIの処理をしない。
            // 次クールタイムが終わったタイミングでオンプスライムを出す場合に
            // UIの表記が変わるようにフラグ管理をする
            if (!noteSoundFlag && note)
            {
                noteSoundFlag = true;
            }

            return;
        }

        //countTime = COUNT;
        countTime = count;

        // オンプスライムのUIだった場合の処理
        if(note)
        {
            noteSoundFlag = false;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        // 毎秒処理
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
