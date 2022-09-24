using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{

    MeshRenderer objectToBlink;
    IEnumerator blinkTheObject;
    //int blinkTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        blinkTheObject = BlinkTheObject();
        objectToBlink = GetComponent<MeshRenderer>();
        blinkTheObject = BlinkTheObject();
        StartCoroutine(blinkTheObject);
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    public IEnumerator BlinkTheObject()
    {
       // Debug.Log(" Cor started");
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 4));
            objectToBlink.enabled = true;  //Poorly named this refers to the object's meshrenderer - not the object
        yield return new WaitForSeconds(Random.Range(1,2));
        objectToBlink.enabled = false;

        }

    }
    public void OnDisable()
    {
        StopAllCoroutines();
    }
}
