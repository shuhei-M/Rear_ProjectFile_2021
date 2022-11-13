using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGMScript : MonoBehaviour
{
    static bool isLoad = false;
    float fadeSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        if (isLoad)
        {   // すでにロードされていたら自分自身を破棄して終了
            Destroy(this.gameObject);
        }

        isLoad = true;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EraseBGM()
    {
        isLoad = false;
        Destroy(this.gameObject);
    }

    public void FadeOutBGM()
    {
        var volume = GetComponent<AudioSource>().volume;

        if (volume > 0)
        {
            GetComponent<AudioSource>().volume -= fadeSpeed * Time.deltaTime;
        }
    }
}
