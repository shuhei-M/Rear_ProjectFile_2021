using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScripts : MonoBehaviour
{
    [SerializeField] GameObject startCall;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject avatarEventSystem;
    [SerializeField] GameObject pauseEventSystem;
    [SerializeField] GameObject BGMObj;
    [SerializeField] GameObject[] skillTable;
    GameObject selectAvatar;
    GameObject selectUI;
    Animator playerAnimator;
    AnimatorStateInfo stateInfo;
    bool PauseFlag = false;
    bool closeFlag = false;
    bool retryFlag = false;
    bool titleFlag = false;
    float time = 0f;
    float st = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // スタートコールをしている最中はポーズ画面を表示できないようにする
        if (!Timer.timeStart)
            return;

        // ゲームが終了していたらポーズ画面を表示できないようにする
        if (!PlayerController.Alive || (StartAndEndGame.EnemyLife < 1))
            return;

        if (PauseFlag)
        {
            if (pauseEventSystem.GetComponent<EventSystem>().currentSelectedGameObject != null)
            {
                selectUI = pauseEventSystem.GetComponent<EventSystem>().currentSelectedGameObject.gameObject;
            }

            if (Input.GetAxisRaw(pauseEventSystem.GetComponent<StandaloneInputModule>().horizontalAxis) != 0 ||
                Input.GetAxisRaw(pauseEventSystem.GetComponent<StandaloneInputModule>().verticalAxis) != 0)
            {
                if (pauseEventSystem.GetComponent<EventSystem>().currentSelectedGameObject == null)
                    selectUI.GetComponent<Button>().Select();
            }
        }

        stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Die"))
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown("joystick button 7"))
            {
                if (!PauseFlag)
                {
                    OpenPause();
                }
                else
                {
                    CloseOnFlag();
                }
            }
        }

        if (closeFlag)
        {
            time = Time.realtimeSinceStartup - st;

            if (time >= 1.0f)
            {
                ClosePause();
                closeFlag = false;
            }
        }

        if (retryFlag)
        {
            time = Time.realtimeSinceStartup - st;

            if (time >= 1.0f)
            {

                Timer.timeStart = false;
                Time.timeScale = 1f;
                SceneManager.LoadScene(GameSceneOther.SceneOther.Instance.referer);
            }
        }

        if (titleFlag)
        {
            time = Time.realtimeSinceStartup - st;

            if (time >= 1.0f)
            {

                Timer.timeStart = false;
                Time.timeScale = 1f;
                SceneManager.LoadScene("Title");
            }
        }
    }

    void CloseOnFlag()
    {
        closeFlag = true;
    }

    void OpenPause()
    {
        Time.timeScale = 0f;
        selectAvatar = avatarEventSystem.GetComponent<EventSystem>().currentSelectedGameObject.gameObject;
        pause.SetActive(true);
        pauseEventSystem.SetActive(true);
        avatarEventSystem.SetActive(false);
        for (int i = 0; i < skillTable.Length; i++)
        {
            skillTable[i].GetComponent<Button>().enabled = false;
        }
        continueButton.GetComponent<Button>().Select();
        BGMObj.GetComponent<StageBGMScript>().PauseBGM();
        PauseFlag = true;
    }

    void ClosePause()
    {
        pause.SetActive(false);
        pauseEventSystem.SetActive(false);
        avatarEventSystem.SetActive(true);
        Time.timeScale = 1f;
        for (int i = 0; i < skillTable.Length; i++)
        {
            skillTable[i].GetComponent<Button>().enabled = true;
        }
        selectAvatar.GetComponent<Button>().Select();
        BGMObj.GetComponent<StageBGMScript>().UnPauseBGM();
        PauseFlag = false;
    }

    public void OnContinueButton()
    {
        CloseOnFlag();
        st = Time.realtimeSinceStartup;
    }

    public void OnRetryButton()
    {
        retryFlag = true;
        st = Time.realtimeSinceStartup;
    }

    public void OnTitleButton()
    {
        titleFlag = true;
        st = Time.realtimeSinceStartup;
    }
}
