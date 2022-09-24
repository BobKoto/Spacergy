using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlinkFrame : MonoBehaviour
{
    public float blinkRate = 1f;
    private MeshRenderer meshRenderer;
    private Color myColor;
    private IEnumerator coroutine;
    private int iColor;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        coroutine = BlinkBumper(blinkRate);
        StartCoroutine(coroutine);
    }
    IEnumerator BlinkBumper(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            meshRenderer.material.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(waitTime);
            meshRenderer.material.DisableKeyword("_EMISSION");
        }
    }
    void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
