using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CntrolScript : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject seObj;
    [SerializeField] GameObject[] page;
    GameObject nowPage;
    GameObject selectUI;
    float coolDeltaTime = 0f;
    float coolTime = 0.5f;
    bool titleBackFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        nowPage = page[0];
    }

    // Update is called once per frame
    void Update()
    {
        //if (eventSystem.currentSelectedGameObject != null)
        //{
        //    selectUI = eventSystem.currentSelectedGameObject.gameObject;
        //}

        //if (Input.GetAxis(eventSystem.GetComponent<StandaloneInputModule>().horizontalAxis) != 0 ||
        //    Input.GetAxis(eventSystem.GetComponent<StandaloneInputModule>().verticalAxis) != 0)
        //{
        //    if (eventSystem.GetComponent<EventSystem>().currentSelectedGameObject == null)
        //        selectUI.GetComponent<Button>().Select();
        //}

        if (coolDeltaTime < 1.0f)
        {
            coolDeltaTime += Time.deltaTime;
        }

        // �y�[�W��i�߂�
        if(Input.GetAxis("Horizontal") > 0 && coolDeltaTime > coolTime && !titleBackFlag)
        {
            for(int i = 0; i < page.Length - 1; i++)
            {
                if (nowPage == page[i])
                {
                    page[i].SetActive(false);
                    page[i + 1].SetActive(true);
                    nowPage = page[i + 1];
                    coolDeltaTime = 0f;
                    seObj.GetComponent<ButtonSEScript>().SelectButton();
                    break;
                }
            }
        }

        // �y�[�W��߂�
        if (Input.GetAxis("Horizontal") < 0 && coolDeltaTime > coolTime && !titleBackFlag)
        {
            for (int i = 1; i < page.Length; i++)
            {
                if (nowPage == page[i])
                {
                    page[i].SetActive(false);
                    page[i - 1].SetActive(true);
                    nowPage = page[i - 1];
                    coolDeltaTime = 0f;
                    seObj.GetComponent<ButtonSEScript>().SelectButton();
                    break;
                }
            }
        }

        // TitleScene�ɖ߂�
        if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown("joystick button 1"))
        {
            titleBackFlag = true;
            seObj.GetComponent<ButtonSEScript>().OnClick();
            Invoke("TitleScene", 1);
        }
    }

    //// TitleScene�ɖ߂�
    //public void BackTitle()
    //{
    //    seObj.GetComponent<ButtonSEScript>().OnClick();
    //    Invoke("TitleScene", 1);
    //}

    void TitleScene()
    {
        SceneManager.LoadScene("Title");
    }
}
