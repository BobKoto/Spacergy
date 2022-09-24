using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushForward : MonoBehaviour
{
    Rigidbody rb;
    IEnumerator coroutine;
 //   Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
      //  Debug.Log("We are watching  " + gameObject.name );

        coroutine = AddForceIfNeeded();
        StartCoroutine(coroutine);

    }

    // Update is called once per frame
    //void FixedUpdate()   //coroutine instead 
    //{

    //}
    IEnumerator AddForceIfNeeded()
    {
     //   bool didBump = false;
        float zVelocity;
        Vector3 objectVelocity;

        yield return new WaitForSeconds(2);
       // Debug.Log("Waited 2 Secs in coroutine"); Verified runs 1 for each but not much help 
        while (true)
        {
         //   Debug.Log("PushForward for " + gameObject.name + " reporting IN.");
            objectVelocity = rb.velocity;
            zVelocity = objectVelocity.z;
            if (zVelocity > -20)  //means slower : -40 is the original normal speed 
            {
                Debug.Log("We have a slow mover. Give a bump to: " + gameObject.name + "  Velocity = " + zVelocity);
           //     didBump = true;

             //    rb.AddForce(0, 0, -2000); //-2000 per GenRndBKg  IN THE EDITOR!   //neither this line nor rb.velocity (next line) seem to work :(
                 rb.velocity = new Vector3(0, 0, -40);
            }
         //   if (didBump) Debug.Log("We didBump to: " + gameObject.name + "  Velocity = " + zVelocity);
            yield return new WaitForSeconds(2);
        }
    }
    private void OnDisable()
    {
      //  Debug.Log("Push disabled for " + gameObject.name);
        StopAllCoroutines();
    }
    private void OnEnable()
    {
      //  Debug.Log("Push ENABLED for " + gameObject.name);
       // StopAllCoroutines();
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("bkgrnObject hit: " + collision.gameObject.name);
    //}
}
