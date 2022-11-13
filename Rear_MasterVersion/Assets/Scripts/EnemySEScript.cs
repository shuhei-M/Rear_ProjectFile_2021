using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySEScript : MonoBehaviour
{
    [SerializeField] AudioClip discoverSE;
    [SerializeField] AudioClip fightSE;
    [SerializeField] AudioClip stunSE;
    [SerializeField] AudioClip dieSE;

    AudioSource myAudio;

    float discoverCooltime = 0f;
    float fightCooltime = 0f;
    bool discoverFlag = true;
    bool fightFlag = true;

    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!discoverFlag)
        {
            if (discoverCooltime >= 1f)
            {
                discoverFlag = true;
                discoverCooltime = 0f;
            }
            else
            {
                discoverCooltime += Time.deltaTime;
            }
        }

        if (!fightFlag)
        {
            if (fightCooltime >= 1f)
            {
                fightFlag = true;
                fightCooltime = 0f;
            }
            else
            {
                fightCooltime += Time.deltaTime;
            }
        }
    }

    public void DiscoverSE()
    {
        if (!discoverFlag)
            return;
        myAudio.PlayOneShot(discoverSE);
        discoverFlag = false;
    }

    public void FightSE()
    {
        if (!fightFlag)
            return;
        myAudio.PlayOneShot(fightSE);
        fightFlag = false;
    }

    public void StunSE()
    {
        myAudio.loop = true;
        myAudio.clip = stunSE;
        myAudio.Play();
    }

    public void UnStunSE()
    {
        myAudio.Stop();
        myAudio.loop = false;
    }

    public void DieSE()
    {
        myAudio.loop = false;
        myAudio.PlayOneShot(dieSE);
    }
}
