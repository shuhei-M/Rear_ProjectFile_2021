using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HornAvatarController : MonoBehaviour
{
    //[SerializeField] float moveSpeed = 3.0f;
    //[SerializeField] GameObject player;
    Vector3 GoStraight;
    Animator animator;
    AnimatorStateInfo stateInfo;
    Vector3 v = Vector3.zero;
    List<GameObject> hateEnemy;
    float deltaTime = 0f;
    float dieCountDown = 0f;
    float moveSpeed = 2.0f;
    bool stop = false;
    bool dieFlag = false;
    NavMeshAgent agent;

    bool IsJump = false;
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float jumpMagnification = 1.1f;
    CharacterController charaCon;

    GameObject hornStopper;

    public GameObject HateEnemy
    {   // í«ê’ÇµÇƒÇ´ÇΩìGÇï€ä«
        set { hateEnemy.Add(value); }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        hateEnemy = new List<GameObject>();

        agent = GetComponent<NavMeshAgent>();
        GoStraight = transform.forward * moveSpeed * Time.deltaTime;

        charaCon = GetComponent<CharacterController>();
        charaCon.enabled = false;

        hornStopper = transform.GetChild(2).gameObject;
    }

    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stop)
        {
            deltaTime = 0;
            dieCountDown += Time.deltaTime;
            //this.GetComponent<Rigidbody>().velocity = v;
            GoStraight = v;
            agent.isStopped = true;
            //transform.Rotate(new Vector3(0, 0, 180));
        }
        else
        {
            deltaTime += Time.deltaTime;
            //GetComponent<Rigidbody>().velocity = transform.forward * moveSpeed;
            agent.Move(GoStraight);
        }

        if (IsJump)
        {
            moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
            charaCon.Move((GoStraight / jumpMagnification) + (moveDirection * Time.deltaTime));
            if (charaCon.isGrounded)
            {
                IsJump = false;
                agent.Warp(transform.position);
                agent.updatePosition = true;
                charaCon.enabled = false;
            }
        }

        animator.SetFloat("DeltaTime", deltaTime);

        if (dieCountDown >= 1.5f)
        {
            EnemyTargetReset();
            animator.SetTrigger("toDie");
            Destroy(this.gameObject);
        }

        if(dieFlag)
        {
            if(transform.localScale.y >= 0)
            {
                transform.localScale -= new Vector3(0.0f, 0.01f, 0.0f);
            }
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if ((collision.gameObject.tag != "Floor") 
    //     && (collision.gameObject.tag != "Grass")
    //     && (collision.gameObject.tag != "JumpPoint"))
    //    {   // ï«Ç…ìñÇΩÇ¡ÇΩéûÇÃèàóù
    //        Vector3 hitPos = collision.contacts[0].point;
    //        var Direction = hitPos - transform.position;
    //        var angle = Vector3.Angle(transform.forward, Direction);

    //        if(angle <= 60f)
    //        {
    //            //this.GetComponent<Rigidbody>().velocity = v;
    //            GoStraight = v;
    //            agent.isStopped = true;
    //            stop = true;
    //            deltaTime = 0f;
    //            Debug.Log("ÉcÉmè’ìÀ");
    //        }
    //    }
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Floor")
    //    {
    //        for(int i = 0;i < collision.contacts.Length; i++)
    //        {
    //            Vector3 hitPos = collision.contacts[0].point;
    //            var Direction = hitPos - transform.position;
    //            var angle = Vector3.Angle(transform.forward, Direction);
    //            if (angle <= 60f && Direction.y <= 0.5f)
    //            {
    //                //this.GetComponent<Rigidbody>().velocity = v;
    //                GoStraight = v;
    //                agent.isStopped = true;
    //                stop = true;
    //                deltaTime = 0f;
    //            }
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        //// âåÇÃíÜÇ…ì¸Ç¡ÇΩÇÁíTímÇ≥ÇÍÇ»Ç≠Ç»ÇÈ
        //if (other.gameObject.tag == "SmokeArea")
        //{
        //    EnemyTargetReset();
        //    this.tag = "Untagged";
        //}

        if (other.gameObject.tag == "Sword")
        {   // çUåÇÇ≥ÇÍÇΩÇ∆Ç´Ç…èàóù
            var Enemy = other.transform.root.GetChild(0);
            var ec = Enemy.GetComponent<EnemyController>();

            EnemyTargetReset();

            //éÄñSéûÉAÉjÉÅÅ[ÉVÉáÉìÇÃë„ë÷èàóù
            var ps = GetComponent<ParticleSystem>();
            var ep = new ParticleSystem.EmitParams();
            ep.startColor = Color.red;
            ep.startSize = 0.1f;
            ps.Emit(ep, 1000);

            animator.SetTrigger("toDie");
            Debug.Log("ÉcÉmÇ‚ÇÁÇÍÇΩÅI");
            Destroy(this.gameObject, 0.5f);
            Destroy(hornStopper);
            dieFlag = true;
            GoStraight = v;
            agent.isStopped = true;
        }

        if (other.gameObject.tag == "JumpPoint")
        {
            Debug.Log("ÉcÉmîÚÇ—Ç‹Ç∑ÅI");

            Jump();
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject.tag == "SmokeArea")
    //    {
    //        this.tag = "HornAvatar";
    //    }
    //}

    public void EnemyTargetReset()
    {
        gameObject.tag = "Untagged";
        for (int i = 0; i < hateEnemy.Count; i++)
        {
            var ec = hateEnemy[i].transform.GetComponent<EnemyController>();
            ec.Avatar = null;
            ec.AvatarIsFinded = false;
        }
        hateEnemy.Clear();
    }

    void Stop()
    {
        Debug.Log("ÉcÉmè’ìÀÅI");
        stop = true;
    }

    public void Jump()
    {
        Debug.Log("Jump");
        charaCon.enabled = true;
        IsJump = true;
        agent.updatePosition = false;
        moveDirection.y = jumpSpeed;
        //playerSE.JumpSE();
    }
}
