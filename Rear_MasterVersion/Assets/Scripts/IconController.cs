using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    GameObject player;
    [SerializeField] Camera myCamera;
    //Vector3 v = new Vector3(0.0f, 1.3f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        //transform.position = player.transform.position + v;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = player.transform.position + v;
    }

    void LateUpdate()
    {
        //@ƒJƒƒ‰‚Æ“¯‚¶Œü‚«‚Éİ’è
        transform.rotation = myCamera.transform.rotation;
    }
}
