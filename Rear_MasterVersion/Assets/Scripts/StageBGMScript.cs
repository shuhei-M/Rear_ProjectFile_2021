using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBGMScript : MonoBehaviour
{
    //AudioClip StageBGM;
    float fadeSpeed = 0.1f;

    public void VolumeSet()
    {
        GetComponent<AudioSource>().volume = 0.25f;
    }

    public void FadeOutBGM()
    {
        var volume = GetComponent<AudioSource>().volume;

        if (volume > 0)
        {
            GetComponent<AudioSource>().volume -= fadeSpeed * Time.deltaTime;
        }
    }

    public void FadeInBGM()
    {
        var volume = GetComponent<AudioSource>().volume;

        if (volume < 0.25)
        {
            GetComponent<AudioSource>().volume += fadeSpeed * Time.deltaTime;
        }
    }

    public void PauseBGM()
    {
        GetComponent<AudioSource>().Pause();
    }

    public void UnPauseBGM()
    {
        GetComponent<AudioSource>().UnPause();
    }
}
