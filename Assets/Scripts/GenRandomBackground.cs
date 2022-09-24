using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
// ///// For starters this scripts generates objects for ship to intercept and score points // and recycles unintercepted objects for replay
public class GenRandomBackground : MonoBehaviour
{
    public GameObject[] backgroundObjects;
    public GameObject[] satelliteObjects; //
    GameObject[] generatedObjects;
    GameObject[] generatedSatellites;
    GameObject theClone; 
  //  Rigidbody[] rb;
    Transform backgroundParentTransform;
    public Transform playerTransform;
    LevelChanger levelChanger;
    ScoreKeeper scoreKeeper;
    //    public int backgroundForce = 2000;
    public int objectSpeed = 90;
    public int objectSpeedIncrement = 30;
    public int satellitesReminingToAchieveNextLevel = 6;
    public int scoreToAchieveNextLevel = 20000;
 //   int numberOfBackgroundObjects = 5;
    public float minPosX = -60f;
    public float maxPosX = 10f;
    public float minPosY = 5f;
    public float maxPosY = 6f;
    public float minPosZ = 400f;
    public float maxPosZ = 450f;
    private int index;
    public static int satellitesInPlay;
    private int satelliteIndex, objectsHit;
   // private int bonusIncrement = 400;  //score to add for each bonus eligible object (satellites here for now) 

    Vector3 objPosition;
    private bool timerExpired, timerExpiredProcessed; // as reported by ScoreKeeper or UI Button
    IEnumerator coroutine;
    AudioSource audioSource;
    TextMeshProUGUI collectBonusText, satelliteCount, speedNumber;
    //GameObject satNotches; //2/10/22 revert to a single GO - from a GO array
    //String[] satNotchesStrings;
    // Start is called before the first frame update
    void Start()
    {
        levelChanger = GameObject.Find("LevelChanger").GetComponent<LevelChanger>();
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
       // Debug.Log("num OfBkObjs from scoreKeeper: " + scoreKeeper.ObjectsToGenerate);
        backgroundParentTransform = GameObject.Find("BackgroundParent").GetComponent<Transform>();
        generatedObjects = new GameObject[30];  //these should be reduced from 300 to like 30
 //       rb = new Rigidbody[30];
        generatedSatellites = new GameObject[30];
        satellitesInPlay = 0;
        // yes it does Debug.Log("From GenRanBkrnd PlayerPosition x " + playerTransform.position.x + " y " + playerTransform.position.y + "  z " + playerTransform.position.z);
        audioSource = GetComponent<AudioSource>();
        collectBonusText = GameObject.Find("CollectBonus").GetComponent<TextMeshProUGUI>();
        collectBonusText.enabled = false;
        satelliteCount = GameObject.Find("SatelliteCount").GetComponent<TextMeshProUGUI>();
        speedNumber = GameObject.Find("SpeedNumber").GetComponent<TextMeshProUGUI>();
        speedNumber.text = objectSpeed.ToString("###");
        StaticBackground.objectSpeed = objectSpeed; // 2/27/22 added to restore speed to StaticBackground (which continues to run?)

    }

