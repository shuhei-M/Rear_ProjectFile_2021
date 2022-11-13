using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarData : MonoBehaviour
{
    //[SerializeField] float moveSpeed = 3.0f;
    [SerializeField] GameObject hornAvatar;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject smokeAvatar;
    [SerializeField] GameObject noteAvatar;
    [SerializeField] AudioClip noteSoundAudio;
    [SerializeField] GameObject smokeDummyObjPref;

    bool hornLife = true;
    bool smokeLife = true;
    bool noteLife = true;
    bool noteSoundFlag;

    AudioSource myAudio;
    GameObject player;
    GameObject newHornAvatar;
    GameObject newSmokeAvatar;
    GameObject newNoteAvatar;
    List<GameObject> dummySphereList = new List<GameObject>();
    Vector3 smokeV0 = new Vector3(0, 5, 5);
    int dummyCount = 15;
    float hornAvailablCount;
    float smokeAvailablCount;
    float noteAvailablCount;

    public bool HornLife
    {
        get { return hornLife; }
        set { hornLife = value; }
    }

    public bool SmokeLife
    {
        get { return smokeLife; }
        set { smokeLife = value; }
    }

    public bool NoteLife
    {
        get { return noteLife; }
        set { noteLife = value; }
    }

    public bool NoteSoundFlag
    {
        get { return noteSoundFlag; }
        set { noteSoundFlag = value; }
    }

    void Start()
    {
        player = GameObject.Find("Player");

        Rigidbody smokeRigid = smokeAvatar.GetComponent<Rigidbody>();

        myAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // ツノスライムのクールタイム処理
        if (hornLife)
        {
            hornAvailablCount = 0f;
        }
        else
        {
            hornAvailablCount += Time.deltaTime;
            if (hornAvailablCount >= 5f)
            {
                hornLife = true;
                Debug.Log("ツノスライム利用可能");
            }
        }

        // ケムリスライムのクールタイム処理
        if(smokeLife)
        {
            smokeAvailablCount = 0;
        }
        else
        {
            smokeAvailablCount += Time.deltaTime;
            if (smokeAvailablCount >= 15f)
            {
                smokeLife = true;
                Debug.Log("ケムリスライム利用可能");
            }
        }

        // オンプスライムのクールタイム処理
        if (noteLife)
        {
            noteAvailablCount = 0f;
        }
        else
        {
            noteAvailablCount += Time.deltaTime;
            if (noteAvailablCount >= 15f)
            {
                noteLife = true;
                Debug.Log("オンプスライム利用可能");
            }
        }
    }

    void HornSkill()    // ツノスライムのスキル
    {
        if (newHornAvatar != null)
        {
            newHornAvatar.GetComponent<HornAvatarController>().SendMessage("EnemyTargetReset");
            Destroy(newHornAvatar);
        }
        newHornAvatar = Instantiate(hornAvatar, spawnPoint.position, player.transform.rotation);
        //newHornAvatar.GetComponent<Rigidbody>().velocity = player.transform.forward * moveSpeed;
        newHornAvatar.AddComponent<HornAvatarController>();
        hornLife = false;
    }

    void SmokeOrbitInstantiate()    // ケムリスライムの軌道を生成
    {
        for(int i = 0; i < dummyCount; i++)
        {
            var obj = Instantiate(smokeDummyObjPref, player.transform);
            dummySphereList.Add(obj);
        }
    }

    void SmokeOrbit()   //ケムリスライムの軌道を更新
    {
        float secInterval = 0.1f;

        for(int i = 0; i < dummyCount; i++)
        {
            var t = i * secInterval;
            var x = t * smokeV0.x;
            var z = t * smokeV0.z;
            var y = (smokeV0.y * t) - 0.5f * (-Physics.gravity.y) * Mathf.Pow(t, 2.0f);
            dummySphereList[i].transform.localPosition = new Vector3(x, y, z);
        }
    }

    void DestroySmokeOrbit()   //ケムリスライムの軌道を消去
    {
        for (int i = 0; i < dummyCount; i++)
        {
            Destroy(dummySphereList[i]);
        }
        dummySphereList.Clear();
    }

    void SmokeSkill()   // ケムリスライムのスキル
    {
        DestroySmokeOrbit();
        newSmokeAvatar = Instantiate(smokeAvatar, spawnPoint.position/*player.transform.position*/, transform.rotation);
        var v = player.transform.TransformDirection(smokeV0);
        newSmokeAvatar.GetComponent<Rigidbody>().AddForce(v, ForceMode.Impulse);
        newSmokeAvatar.AddComponent<SmokeAvatarController>();
        newSmokeAvatar.transform.Find("SmokeArea").gameObject.AddComponent<SmokeArea>();
        smokeLife = false;
    }

    void NoteSkill()    // オンプスライムのスキル
    {
        if (noteLife && !noteSoundFlag)
        {   // 配置の処理
            if (newNoteAvatar != null)
            {
                newNoteAvatar.GetComponent<NoteAvatarController>().NoteDieOperation();
                Destroy(newNoteAvatar);
            }
            newNoteAvatar = Instantiate(noteAvatar, spawnPoint.position, player.transform.rotation);
            newNoteAvatar.AddComponent<NoteAvatarController>();
            newNoteAvatar.transform.Find("SoundArea").gameObject.AddComponent<SoundArea>();
            noteLife = false;
            noteSoundFlag = true;
        }
        else if (noteSoundFlag)
        {   // 音を鳴らしたときの処理
            noteSoundFlag = false;
            newNoteAvatar.transform.Find("SoundArea").gameObject.SendMessage("Sounding");
            newNoteAvatar.transform.GetChild(3).gameObject.SetActive(true);
            myAudio.clip = noteSoundAudio;
            myAudio.Play();
            Debug.Log("オンプスライムが音を鳴らしました。");
        }
    }

    public void UnNoteSE()
    {
        myAudio.Stop();
    }
}
