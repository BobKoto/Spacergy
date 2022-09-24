using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkOnCollision : MonoBehaviour    // 3/18/22   this whole script may be defunct
{
    private MeshRenderer meshRenderer;
    ScoreKeeper scoreKeeper;
   
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello BlinkOnCollision...");
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collider)
    { 
         
      //  Debug.Log("trigger entered with..." + collider.name);
        if (collider.gameObject.CompareTag("Player"))
        {
           Debug.Log("player entered triggered collider  ...");
            meshRenderer.material.EnableKeyword("_EMISSION");
        }

    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
          //  Debug.Log("player exited triggered collider  ...");
            meshRenderer.material.DisableKeyword("_EMISSION");
           // scoreKeeper.UpdateScore(10);     // 3/18/22   this whole script may be defunct
        }

    }
}
