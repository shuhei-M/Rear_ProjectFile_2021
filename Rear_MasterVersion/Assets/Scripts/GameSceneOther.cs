using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneOther : MonoBehaviour
{
    public class SceneOther
    {
        // EndGame.cs��State���\�b�h�łǂ̃X�e�[�W���Ăяo���ꂽ�����擾
        // �܂��A���U���g��ʂłǂ̃X�e�[�W�̕]�����̗p���邩��I�ԍۂɗ��p
        public readonly static SceneOther Instance = new SceneOther();

        public string referer = string.Empty;
    }
    
    //// EndGame.cs��State���\�b�h�łǂ̃X�e�[�W���Ăяo���ꂽ�����擾
    //// �܂��A���U���g��ʂłǂ̃X�e�[�W�̕]�����̗p���邩��I�ԍۂɗ��p
    //public readonly static GameSceneOther Instance = new GameSceneOther();

    //public string referer = string.Empty;
}
