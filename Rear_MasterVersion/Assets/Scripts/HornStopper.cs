using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornStopper : MonoBehaviour
{
    GameObject parent;
    
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor")
        {
            parent.SendMessage("Stop");
        }
    }
}
