using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Stars : MonoBehaviour    //Find in GravityTest
{
    public Transform staticBackgroundParentTransform, uniSphereTransform;  
    public GameObject[] backGroundObjects;  //
    GameObject[] generatedObject;
    GameObject theClone;
    float uniSpherePositionX;
    float uniSpherePositionY;
   // int updateFramesInterval = Application.targetFrameRate;
    int updateFrames;
    bool roundInProgress = true, inPostRoundStarDisplay;
    // Start is called before the first frame update
    void Start()
    {
        uniSpherePositionX = uniSphereTransform.position.x;
        uniSpherePositionY = uniSphereTransform.position.y;
        generatedObject = new GameObject[50];
        GenerateObjects();
    }
    private Vector3 GetAScramble()   //hard coded and ugly but it works
    {
        Vector3 newVector;
        float zFar = 500f, zNear = 490f;     //cam pos Z is -68.6f    Player is X -32, Y 4, Z 93.8

        newVector = new Vector3(Random.Range(uniSpherePositionX - 90, uniSpherePositionX + 90), // 6/28/22 change 75 to 90
                                Random.Range(uniSpherePositionY - 65, uniSpherePositionY + 65), // 6/28/22 change 50 to 65
                                Random.Range(zNear, zFar));

        return newVector;
    }

    private Vector3 GetAScramble(float zRange, float xRange )   //for use at very end of successful round 
    {
        Vector3 newVector;
       // float zFar = 500f, zNear = 490f;     //cam pos Z is -68.6f    Player is X -32, Y 4, Z 93.8

        newVector = new Vector3(Random.Range(uniSpherePositionX - xRange, uniSpherePositionX + xRange),
                                Random.Range(uniSpherePositionY - 150, uniSpherePositionY + 150), //6/28/22 change 70 to 150 
                                zRange);
                               // Random.Range(zNear, zFar));

        return newVector;
    }
    void GenerateObjects()    //Retains ability to instantiate multiple Star(like) objects in the prefab array
    {
        Vector3 position;
        int x = 0;  //for the Prefab array
        int index = 0;  //this is LOCAL index (not the one referenced in Update() )
        for (int i = 0; i <= (generatedObject.Length) - 1; i++)  //QUADRANT ONE   LEFT  100
        {
            PositionObject();
        }
        void PositionObject()  //Here we are instantiating so we probably can't reuse
        {
            //position = new Vector3(Random.Range(-250f, 200f), Random.Range(12.0f, 175f), Random.Range(-200, 550));
            if (roundInProgress)
            {
            position = GetAScramble();
            }
            else
            {
                position = GetAScramble(550f, 350f); //6/28/22 change xrange 200 to 350
            }
            theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);
            theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
            generatedObject[index] = theClone.gameObject;
            index++;
            x++;
            if (x >= backGroundObjects.Length) x = 0;
        }
    }
    private void Update()
    {
        if (roundInProgress || inPostRoundStarDisplay)
        {
            updateFrames++;
            if (updateFrames >= Application.targetFrameRate)  //30 
            {
                var scaler = Random.Range(1, 3);
                for (int i = 0; i <= (generatedObject.Length) - 1; i += scaler)
                {
                    generatedObject[i].transform.localScale = new Vector3(scaler, scaler, scaler);// (Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
                }

                updateFrames = 0;
            }
        }

    }
    private void OnWormholeCycleCompleted()
    {
        //hello !
        Vector3 position;
        int index = 0;
        if (roundInProgress)
        {
            for (int i = 0; i <= (generatedObject.Length) - 1; i++)
            {
                position = GetAScramble();
                generatedObject[index].transform.SetPositionAndRotation(position, Quaternion.identity);
                index++;  //??
            }
        }
    }
    void OnTimerExpired()
    {
        roundInProgress = false; 
        DestroyStars();    //or do something else 
        GenerateObjects();
        inPostRoundStarDisplay = true;
        StartCoroutine(PostRoundStarDisplay());
    }

    void DestroyStars()
    {
        for (int i = 0; i <= (generatedObject.Length) - 1; i++)
        {
            Destroy(generatedObject[i]); //.transform.SetPositionAndRotation(position, Quaternion.identity);
        }
    }
    IEnumerator PostRoundStarDisplay()
    {
        yield return new WaitForSeconds(10f);
        inPostRoundStarDisplay = false;
    }
    private void OnEnable()
    {
       FrameMoverNonRigidbody.WormholeCycleCompleted += OnWormholeCycleCompleted;
        ScoreKeeper.SceneXTimerExpired += OnTimerExpired;
    }
    private void OnDisable()
    {
        FrameMoverNonRigidbody.WormholeCycleCompleted -= OnWormholeCycleCompleted;
        ScoreKeeper.SceneXTimerExpired -= OnTimerExpired;
        StopAllCoroutines();
    }
}
