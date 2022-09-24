using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectTrigger : MonoBehaviour   //Find this in FrontBarrierTrigger  - for recycling objects that move on z 
{
    public delegate void BackgroundObjectEnteredTrigger(GameObject other);
    public static event BackgroundObjectEnteredTrigger OnBackgroundObjectEnteredTrigger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(" what? something hit the front barrier  " + other.name + " tag? " + other.tag);
        if (other.gameObject.CompareTag("BackgroundObject")
            || other.gameObject.CompareTag("Face" )
            || other.gameObject.CompareTag("RogueTesla")
            || other.gameObject.CompareTag("Satellite")
            || other.gameObject.CompareTag("WormholeSegment")
            || other.gameObject.CompareTag("WormholePipe") )
        {
            OnBackgroundObjectEnteredTrigger?.Invoke(other.gameObject);
             //    Debug.Log(" BackGroundObject entered trigger ... " + other.name);
                   //Here is where we set an event for GenRandomBackground et. al.  to detect and recycle the object... omg its working this far ...
        }
    }
}
