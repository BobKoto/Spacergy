using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBlocker : MonoBehaviour      // 3/18/22   this whole script may be defunct
{
    public AudioSource frontPlaneContact;
    public GameObject frame;
    private MeshRenderer meshRenderer;
    public MeshRenderer[] meshRenderers;
    private IEnumerator coroutine;
    private float blinkRate = 1f;
    ScoreKeeper scoreKeeper;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderers = frame.GetComponentsInChildren<MeshRenderer>();
      //  Debug.Log("MeshRenderers count..." + meshRenderers.Length);
        Debug.Log("Hello from PlaneBlocker..." );
        //foreach (MeshRenderer xRender in meshRenderers)
        //    Debug.Log("mre info..." + xRender.material);

        frontPlaneContact = GetComponent<AudioSource>();
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        frontPlaneContact.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
      //  Debug.Log("Trigger enter...  coroutine");
        frontPlaneContact.Play();
        foreach (MeshRenderer xRender in meshRenderers)           
              xRender.material.EnableKeyword("_EMISSION");

        coroutine = ResetEmission(blinkRate);
        StartCoroutine(coroutine);
    }
 
    IEnumerator ResetEmission(float waitTime)
    {
       yield return new WaitForSeconds(waitTime);
       foreach (MeshRenderer xRender in meshRenderers)
           xRender.material.DisableKeyword("_EMISSION");
    }
  
    private void OnTriggerExit(Collider other)
    {
     //   Debug.Log("collided with " + other);
     //  if (other.CompareTag ( "Player"))  scoreKeeper.UpdateScore(10);  // 3/18/22   this whole script may be defunct
    }
  
    void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
