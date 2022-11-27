using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 煙幕のエフェクトを発生させるクラス </summary>
public class SmokeScreen : MonoBehaviour
{
    #region define

    #endregion

    #region serialize field
    [SerializeField] Material targetMaterial;   // 煙幕用マテリアル

    /// <summary> 煙幕の模様が動く方向 </summary>
    [SerializeField] float scrollX;
    [SerializeField] float scrollY;

    [SerializeField] float scaleUpSpeed;   // 煙幕が拡大するスピード
    #endregion

    #region field
    Vector2 offset;   // 煙幕模様スクロール用

    float scale = 1.0f;   // 煙幕エフェクトの大きさ
    #endregion

    #region property

    #endregion

    #region Unity function
    private void Awake()
    {
        offset = targetMaterial.mainTextureOffset;
    }

    private void Update()
    {
        offset.x += scrollX * Time.deltaTime;
        offset.y += scrollY * Time.deltaTime;
        targetMaterial.mainTextureOffset = offset;

        // 大きさ10に到達するまで、煙幕エフェクトは徐々に大きくなる
        if (scale < 10.0f)
        {
            scale += scaleUpSpeed;
            if (scale > 10.0f) scale = 10.0f;
            this.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
