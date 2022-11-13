using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchLight : MonoBehaviour
{
    GameObject enemy;
    Animator animator;
    AnimatorStateInfo stateInfo;

    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.gameObject;

        animator = enemy.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Die"))
        {
            Destroy(gameObject, 0.05f);
        }
    }
}
