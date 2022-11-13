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
        // �c�m�X���C���̃N�[���^�C������
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
                Debug.Log("�c�m�X���C�����p�\");
            }
        }

        // �P�����X���C���̃N�[���^�C������
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
                Debug.Log("�P�����X���C�����p�\");
            }
        }

        // �I���v�X���C���̃N�[���^�C������
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
                Debug.Log("�I���v�X���C�����p�\");
            }
        }
    }

    void HornSkill()    // �c�m�X���C���̃X�L��
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

    void SmokeOrbitInstantiate()    // �P�����X���C���̋O���𐶐�
    {
        for(int i = 0; i < dummyCount; i++)
        {
            var obj = Instantiate(smokeDummyObjPref, player.transform);
            dummySphereList.Add(obj);
        }
    }

    void SmokeOrbit()   //�P�����X���C���̋O�����X�V
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

    void DestroySmokeOrbit()   //�P�����X���C���̋O��������
    {
        for (int i = 0; i < dummyCount; i++)
        {
            Destroy(dummySphereList[i]);
        }
        dummySphereList.Clear();
    }

    void SmokeSkill()   // �P�����X���C���̃X�L��
    {
        DestroySmokeOrbit();
        newSmokeAvatar = Instantiate(smokeAvatar, spawnPoint.position/*player.transform.position*/, transform.rotation);
        var v = player.transform.TransformDirection(smokeV0);
        newSmokeAvatar.GetComponent<Rigidbody>().AddForce(v, ForceMode.Impulse);
        newSmokeAvatar.AddComponent<SmokeAvatarController>();
        newSmokeAvatar.transform.Find("SmokeArea").gameObject.AddComponent<SmokeArea>();
        smokeLife = false;
    }

    void NoteSkill()    // �I���v�X���C���̃X�L��
    {
        if (noteLife && !noteSoundFlag)
        {   // �z�u�̏���
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
        {   // ����炵���Ƃ��̏���
            noteSoundFlag = false;
            newNoteAvatar.transform.Find("SoundArea").gameObject.SendMessage("Sounding");
            newNoteAvatar.transform.GetChild(3).gameObject.SetActive(true);
            myAudio.clip = noteSoundAudio;
            myAudio.Play();
            Debug.Log("�I���v�X���C��������炵�܂����B");
        }
    }

    public void UnNoteSE()
    {
        myAudio.Stop();
    }
}
