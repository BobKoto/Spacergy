using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkShipBulb : MonoBehaviour
{
    MeshRenderer shipBulb;
    IEnumerator blinkTheBulb;
    float blinkTime = .5f;

    public bool Blink { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello from BlinkShipBulb.cs");
        Blink = true;
        shipBulb = GetComponent<MeshRenderer>();
        blinkTheBulb = BlinkBulb(blinkTime);
        StartCoroutine(blinkTheBulb);
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    public IEnumerator BlinkBulb(float blinkTime)
    {
        while (true)
        {
          //  Debug.Log("in while true loop blink is" + Blink);
            if (Blink)
            {
             //   Debug.Log("blink is" + Blink + "  Inside if blink");
                yield return new WaitForSeconds(blinkTime);
               shipBulb.material.DisableKeyword("_EMISSION");
               yield return new WaitForSeconds(blinkTime);
               shipBulb.material.EnableKeyword("_EMISSION");
            }
            else  //Blink is false 
            {
                yield return new WaitForSeconds(blinkTime);
             //   shipBulb.material.DisableKeyword("_EMISSION");
            }

        }

    }

   //void OnEnable()
   // {
   //     GravityTest.HoverPressed += HoverPressedReceived;
   // }
   // private void OnDisable()
   // {
   //     GravityTest.HoverPressed -= HoverPressedReceived;
   //     StopAllCoroutines();
   // }
   // public void HoverPressedReceived()
   // {
   //    // Debug.Log("Hover Press Received in blinkbulb! ");
   //     Blink = !Blink;
   // }
}
