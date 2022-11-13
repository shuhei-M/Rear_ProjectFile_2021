using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScreen : MonoBehaviour
{
    [SerializeField]
    private Material targetMaterial;

    [SerializeField]
    private float scrollX;
    [SerializeField]
    private float scrollY;

    private Vector2 offset;

    float scale =1.0f;
    [SerializeField] float scaleUpSpeed;

    private void Awake()
    {
        offset = targetMaterial.mainTextureOffset;
    }

    private void Update()
    {
        offset.x += scrollX * Time.deltaTime;
        offset.y += scrollY * Time.deltaTime;
        targetMaterial.mainTextureOffset = offset;

        if(scale < 10.0f)
        {
            scale += scaleUpSpeed;
            if (scale > 10.0f) scale = 10.0f;
            this.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