    private void Update()   //12/16 changed from Fixed to just Update
    {
        if (timerExpired && !timerExpiredProcessed)  // 12/12  END THE ROUND and Destroy all active junk objects
        {
            CleanUpObjects();
        }
        if (!timerExpired)  //Play the game 
        {
            MoveObjects();
        }
    }
    public void GenerateObjects()
    {
        int x = 0;
        index = 0;
        // numberOfBackgroundObjects = scoreKeeper.ObjectsToGenerate;  // DEPENDENCY is this needed anymore ? was only for scoring (defunct) 
        for (int i = 0; i <= backgroundObjects.Length - 1; i++)    //Object gen'd = something * the size of the objectsArray
        {
            objPosition = GetAScramble();
            theClone = Instantiate(backgroundObjects[x], objPosition, backgroundObjects[x].transform.rotation, backgroundParentTransform); //parent is just to keep editor uncluttered
            index = i;
            generatedObjects[index] = theClone;  // replacement for line above - NOW we are changing positions!!! 
            x++;
            if (x >= backgroundObjects.Length) x = 0;   //start repeating object generation (won't work for Satellites anymore)
        }
        //Now gen a single Satellite 
        GenerateSatellite();
    }
    void GenerateSatellite()
    {
        objPosition = GetAScramble();
        theClone = Instantiate(satelliteObjects[0], objPosition, satelliteObjects[0].transform.rotation, backgroundParentTransform);
       // if (satellitesInPlay <= satNotches.Length - 1) satNotches[satellitesInPlay].SetActive(true);   //limit to 6 //2/10/22 deimp

        satellitesInPlay++;
        satelliteCount.text = satellitesInPlay.ToString("##");
        //Debug.Log("FROM GenerateSatellite() gen NEW Satellite satsInPlay = " + satellitesInPlay + "  IID " + theClone.transform.GetInstanceID());
        generatedSatellites[satelliteIndex] = theClone;
        satelliteIndex++;
    }
    void MoveObjects()
    {
        for (int i = 0; i <= backgroundObjects.Length - 1; i++)
        {
            if (generatedObjects[i])   //exists and enabled - i guess 
            {
                if (!generatedObjects[i].gameObject.CompareTag("Satellite"))  // if NOT a satellite move it 
                {
                    generatedObjects[i].transform.position += Vector3.back * Time.deltaTime * objectSpeed;
                }
                if (generatedObjects[i].transform.position.z < 0f)   // 1/14/22 for apparent Mobile issue of disappearing/drifting objects
                {
                    RecycleBackgroundObject(generatedObjects[i].gameObject, false);
                }
            }
        }
        // here we move active satellites 'cause they're in a separate Array 
        if (satellitesInPlay > 0)    
        {
            for (int z = 0; z <= satelliteIndex; z++)  // generatedSatellites.Length - 1; z++)  //1/6/22 narrow down 
            {
                if (generatedSatellites[z])   //exists and enabled - i guess //2/12/22 to fix pos check
                {
                    generatedSatellites[z].transform.position += Vector3.back * Time.deltaTime * objectSpeed;
                    if (generatedSatellites[z].transform.position.z < 0f)   // 2/12/22 for apparent Mobile issue of disappearing/drifting Sats
                    {
                        RecycleBackgroundObject(generatedSatellites[z].gameObject, false);
                    }
                }
            }
        }
    }
    void CleanUpObjects()
    {
        for (int z = 0; z <= backgroundObjects.Length - 1; z++)
        {
            if (generatedObjects[z] && !generatedObjects[z].gameObject.CompareTag("Satellite"))  //if active/enabled and not satellite
            {
               // Debug.Log("destroying " + generatedObjects[z].name);
                Destroy(generatedObjects[z]);
            }
        }
        if (satellitesInPlay > 0)   //Destroy satellites left  ADDED 2/24/22 
        {
            for (int z = 0; z <= satelliteIndex; z++)  // generatedSatellites.Length - 1; z++)  //1/6/22 narrow down 
            {
                if (generatedSatellites[z])  
                {
                    Destroy(generatedSatellites[z]);
                }
            }
        }
        timerExpiredProcessed = true;
    }
    private void OnPlayerCollisionReport(GameObject objectHit)
    {
        if (objectHit.gameObject.CompareTag("RogueTesla"))
        {
            //coroutine = FlashBonus();
            //StartCoroutine(coroutine);
        }
        else
        if (objectHit.gameObject.CompareTag("Satellite"))
        {
            satellitesInPlay--;   //if (satellitesInPlay > 0) this can't be possible 
            Destroy(objectHit);
            satelliteCount.text = satellitesInPlay.ToString("#0");
        }
        if (!objectHit.CompareTag("Player") && !objectHit.gameObject.CompareTag("Satellite")) //player reports a spurious/dumb programmer collision at start 
        {                                                                                 //so just destroy/recycle anything not Player - that's the game!
            objectsHit++;
            if (objectsHit >= backgroundObjects.Length && !timerExpired)  //added && !timeExpired 1/2
            {
                // Debug.Log(" objects hit = " + objectsHit );
                GenerateSatellite();  //removed if (!timerExpired) 1/2
                objectsHit = 0;
                objectSpeed += objectSpeedIncrement;
                StaticBackground.objectSpeed = objectSpeed;
                speedNumber.text = objectSpeed.ToString("###");
            }
            RecycleBackgroundObject(objectHit, false);     //here we need to replace destroy with recycle     DONE!     
                                                           //   Debug.Log("GenRandomBackground objectSpeed = " + objectSpeed);
        }
    }
    void RecycleBackgroundObject(GameObject other, bool applyForce)
    {
        objPosition = GetAScramble();
        other.gameObject.transform.SetPositionAndRotation(objPosition, other.gameObject.transform.rotation);  //works on theClone!!! Quaternion.identity
  //     if (applyForce) other.GetComponent<Rigidbody>().AddForce(0, 0, -1 * backgroundForce); //force is cumulative if obj unimpeded. Do this only for objs that drift/bounce forward (+z)
    }
    Vector3 GetAScramble()   //Consolidates both original and recycled randomness
    {
        float playerPositionX = playerTransform.position.x;
        float playerPositionY = playerTransform.position.y;
        float playerPositionZ = playerTransform.position.z;  // = +94 with currently player movements along Z disabled
        Vector3 randomPosition;

        randomPosition = new Vector3
              (                                                             
               Random.Range(playerPositionX-35, playerPositionX+35),        //35 matches original x range of 70 
               Random.Range(playerPositionY+5,playerPositionY-5),
               Random.Range(minPosZ, maxPosZ));   

        return randomPosition;
    }

