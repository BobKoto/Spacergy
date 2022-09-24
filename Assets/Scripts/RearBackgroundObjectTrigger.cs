using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearBackgroundObjectTrigger : MonoBehaviour
{
    public delegate void BackgroundObjectEnteredRearTrigger(GameObject other);
    public static event BackgroundObjectEnteredRearTrigger OnBackgroundObjectEnteredRearTrigger;
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
        //  Debug.Log(" what? something hit the front barrier  " + other.name);
        if (other.gameObject.CompareTag("BackgroundObject"))
        {
            OnBackgroundObjectEnteredRearTrigger?.Invoke(other.gameObject);
            // Debug.Log(" BackGroundObject entered trigger ... " + other.name);
            //Here is where we set an event for GenRandomBackground to detect and recycle the object... omg its working this far ...
        }
    }
}
