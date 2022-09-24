using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenRandomGround : MonoBehaviour
{
    public GameObject[] groundObjects;
    public Transform surfaceParentTransform;
    public int numberGroundObjects = 10;
    // Start is called before the first frame update
    void Start()
    {
        GenerateTheGround();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    void GenerateTheGround()
    {
        int x = 0;
        for (int i = 0; i <= numberGroundObjects - 1; i++)
        {

            var position = new Vector3(Random.Range(-40f, 10f), -25f, Random.Range(-5.0f, 125f));
            Instantiate(groundObjects[x], position, Quaternion.identity, surfaceParentTransform);
            x++;
            if (x >= groundObjects.Length) x = 0;
        }
    }
}