    void OnBackgroundObjectEnteredTrigger(GameObject other)  //The FRONT as player/viewer see it (the NEAR/front of screen, lower on Z axis)
    {
        RecycleBackgroundObject(other, false);                  // no need to apply force
    }
    void OnBackgroundObjectEnteredRearTrigger(GameObject other)  //The REAR (the FAR, higher on Z axis)  
    {
        RecycleBackgroundObject(other, true);                    //object "bounced/drifted forward" - apply force to get object moving backward (-z) again
    }
    private void OnRestartPressed()
    {
      //  Debug.Log("Restart pressed calling GenerateObjects from ON event");
        GenerateObjects();
    }
    private void OnGoPressed()
    {
       // Debug.Log("GO pressed calling GenerateObjects from ON event");
        GenerateObjects();
       // roundStarted = true;
    }
    private void OnTimerExpired()
    {
        Debug.Log("GenRandomBackground detected SceneXTimerExpired...");
        timerExpired = true;
       // Debug.Log("OnTimerExpired()  satellitesInPlay = " + satellitesInPlay);
    }
    private void OnEnable()
    {
        ScoreKeeper.SceneXTimerExpired += OnTimerExpired;
        ScoreKeeper.GoButtonPressed += OnGoPressed;
        ScoreKeeper.RestartButtonPressed += OnRestartPressed; 
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger += OnBackgroundObjectEnteredTrigger;
        RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger += OnBackgroundObjectEnteredRearTrigger;
        PlayerCollisions.PlayerCollisionReport += OnPlayerCollisionReport;
    }
    private void OnDisable()
    {
        ScoreKeeper.SceneXTimerExpired -= OnTimerExpired;
        ScoreKeeper.GoButtonPressed -= OnGoPressed;
        ScoreKeeper.RestartButtonPressed -= OnRestartPressed;
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger -= OnBackgroundObjectEnteredTrigger;
        RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger -= OnBackgroundObjectEnteredRearTrigger;
        PlayerCollisions.PlayerCollisionReport -= OnPlayerCollisionReport;
        StopAllCoroutines();
    }

}  //End Class
