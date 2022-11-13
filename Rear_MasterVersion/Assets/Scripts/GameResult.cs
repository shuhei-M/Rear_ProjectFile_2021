using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameResult : MonoBehaviour
{
    enum Rank { S, A, B, C};

    [SerializeField] Sprite[] rankSprite = new Sprite[4];
    [SerializeField] Sprite[] illustSprite = new Sprite[4];
    [SerializeField] GameObject rankImage;
    [SerializeField] GameObject clearTimeText;
    [SerializeField] GameObject timeImage;
    [SerializeField] GameObject illustImage;
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject titleButton;
    [SerializeField] GameObject SEObj;
    [SerializeField] GameObject eventSystem;
    [SerializeField] AudioClip pasteSE;
    [SerializeField] AudioClip selectSE;
    [SerializeField] AudioClip clickSE;
    GameObject selectUI;
    float[] Stage01GoalTime = { 60, 120, 180 };
    float[] Stage02GoalTime = { 120, 180, 300 };
    float[] Stage03GoalTime = { 180, 240, 360  /*180, 300, 420*/};
    float showTimer = 0f;

    Rank clearRank;

    // Start is called before the first frame update
    void Start()
    {
        // Stage01のランク付け
        if (GameSceneOther.SceneOther.Instance.referer == "Stage01")
        {
            if (Timer.time < Stage01GoalTime[0])
            {
                clearRank = Rank.S;
                rankImage.GetComponent<Image>().sprite = rankSprite[0];
                illustImage.GetComponent<Image>().sprite = illustSprite[0];
            }
            else if (Timer.time < Stage01GoalTime[1])
            {
                clearRank = Rank.A;
                rankImage.GetComponent<Image>().sprite = rankSprite[1];
                illustImage.GetComponent<Image>().sprite = illustSprite[1];
            }
            else if (Timer.time < Stage01GoalTime[2])
            {
                clearRank = Rank.B;
                rankImage.GetComponent<Image>().sprite = rankSprite[2];
                illustImage.GetComponent<Image>().sprite = illustSprite[2];
            }
            else
            {
                clearRank = Rank.C;
                rankImage.GetComponent<Image>().sprite = rankSprite[3];
                illustImage.GetComponent<Image>().sprite = illustSprite[3];
            }
        }

        // Stage02のランク付け
        if (GameSceneOther.SceneOther.Instance.referer == "Stage02")
        {
            if (Timer.time < Stage02GoalTime[0])
            {
                clearRank = Rank.S;
                rankImage.GetComponent<Image>().sprite = rankSprite[0];
                illustImage.GetComponent<Image>().sprite = illustSprite[0];
            }
            else if (Timer.time < Stage02GoalTime[1])
            {
                clearRank = Rank.A;
                rankImage.GetComponent<Image>().sprite = rankSprite[1];
                illustImage.GetComponent<Image>().sprite = illustSprite[1];
            }
            else if (Timer.time < Stage02GoalTime[2])
            {
                clearRank = Rank.B;
                rankImage.GetComponent<Image>().sprite = rankSprite[2];
                illustImage.GetComponent<Image>().sprite = illustSprite[2];
            }
            else
            {
                clearRank = Rank.C;
                rankImage.GetComponent<Image>().sprite = rankSprite[3];
                illustImage.GetComponent<Image>().sprite = illustSprite[3];
            }
        }

        // Stage03のランク付け
        if (GameSceneOther.SceneOther.Instance.referer == "Stage03")
        {
            if (Timer.time < Stage03GoalTime[0])
            {
                clearRank = Rank.S;
                rankImage.GetComponent<Image>().sprite = rankSprite[0];
                illustImage.GetComponent<Image>().sprite = illustSprite[0];
            }
            else if (Timer.time < Stage03GoalTime[1])
            {
                clearRank = Rank.A;
                rankImage.GetComponent<Image>().sprite = rankSprite[1];
                illustImage.GetComponent<Image>().sprite = illustSprite[1];
            }
            else if (Timer.time < Stage03GoalTime[2])
            {
                clearRank = Rank.B;
                rankImage.GetComponent<Image>().sprite = rankSprite[2];
                illustImage.GetComponent<Image>().sprite = illustSprite[2];
            }
            else
            {
                clearRank = Rank.C;
                rankImage.GetComponent<Image>().sprite = rankSprite[3];
                illustImage.GetComponent<Image>().sprite = illustSprite[3];
            }
        }

        // デバッグ用
        Debug.Log(Timer.time);
        Debug.Log(clearRank);
    }

    // Update is called once per frame
    void Update()
    {

        // 背景を見せたら1秒経過ごとに結果を表示していく
        if (showTimer > 1f && !rankImage.activeInHierarchy)
        {
            rankImage.SetActive(true);
            SEObj.GetComponent<AudioSource>().PlayOneShot(pasteSE);
            showTimer = 0f;
        }
        else if (showTimer > 1f && !clearTimeText.activeInHierarchy)
        {
            int minutes = (int)Timer.time / 60;
            int seconds = (int)Timer.time % 60;
            clearTimeText.GetComponent<Text>().text = minutes.ToString() + ":" + seconds.ToString();
            clearTimeText.SetActive(true);
            timeImage.SetActive(true);
            SEObj.GetComponent<AudioSource>().PlayOneShot(pasteSE);
            showTimer = 0f;
        }
        else if (showTimer > 1f && !illustImage.activeInHierarchy)
        {
            illustImage.SetActive(true);
            SEObj.GetComponent<AudioSource>().PlayOneShot(pasteSE);
            showTimer = 0f;
        }
        else if (showTimer > 1f && !retryButton.activeInHierarchy)
        {
            retryButton.SetActive(true);
            titleButton.SetActive(true);
        }

        if (!retryButton.activeInHierarchy)
        {
            showTimer += Time.deltaTime;
        }

        if (retryButton.activeInHierarchy)
        {
            if (eventSystem.GetComponent<EventSystem>().currentSelectedGameObject != null)
            {
                selectUI = eventSystem.GetComponent<EventSystem>().currentSelectedGameObject.gameObject;
            }

            if (Input.GetAxis(eventSystem.GetComponent<StandaloneInputModule>().horizontalAxis) != 0 ||
                Input.GetAxis(eventSystem.GetComponent<StandaloneInputModule>().verticalAxis) != 0)
            {
                if (eventSystem.GetComponent<EventSystem>().currentSelectedGameObject == null)
                    selectUI.GetComponent<Button>().Select();
            }
        }
    }

    public void ClickRetryButton()
    {
        SEObj.GetComponent<AudioSource>().PlayOneShot(clickSE);
        SceneManager.LoadScene(GameSceneOther.SceneOther.Instance.referer);
    }

    public void ClickTitleButton()
    {
        SEObj.GetComponent<AudioSource>().PlayOneShot(clickSE);
        SceneManager.LoadScene("Title");
    }

    public void DeSelectButton()
    {
        SEObj.GetComponent<AudioSource>().PlayOneShot(selectSE);
    }
}
