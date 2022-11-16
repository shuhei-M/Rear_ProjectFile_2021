# Rear_ProjectFile_2021 概要
- ステルスアクションゲーム『Rear』の、公開用のプロジェクトファイルです。  
- Xboxコントローラを使用する、PC用のゲームです。  
- 5人のチームでUnityを用い、約半年間かけて制作しました。  
- 私は主にプログラムを担当しました。
## 開発期間
- 2021年 7月下旬～1月10日 (大学2年次 / 約半年間)  
## 開発環境
- C#
- Unity2020.3.18f
- VisualStudio2019
## チーム構成
- 企画担当1人、デザイン担当2人、プログラム担当2人です。
  
  
  
# プレイ動画
- 動画時間は約8分です。遊び方の説明と、Stage01～03のプレイ動画の四部構成で、概要欄にタイムスタンプがあります。  
[『Rear』プレイ動画](https://youtu.be/mYLQkXrPDaM)  
  
  
  
# 実行ファイル（.exe）の公開
[『Rear』実行ファイル](https://github.com/shuhei-M/Rear_ExeFile_2021)  
  
  
  
# 担当箇所の詳細（シーン毎に記載）
## タイトル画面
- 背景、UIの全て  
- シーン遷移とゲーム終了機能  
- デザイン分野が作成した素材の実装  
  
## ステージセレクト画面
- 背景、UIの全て  
- シーン遷移機能  
- デザイン分野が作成した素材の実装  
  
## ステージ（Stage01、Stage02、Stage03）  
- UI（ポーズ機能以外全て）  
    - 仕様の考案、決定  
    - スキル選択ボタン（クールタイム表示機能付き）  
    - 残り酸素量を示すゲージ  
    - 経過時間表示  
    - 残りの敵数表示  
    - ゲームオーバー時に表示される、タイトルボタンとリトライボタン  
    - デザイン分野が作成した素材の実装  
- プレイヤー（全て）  
    - アニメーターの作成  
    - 移動、ジャンプ、攻撃、草に隠れる、死亡機能  
    - スキル選択UIによる分身生成の制御機能  
    - 攻撃時のエフェクト  
    - デザイン分野が作成したモデル・モーションの実装  
- 分身スライム（一部）  
    - ツノ  
        - 移動、ジャンプ、衝突機能を追加・変更  
    - ケムリ  
        - 発動すると、敵からプレイヤーが発見されなくなる機能  
        - エフェクト  
    - オンプ  
        - エフェクト（HLSL/Cgを使用）  
    - デザイン分野が作成した3Dモデル（ツノ、ケムリ、オンプ）の実装  
- 敵（全て）  
    - アニメーターの作成  
    - プレイヤーの検知機能  
    - 駐在モードと徘徊モードを、インスペクターウィンドウ上で切り替えることが出来る機能（企画分野が敵の配置を考えやすくするため）  
    - 待機、徘徊、追跡、元居た場所への帰還、攻撃、死亡、スタン機能  
    - デザイン分野が作成した3Dモデル・モーションの実装  
- フィールド  
    - NavMeshの適応、設定、調整  
    - プレイヤーが隠れるための草の機能  
    - プレイヤー、ツノが段差でジャンプできる機能  
    - デザイン分野が作成した3Dモデルの実装  
  
  
  
# 担当ソースコード
[ソースコード表](CadeTable.md)
