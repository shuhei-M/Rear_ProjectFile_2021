using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartAndEndGame : MonoBehaviour
{
    [SerializeField] Sprite readyImage;
    [SerializeField] Sprite goImage;
    [SerializeField] GameObject playUI;
    [SerializeField] GameObject startCall;
    [SerializeField] GameObject startCallImage;
    [SerializeField] GameObject resultUI;
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject congratulationsImage;
    [SerializeField] GameObject skipButton;
    [SerializeField] GameObject fadeObj;
    [SerializeField] GameObject avatarEventSystem;
    [SerializeField] GameObject eventSystem;
    [SerializeField] GameObject BGMObj;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject[] skillTable;
    [SerializeField] GameObject[] viewCmera;
    [SerializeField] bool[] viewFlag;
    [SerializeField] int enemyLife;
    Image fadeImage;
    Camera nowView;
    GameObject selectUI;
    int viewCount = 0;
    float fadeSpeed = 3f;
    float red, green, blue, alfa;
    float st;
    float startTimer = 0f;
    float endTime = 0f;
    bool readyFlag = false;
    bool goCall = false;
    bool startFlag = false;
    bool showButtonFlag = false;
    bool viewSkipFlag = false;

    public static int EnemyLife;


    // Start is called before the first frame update
    void Start()
    {
        fadeImage = fadeObj.GetComponent<Image>();
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;

        // スタートをコールするまでゲームが始まらない
        //Time.timeScale = 0f;
        //st = Time.realtimeSinceStartup;

        EnemyLife = enemyLife;

        viewFlag[0] = true;
    }

    // Update is called once per frame
    void Update()
    {
        // まだゲームが始まってないとき
        if(!startFlag)
        {
            if (GameSceneOther.SceneOther.Instance.referer != "StageSelect"
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 1")))
            {
                viewSkipFlag = true;
                alfa = 0.0f;
                fadeImage.color = new Color(red, green, blue, alfa);
                BGMObj.GetComponent<StageBGMScript>().VolumeSet();
            }

            if (GameSceneOther.SceneOther.Instance.referer != "StageSelect")
            {
                skipButton.SetActive(true);
            }

            if (!viewSkipFlag && !viewFlag[viewFlag.Length - 1])
            {
                if (alfa > 0f)
                {
                    FadeIn();
                    if (alfa < 1f)
                        BGMObj.GetComponent<StageBGMScript>().FadeInBGM();
                }
                else if (!viewFlag[viewFlag.Length - 1])
                {
                    if (startTimer > 2f)
                    {
                        viewCmera[viewCount].GetComponent<Camera>().depth = 0;
                        viewCmera[viewCount].SetActive(false);
                        viewCount++;
                        viewCmera[viewCount].SetActive(true);
                        viewCmera[viewCount].GetComponent<Camera>().depth = 1;
                        viewFlag[viewCount] = true;
                        startTimer = 0f;
                    }
                }
            }
            else if (viewSkipFlag && !mainCamera.activeInHierarchy)
            {
                viewCmera[viewCount].GetComponent<Camera>().depth = 0;
                viewCmera[viewCount].SetActive(false);
                mainCamera.SetActive(true);
                mainCamera.GetComponent<Camera>().depth = 1;
                startCall.SetActive(true);
                playUI.SetActive(true);
                skipButton.GetComponent<Image>().enabled = false;
                startTimer = 0f;
            }
            else if (startTimer >= 2f && !mainCamera.activeInHierarchy)
            {
                viewCmera[viewFlag.Length - 1].GetComponent<Camera>().depth = 0;
                viewCmera[viewFlag.Length - 1].SetActive(false);
                mainCamera.SetActive(true);
                mainCamera.GetComponent<Camera>().depth = 1;
                startCall.SetActive(true);
                playUI.SetActive(true);
                skipButton.GetComponent<Image>().enabled = false;
                startTimer = 0f;
            }
            else if(startTimer >= 2f && !readyFlag)
            {
                //skipButton.SetActive(false);
                //skipButton.GetComponent<Image>().enabled = false;
                startCallImage.GetComponent<Image>().sprite = readyImage;
                readyFlag = true;
                startTimer = 0f;
            }
            else if (startTimer >= 2f && !goCall)
            {
                // 選択したステージをここで取得することで、viewをスキップする機能を採用できるようにしている
                GameSceneOther.SceneOther.Instance.referer
                    = SceneManager.GetActiveScene().name;
                startCallImage.GetComponent<Image>().sprite = goImage;
                goCall = true;
                startTimer = 0f;
                //st = Time.realtimeSinceStartup;
            }
            else if (startTimer >= 1f && goCall)
            {
                avatarEventSystem.SetActive(true);
                startCall.SetActive(false);
                startFlag = true;
                Timer.timeStart = true;
                //Time.timeScale = 1f;
            }

            //startTimer = Time.realtimeSinceStartup - st;
            startTimer += Time.deltaTime;
        }

        // スタートコールをしているかポーズ画面を表示していたら以降のUpdateの処理をしない
        if (Mathf.Approximately(Time.timeScale, 0f) || !startFlag)
        {
            return;
        }

        if (avatarEventSystem.GetComponent<EventSystem>().currentSelectedGameObject != null)
        {
            selectUI = avatarEventSystem.GetComponent<EventSystem>().currentSelectedGameObject.gameObject;
        }

        if (Input.GetAxis(avatarEventSystem.GetComponent<StandaloneInputModule>().horizontalAxis) != 0 ||
            Input.GetAxis(avatarEventSystem.GetComponent<StandaloneInputModule>().verticalAxis) != 0)
        {
            if(avatarEventSystem.GetComponent<EventSystem>().currentSelectedGameObject == null)
                selectUI.GetComponent<Button>().Select();
        }

        // プレイヤーが死んだ場合
        if (!PlayerController.Alive)
        {
            if (resultUI.activeInHierarchy == false)
            {
                for (int i = 0; i < skillTable.Length; i++)
                {
                    skillTable[i].SetActive(false);
                }
            }

            if (alfa < 0.5f)
            {
                FadeOut();
                BGMObj.GetComponent<StageBGMScript>().FadeOutBGM();
            }
            else
            {
                resultUI.GetComponent<RectTransform>().SetAsLastSibling();
                resultUI.SetActive(true);
                endTime += Time.deltaTime;
                if (endTime > 1f && !showButtonFlag)
                {
                    showButtonFlag = true;
                    GameObject Buttons = resultUI.transform.GetChild(1).gameObject;
                    Timer.timeStart = false;
                    Buttons.SetActive(true);
                    eventSystem.SetActive(true);
                    avatarEventSystem.SetActive(false);
                    retryButton.GetComponent<Button>().Select();
                }

                if (showButtonFlag)
                {
                    if (eventSystem.GetComponent<EventSystem>().currentSelectedGameObject != null)
                    {
                        selectUI = eventSystem.GetComponent<EventSystem>().currentSelectedGameObject.gameObject;
                    }

                    if (Input.GetAxisRaw(eventSystem.GetComponent<StandaloneInputModule>().horizontalAxis) != 0 ||
                        Input.GetAxisRaw(eventSystem.GetComponent<StandaloneInputModule>().verticalAxis) != 0)
                    {
                        if (eventSystem.GetComponent<EventSystem>().currentSelectedGameObject == null)
                            selectUI.GetComponent<Button>().Select();
                    }
                }
            }
        }

        // 敵を殲滅した場合
        if (EnemyLife < 1)
        {
            if(endTime > 1f && !congratulationsImage.activeInHierarchy)
            {
                congratulationsImage.SetActive(true);
                endTime = 0f;
            }
            else if (endTime > 3f)
            {
                if(alfa < 1f)
                {
                    FadeOut();
                    if (alfa > 0.5f)
                    {
                        BGMObj.GetComponent<StageBGMScript>().FadeOutBGM();
                    }
                }
                else
                {
                    Timer.timeStart = false;
                    SceneManager.LoadScene("ResultScene");
                }
            }
            endTime += Time.deltaTime;
        }
    }

    public void OnTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void OnRetry()
    {
        SceneManager.LoadScene(GameSceneOther.SceneOther.Instance.referer);
    }

    void FadeOut()
    {
        alfa += Time.deltaTime / fadeSpeed;
        fadeImage.color = new Color(red, green, blue, alfa);
    }

    void FadeIn()
    {
        alfa -= Time.deltaTime / fadeSpeed;
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
