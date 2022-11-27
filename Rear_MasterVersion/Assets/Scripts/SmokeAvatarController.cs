using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> OnCollisionEnter(Collision collision)�֐��A�����G�t�F�N�g�̂ݒǉ��B </summary>

public class SmokeAvatarController : MonoBehaviour
{
    GameObject smokeArea;
    //SphereCollider smokeArea;
    float deltaTime = 0;
    bool smokingFalg = false;

    GameObject smokeScreen;   // (����)�����G�t�F�N�g

    // Start is called before the first frame update
    void Start()
    {
        smokeArea = transform.GetChild(2).gameObject;
        smokeScreen = transform.GetChild(3).gameObject;

        smokeScreen.SetActive(false);

        //smokeArea = transform.Find("SmokeArea").gameObject.GetComponent<GameObject>();
    }

    //private void Awake()
    //{
    //    offset = smokeScreenMaterial.mainTextureOffset;
    //}

    // Update is called once per frame
    void Update()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime >= 3f && !smokingFalg)
        {
            smokeArea.GetComponent<SphereCollider>().enabled = true;
            smokingFalg = true;
            Debug.Log("Smoking!!");
        }

        if (deltaTime >= 10f)
        {
            smokeArea.SendMessage("EnemySmokedReset");
            smokeScreen.SetActive(false);
            Debug.Log("�P�����I��");
            Destroy(this.gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Floor")
    //    {
    //        smokeScreen.SetActive(true);
    //    }
    //}

    /// <summary>
    /// ����ҁF����
    /// �P�����X���C�������n�����ۂɌĂ΂��
    /// �����G�t�F�N�g��W�J����
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor"
            || collision.gameObject.tag == "Fence")
        {
            Debug.Log("���n�I");
            smokeScreen.SetActive(true);
        }
    }
}
