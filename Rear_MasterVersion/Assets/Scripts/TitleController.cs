using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleController : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    [SerializeField] GameObject fadeObj;
    [SerializeField] EventSystem eventSystem;

    [SerializeField] GameObject SEObj;
    #endregion

    #region field
    GameObject titleBGMObj;
    GameObject selectUI;
    Image fadeImage;
    bool stageSelectFalg = false;
    float fadeSpeed = 3f;
    float red, green, blue, alfa;
    string stageName;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        titleBGMObj = GameObject.Find("TitleBGMObj");
        if (fadeObj != null)
        {
            fadeImage = fadeObj.GetComponent<Image>();
            red = fadeImage.color.r;
            green = fadeImage.color.g;
            blue = fadeImage.color.b;
            alfa = fadeImage.color.a;
        }

        GameSceneOther.SceneOther.Instance.referer = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            selectUI = eventSystem.currentSelectedGameObject.gameObject;
        }

        if (Input.GetAxis(eventSystem.GetComponent<StandaloneInputModule>().horizontalAxis) != 0 ||
            Input.GetAxis(eventSystem.GetComponent<StandaloneInputModule>().verticalAxis) != 0)
        {
            if (eventSystem.GetComponent<EventSystem>().currentSelectedGameObject == null)
                selectUI.GetComponent<Button>().Select();
        }

        if (SceneManager.GetActiveScene().name == "StageSelect"
            && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown("joystick button 1"))
            && !stageSelectFalg)
        {
            Invoke("BackScene", 1);
            SEObj.GetComponent<ButtonSEScript>().OnClick();
        }

        if (stageSelectFalg)
        {
            if (alfa < 1f)
            {
                FadeOut();
                titleBGMObj.GetComponent<TitleBGMScript>().FadeOutBGM();
            }
            else
            {
                titleBGMObj.GetComponent<TitleBGMScript>().EraseBGM();
                SceneManager.LoadScene(stageName);
            }
        }
    }
    #endregion

    #region public function
    public void ClickStageSelectButton()
    {
        Invoke("StageSelectScene", 1);
    }

    public void ClickControlButton()
    {
        Invoke("CntrolScene", 1);
        
    }

    public void ClickQuitButton()
    {
        UnityEngine.Application.Quit();
    }

    public void ClickStage01Button()
    {
        stageSelectFalg = true;
        Invoke("Stage01Scene", 1);
    }

    public void ClickStage02Button()
    {
        stageSelectFalg = true;
        Invoke("Stage02Scene", 1);
    }

    public void ClickStage03Button()
    {
        stageSelectFalg = true;
        Invoke("Stage03Scene", 1);
    }
    #endregion

    #region private function
    void StageSelectScene()
    {
        SceneManager.LoadScene("StageSelect");
    }

    void CntrolScene()
    {
        SceneManager.LoadScene("CntrolScene");
    }

    void Stage01Scene()
    {
        stageName = "Stage01";
    }

    void Stage02Scene()
    {
        stageName = "Stage02";
    }

    void Stage03Scene()
    {
        stageName = "Stage03";
    }

    //public void ClickBackButton()
    //{
    //    Invoke("BackScene", 1);
    //}

    void BackScene()
    {
        SceneManager.LoadScene("Title");
    }

    void FadeOut()
    {
        alfa += Time.deltaTime / fadeSpeed;
        fadeImage.color = new Color(red, green, blue, alfa);
    }
    #endregion
}
