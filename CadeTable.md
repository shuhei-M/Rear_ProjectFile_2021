# ソースコードの詳細  
  
  
## スクリプトファイル
| スクリプトファイル | 概要 |
| --- | --- |
| ▼[Editorフォルダ](https://github.com/shuhei-M/Rear_ProjectFile_2021/tree/main/Rear_MasterVersion/Assets/Editor) |  |
| [FindReferenceAsset.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Editor/FindReferenceAsset.cs) | Editor拡張。オブジェクトの参照を確認する。オブジェクトを右クリックし、そこから「参照を探す」を選択。0個であれば、そのオブジェクトをプロジェクトファイルから削除することを検討する。 |
| ▼[Scriptフォルダ](https://github.com/shuhei-M/Rear_ProjectFile_2021/tree/main/Rear_MasterVersion/Assets/Scripts) |  |
| [AvatarData.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/AvatarData.cs) | DestroySmokeOrbit()関数のみ追加。PlayerControllerクラスから放物線上の投射予測線を消すための関数。 |
| [ButtonCoolTimes.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/ButtonCoolTime.cs) |  |
| [EnemyContlloer.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/EnemyController.cs) |  |
| [HornAvatorContlloer.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/HornAvatarController.cs) | 移動方法・衝突検知方法を変更、死亡時赤いエミッションを放つ機能・ジャンプ機能を追加。 |
| [HornStopper.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/HornStopper.cs) |  |
| [IconContlloer.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/IconController.cs) |  |
| [ManagerScript.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/ManagerScript.cs) |  |
| [NoteAvatarController.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/NoteAvatarController.cs) | 死亡時赤いエミッションを放つ機能のみ追加。 |
| [PlayerContlloer.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/PlayerController.cs) |  |
| [SearchArea.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/SearchArea.cs) |  |
| [SearchLight.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/SearchLight.cs) |  |
| [SmokeArea.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/SmokeArea.cs) | EnemySmokeReset()関数以外の全て。 |
| [SmokeAvatorContlloer.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/SmokeAvatarController.cs) | OnCollisionEnter()関数、煙幕エフェクトのみ追加。 |
| [SmokeScreen.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/SmokeScreen.cs) |  |
| [StartAndEndGame.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/StartAndEndGame.cs) | 基盤（プレイヤー死亡時処理、敵を全滅させた時の処理）の作成、OnRetry()、OnTitle()関数の作成 |
| [Timer.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/Timer.cs) |  |
| [TitleContlloer.cs](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/Scripts/Timer.cs) | 基盤（各シーンへの遷移機能）の作成、改良。フェードアウト、SE以外 |  
  
  
  
## シェーダファイル
| シェーダファイル | 概要 |
| --- | --- |
| ▼[MyAssets/SoundWaveフォルダ](https://github.com/shuhei-M/Rear_ProjectFile_2021/tree/main/Rear_MasterVersion/Assets/MyAssets/SoundWave) |  |
| [Wave.shader](https://github.com/shuhei-M/Rear_ProjectFile_2021/blob/main/Rear_MasterVersion/Assets/MyAssets/SoundWave/Wave.shader) | オンプスライムの発動と効果範囲を示すための、波紋が広がるようなエフェクト。 |

<!-- 
| [.cs]() |  |
| [ソースファイル名](プロジェクトに保存されているファイル名) | 説明文 |
-->
