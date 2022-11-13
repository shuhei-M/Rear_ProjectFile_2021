using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSEScript : MonoBehaviour
{
    [SerializeField] AudioClip clickSE;
    [SerializeField] AudioClip selectSE;
    [SerializeField] GameObject eventSystem;

    AudioSource sound;
    bool clickFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        clickFlag = true;
        sound.clip = clickSE;
        sound.PlayOneShot(sound.clip);
        eventSystem.SetActive(false);
    }

    public void SelectButton()
    {
        if (clickFlag)
            return;

        sound.clip = selectSE;
        sound.PlayOneShot(sound.clip);
    }
}
