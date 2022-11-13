using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneOther : MonoBehaviour
{
    public class SceneOther
    {
        // EndGame.csのStateメソッドでどのステージが呼び出されたかを取得
        // また、リザルト画面でどのステージの評価を採用するかを選ぶ際に利用
        public readonly static SceneOther Instance = new SceneOther();

        public string referer = string.Empty;
    }
    
    //// EndGame.csのStateメソッドでどのステージが呼び出されたかを取得
    //// また、リザルト画面でどのステージの評価を採用するかを選ぶ際に利用
    //public readonly static GameSceneOther Instance = new GameSceneOther();

    //public string referer = string.Empty;
}
