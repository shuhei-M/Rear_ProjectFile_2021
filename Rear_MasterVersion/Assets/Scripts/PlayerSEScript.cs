using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSEScript : MonoBehaviour
{
    [SerializeField] AudioClip attackSE;
    [SerializeField] AudioClip dieSE;
    [SerializeField] AudioClip jumpSE;
    [SerializeField] AudioClip avatarInstantiateSE;
    [SerializeField] AudioClip avatarSelectSE;
    [SerializeField] GameObject pauseObj;

    AudioSource myAudio;

    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    public void AtttackSE()
    {
        myAudio.PlayOneShot(attackSE);
    }

    public void DieSE()
    {
        myAudio.PlayOneShot(dieSE);
    }

    public void JumpSE()
    {
        myAudio.PlayOneShot(jumpSE);
    }

    public void AvatarInstantiateSE()
    {
        myAudio.PlayOneShot(avatarInstantiateSE);
    }

    public void AvatarSelectSE()
    {
        if (pauseObj.activeInHierarchy)
            return;
        myAudio.PlayOneShot(avatarSelectSE);
    }
}
