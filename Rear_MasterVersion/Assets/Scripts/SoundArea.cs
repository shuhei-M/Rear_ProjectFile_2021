using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundArea : MonoBehaviour
{
    GameObject noteAvatar;
    List<GameObject> ListenEnemy;
    SphereCollider area;
    float soundTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        noteAvatar = this.transform.root.gameObject;
        area = GetComponent<SphereCollider>();
        ListenEnemy = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (area.enabled)
        {
            soundTime += Time.deltaTime;
        }

        if (soundTime >= 5f)
        {   // 5�b�o�߂ŉ���炷�̂���߂�
            area.enabled = false;
            noteAvatar.transform.GetChild(3).gameObject.SetActive(false);
            Destroy(noteAvatar, 2.1f);
            Invoke("EnemyTargetReset", 2.0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {   // �͈͓��̓G�����g�ɒǐՏ�Ԃɂ��A�L�^���Ă���
            var Enemy = other.gameObject;
            var ec = Enemy.transform.GetComponent<EnemyController>();
            ec.Avatar = noteAvatar;
            ec.AvatarIsFinded = true;
            for (int i = 0; i < ListenEnemy.Count; i++)
            {
                if (ListenEnemy[i] == Enemy)
                {
                    return;
                }
            }
            ListenEnemy.Add(Enemy);
        }
    }

    void Sounding()
    {
        area.enabled = true;
    }

    public void NoSounding()
    {
        area.enabled = false;
    }

    void EnemyTargetReset()
    {   // �I���v�X���C���̉��𕷂��ē|���ɗ����S�Ă̓G�̃^�[�Q�e�B���O�����Z�b�g
        for (int i = 0; i < ListenEnemy.Count; i++)
        {
            var ec = ListenEnemy[i].transform.GetComponent<EnemyController>();
            ec.AvatarIsFinded = false;
        }
    }
}
