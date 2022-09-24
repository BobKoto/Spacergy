using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateHenryObjects : MonoBehaviour
{
    // Adapted from Level 1 GenRandomBackground.cs 
    // For Level 2 it handles object motion and recycling and listens to OnPlayerCollisionReport(GameObject objectHit)
    public GameObject[] backgroundObjects;
    GameObject[] generatedObjects;
    GameObject[] generatedSatellites;
    GameObject theClone;
    Transform backgroundParentTransform;
    public Transform playerTransform;

    ScoreKeeper scoreKeeper;
    // REMEMBER THE PUBLICS' LAST APPLICABLE VALUES ARE IN THE EDITOR!!!
    public int objectSpeed = 30;
    public float minPosX = -35f;
    public float maxPosX = 35f;
    public float minPosY = -5f;
    public float maxPosY = 5f;
    public float minPosZ = 400f;
    public float maxPosZ = 450f;
    float playerPositionX, playerPositionY, playerPositionZ ; 
    private int index;
    Vector3 objPosition;
    private bool timerExpired, roundStarted; // as reported by ScoreKeeper
    IEnumerator coroutine;
    AudioSource audioSource;
    public AudioClip applause, apert;
    TextMeshProUGUI collisionScoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        // Debug.Log("num OfBkObjs from scoreKeeper: " + scoreKeeper.ObjectsToGenerate);
        backgroundParentTransform = GameObject.Find("BackgroundParent").GetComponent<Transform>();
        generatedObjects = new GameObject[50];  //these should be reduced from 300 to like 30
                                                //       rb = new Rigidbody[30];
        audioSource = GetComponent<AudioSource>();
        //if (collisionScoreText)
        //{
            collisionScoreText = GameObject.Find("CollisionScoreText").GetComponent<TextMeshProUGUI>();
            collisionScoreText.enabled = false;
        //}
        playerPositionX = playerTransform.position.x;
        playerPositionY = playerTransform.position.y;
        playerPositionZ = playerTransform.position.z;  // = +94 with currently player movements along Z disabled

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
            if (x >= backgroundObjects.Length) x = 0;   //start repeating object generation
        }

    }
    private void Update()   //12/16 changed from Fixed to just Update
    {
        int objectsLeft = 0;
        for (int i = 0; i <= backgroundObjects.Length - 1; i++)
        {
            if (generatedObjects[i])   //exists and enabled - i guess 
            {
                if (timerExpired)  // 12/12  END THE ROUND and Destroy all active objects
                {
                    for (int z = 0; z <= backgroundObjects.Length - 1; z++)
                    {
                        if (generatedObjects[z])
                        {
                            if (!generatedObjects[z].gameObject.CompareTag("Satellite"))  //destroy all but satellites
                                Destroy(generatedObjects[i]);
                        }
                    }
                }
                else // timer is not expired so here we move active non-satellite objects 
                {
                    objectsLeft++;  //tally active objects
                    generatedObjects[i].transform.position += Vector3.back * Time.deltaTime * objectSpeed;
                    if (generatedObjects[i].transform.position.z < 0f)   // 2/12/22 for apparent Mobile issue of disappearing/drifting Sats
                    {
                        RecycleBackgroundObject(generatedObjects[i].gameObject, false);
                    }
                }
            }

        }
        //Here we need to know if any objects are left, and if all (BUT Satellites) are gone and time is left call GenerateObjects()
        if (roundStarted && !timerExpired && objectsLeft == 0) //satellitesInPlay)
        {
            GenerateObjects();  // this  also creates a new satellite in generatedObjects[] AND generatedSatellites[] indexed by satellitesInPlay
        }
    }

    //IEnumerator ApplyRemainingObjectsBonuses(int objectsRemaining)   //ONLY Satellites for now // 3/18/22 
    //{
    //    //We want to play a sound for each 100 increment of scoring; in this case 400 for each of objectsRemaining 
    //    int increment = (objectsRemaining * bonusIncrement) / 100;
    //    collisionScoreText.text = "Collecting Bonus for Satellites";
    //    collisionScoreText.enabled = true;
    //    for (int i = 1; i <= increment; i++)
    //    {
    //        scoreKeeper.UpdateScore(100);
    //        audioSource.Play();
    //        yield return new WaitForSeconds(.5f);
    //    }
    //    collisionScoreText.enabled = false;
    //}
    IEnumerator FlashBonus()   //
    {
        collisionScoreText.text = "Hit Wormhole Wall Lose 400!";
        collisionScoreText.enabled = true;
       // scoreKeeper.UpdateScore(9);
       // audioSource.Play();
        yield return new WaitForSeconds(3f);
        collisionScoreText.enabled = false;
    }
    private void OnPlayerCollisionReport(GameObject objectHit)
    {    
       // Debug.Log("GHO OnPlayerCollisionReport object hit is " + objectHit.name);
        if (objectHit.gameObject.CompareTag("WormholePipe"))
        {
            if (!timerExpired)
            {
                coroutine = FlashBonus();
                StartCoroutine(coroutine);
            }
        }
        else
        if (objectHit.gameObject.CompareTag("RogueTesla"))
        {
            coroutine = FlashBonus();
            StartCoroutine(coroutine);
        }
        else
        if (objectHit.gameObject.CompareTag("ShieldSphere"))
        {
            RecycleBackgroundObject(objectHit, false);
          // Debug.Log("OnPlayerCollisionReport object hit  " + objectHit.name );
        }
        else
        if (objectHit.gameObject.CompareTag("WormholeMenace"))   //if we got here the shield was ON  - I think/hope
        {
            RecycleBackgroundObject(objectHit, false);
           // scoreKeeper.UpdateScore(100);
        }

    }
    void RecycleBackgroundObject(GameObject other, bool applyForce)
    {
        objPosition = GetAScramble();
        other.gameObject.transform.SetPositionAndRotation(objPosition, other.gameObject.transform.rotation);  //works on theClone!!! Quaternion.identity
    }
    Vector3 GetAScramble()   //Consolidates both original and recycled randomness
    {
        //float playerPositionX = playerTransform.position.x;
        //float playerPositionY = playerTransform.position.y;
        //float playerPositionZ = playerTransform.position.z;  // = +94 with currently player movements along Z disabled
        Vector3 randomPosition;

        randomPosition = new Vector3
              (
               Random.Range(playerPositionX - minPosX, playerPositionX + maxPosX),        //35 matches original x range of 70 
               Random.Range(playerPositionY - minPosY, playerPositionY + maxPosY),
               Random.Range(minPosZ, maxPosZ));

        return randomPosition;
    }

    void OnBackgroundObjectEnteredTrigger(GameObject other)  //The FRONT as player/viewer see it (the NEAR/front of screen, lower on Z axis)
    {
        if (other.gameObject.CompareTag("WormholeMenace")) Debug.Log("Back trigger entered object = " + other.name);
        //      if (other.gameObject.CompareTag("Henry") || other.gameObject.CompareTag("WormholeMenace"))  //FrameMoverNonRigidBody.cs handles the wormhole  
        if (other.gameObject.CompareTag("WormholeMenace"))  //FrameMoverNonRigidBody.cs handles the wormhole   
        {
            Debug.Log("Back trigger entered object = " + other.name);
            RecycleBackgroundObject(other, false);                  // no need to apply force
        }

    }
    void OnBackgroundObjectEnteredRearTrigger(GameObject other)  //The REAR (the FAR, higher on Z axis)  
    {
        Debug.Log("OnBackgroundObjectEnteredRearTrigger called"); //not seeming to happen here 
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
        roundStarted = true;
    }
    private void OnTimerExpired()  //Assumes (for now) player won the round (didn't run out of shields/get destroyed)
    {
        Debug.Log("GenerateHenryObjects detected SceneXTimerExpired...");
        timerExpired = true;
        //collisionScoreText.text = "You made it home...";
        //collisionScoreText.enabled = true;
        //Debug.Log("satellitesInPlay = " + satellitesInPlay);
        //Here may be where we want to destroy all leftover objectsGenerated --BUT FixedUpdate MIGHT be better ... 

    }
    private void OnPlayerMsgTypeReport(int messageType, int count)
    {
        switch (messageType)
        {
            case 1:  //1 = player destroyed
                //Debug.Log("GenObjects received a messageType call for type 1 -- Player Destroyed");
                timerExpired = true; // 5/17/22 fake but it may be ok and work
                collisionScoreText.text = "Bang you're dead! Score Not Counted...";
                collisionScoreText.enabled = true;
                //  Debug.Log("GenObj calling scorekeeper.UpdateScore(0)");
                //  scoreKeeper.UpdateScore(0);  // 5/17/22 cause restart/continue to be enabled - I hope
                //scoreKeeper.EnableRestartButtonAndStopAudio();// commented 6/15/22 sKeeper gets same event and does the method call 
                break;
            case 2:  //2 = player made it home with (count) shields left over
                if (count > 0)
                {
                    Debug.Log("GHO.cs says Displaying You made it home with (some) Shields Msg...");
                    collisionScoreText.text = "You made it home with " + count + " shields left. Deducting 100 point penalty for each shield...";
                    audioSource.clip = apert;
                    audioSource.Play();
                }
                else
                {
                    Debug.Log("GHO.cs says Displaying Zero Shields Msg...");
                    collisionScoreText.text = "You made it home with zero shields left. Well done. 1,000 points bonus!";
                    audioSource.clip = applause;
                    audioSource.Play();
                }
                
                collisionScoreText.enabled = true;
                // Debug.Log("Skeeper received a messageType call for type 2");
                break;
            default:
               // Debug.Log("Skeeper received an unknown messageType call");
                break;


        }
    }
    private void OnEnable()
    {
        ScoreKeeper.SceneXTimerExpired += OnTimerExpired;
        ScoreKeeper.GoButtonPressed += OnGoPressed;
        ScoreKeeper.RestartButtonPressed += OnRestartPressed;
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger += OnBackgroundObjectEnteredTrigger;
        RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger += OnBackgroundObjectEnteredRearTrigger;
        PlayerCollisions.PlayerCollisionReport += OnPlayerCollisionReport;
        PlayerCollisions.PlayerMsgTypeReport += OnPlayerMsgTypeReport;

    }
    private void OnDisable()
    {
        ScoreKeeper.SceneXTimerExpired -= OnTimerExpired;
        ScoreKeeper.GoButtonPressed -= OnGoPressed;
        ScoreKeeper.RestartButtonPressed -= OnRestartPressed;
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger -= OnBackgroundObjectEnteredTrigger;
        RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger -= OnBackgroundObjectEnteredRearTrigger;
        PlayerCollisions.PlayerCollisionReport -= OnPlayerCollisionReport;
        PlayerCollisions.PlayerMsgTypeReport -= OnPlayerMsgTypeReport;

        StopAllCoroutines();
    }

}  //End Class
