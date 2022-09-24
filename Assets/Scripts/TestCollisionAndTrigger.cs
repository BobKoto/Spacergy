using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollisionAndTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("WormholeFrame hit  " + collision.gameObject.name);

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("WormholeFrame hit trigger " + other.gameObject.name);

    }


}
