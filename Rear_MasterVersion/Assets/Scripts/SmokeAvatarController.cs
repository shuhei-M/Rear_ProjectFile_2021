using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeAvatarController : MonoBehaviour
{
    GameObject smokeArea;
    //SphereCollider smokeArea;
    float deltaTime = 0;
    bool smokingFalg = false;

    GameObject smokeScreen;

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
            Debug.Log("ケムリ終了");
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor"
            || collision.gameObject.tag == "Fence")
        {
            Debug.Log("着地！");
            smokeScreen.SetActive(true);
        }
    }
}
