using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraySphereGen : MonoBehaviour
{
    Rigidbody rb;
    Vector3 v3StartPosition, v3MyPlayerTransform;
    Transform myplayerTransform;
    public GameObject myPlayer;
    // Start is called before the first frame update
    void Start()
    {
        if (myPlayer)
        {
            myplayerTransform = myPlayer.GetComponent<Transform>();
  
            v3MyPlayerTransform = new Vector3(myplayerTransform.position.x, myplayerTransform.position.y, myplayerTransform.position.z);
          Debug.Log("Reporting from Start v3MyPlayerTransform = " + v3MyPlayerTransform);
        }
        rb = GetComponent<Rigidbody>();
        v3StartPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Debug.Log("Starting Position for GreySphere = " + v3StartPosition);
        PushToTheFront();

    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    void PushToTheFront()
    {
      //  rb.AddForce(0, 0, 0);  // like a brake?
        rb.AddForce(0, 0, -5 * 20); //push to the front? yup.
    }
    private void OnBecameInvisible()
    {
       // Debug.Log(" GraySphere Became Invisible.");
        RecycleObject();
     //   PushToTheFront();
    }
    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("GreySphere hit " + collider.name);
        if (collider.gameObject.CompareTag("tagFrontBarrier"))
        {
            RecycleObject();
        }
    }
    private void RecycleObject()
    {
        if (myplayerTransform)
        {
            myplayerTransform = myPlayer.GetComponent<Transform>();
            v3MyPlayerTransform = new Vector3(myplayerTransform.position.x, myplayerTransform.position.y, myplayerTransform.position.z);
      //  Debug.Log("Reporting from RecycleObject: v3MyPlayerTransform = " + v3MyPlayerTransform);
            v3StartPosition.x = Random.Range(-40, -20);
            transform.SetPositionAndRotation(v3StartPosition, new Quaternion(0, 0, 0,0));
       // Debug.Log("Reporting from RecycleObject: GreySphere v3StartPosition = " + v3StartPosition);
        }

    }
}
