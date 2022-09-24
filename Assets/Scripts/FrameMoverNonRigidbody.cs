using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameMoverNonRigidbody : MonoBehaviour
{

    bool roundInProgress,  fadeRoutinesStarted;  //flipMeshes,
    ScoreKeeper scoreKeeper;
    //GameObject gOSecondSegment, theClone;
    // MeshRenderer[] meshSegment;
    //public Material wormholeSegmentMaterial;
    public GameObject uniSphere, directionalLight, nasaBlueMarble; //, uniSphere2;
    //int runningFrameCount, lastRunningFrameCount;
    //float timeRoundStart, timeElapsed, zPosOfUniSphere;
    //float tenSecondTicks;
    Light _light;
    Color directionalLightColorValue; //, uniSphereColor;
    Material uniSphereMaterial, earthSphereMaterial;

    // hexColors deimplemented, replaced by shiftHexColors for a cheapo doppler effect 
    // readonly String[] hexColors = { "#E0B541FF", "#19B7B7FF", "#2520D8FF" , "#C80E29FF", "#A69C9DFF"} ;  // e0b541ff is natural(ish)
    //readonly String[] shiftHexColors = { "#E34A59FF", "#E29949FF", "#DFE249FF", "#4CE249FF", "#49D2E2FF", "#49ACE2FF" };  // e0b541ff is natural(ish)
    //                                       //red       orange          yellow      green        blue       darker blue        natural(ish)
    public int numberOfSegments = 12;
    Material[] wormholeSegmentMaterial;
    Color[] wormholeSegmentColor;
    //public Material wormholeOrange, wormholeYellow, wormholeGreen, wormholeBlue;
    GameObject[] wormholeSegments;
    //Vector3[] wormholePositions;
    bool[] wormholeMeshrendererToggle;
    //Vector3 trailingSegmentV3;
    int  wormholeSequence, trailingSegmentInt; //numberOfWormholeObjects,
    int wormholeObjectsCycled;
    // int shiftLightIndex;
    //int segmentColorChangeCount;
    String[] wormholeSegmentStrings;

    public float objectSpeed = 300;
    public float wormholeRecycleZPosition = 235f;
   // private bool recycleWormholeObject;
    Vector3 vector3OfObjectToBeRecycled;
    public AudioSource audiosource;
    //public AudioClip applause;
    public AudioClip earth10Seconds;
    //public AudioClip shieldsOff;
    public delegate void wormholeCycleCompleted();
    public static event wormholeCycleCompleted WormholeCycleCompleted;


 //   public GameObject matchPanel;
 //   private Color color;
 //if (ColorUtility.TryParseHtmlString("#09FF0064", out color))
 //        { matchPanel.GetComponent<Image>().color = color; }

// Start is called before the first frame update
void Start()
    {
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        Debug.Log("Hello from FrameMover... My frame Transform is: " + transform.position.z );
        _light = directionalLight.GetComponent<Light>();
        directionalLightColorValue = _light.color;
        uniSphereMaterial = GameObject.Find("UniSphere").GetComponent<MeshRenderer>().material;
        earthSphereMaterial = GameObject.Find("EarthSphere").GetComponent<MeshRenderer>().material;
        nasaBlueMarble.SetActive(false);
        //wormholeSegmentMaterial = GameObject.FindObjectsWithTag("WormholePipe").GetComponent<MeshRenderer>().material;
        wormholeMeshrendererToggle = new bool[numberOfSegments];
        wormholeSegments = new GameObject[numberOfSegments];
       // wormholePositions = new Vector3[numberOfSegments];
        wormholeSegmentMaterial = new Material[numberOfSegments];
        wormholeSegmentColor = new Color[numberOfSegments];
        wormholeSegments = GameObject.FindGameObjectsWithTag("WormholePipe");  //returns a fucking random order - so we sort :[
        wormholeSegmentStrings = new String[wormholeSegments.Length];
        for (int i = 0; i <= wormholeSegments.Length - 1; i++)
        {
            wormholeSegmentStrings[i] = wormholeSegments[i].gameObject.name;
        }
        Array.Sort(wormholeSegmentStrings);
        for (int i = 0; i <= wormholeSegments.Length - 1; i++)
        {
            wormholeSegments[i] = GameObject.Find(wormholeSegmentStrings[i]);
        }
        for (int i = 0; i <= wormholeSegments.Length - 1; i++)
        {
            //wormholePositions[i] = wormholeSegments[i].GetComponent<Transform>().position;
            wormholeSegmentMaterial[i] = wormholeSegments[i].GetComponent<MeshRenderer>().material;
            wormholeSegmentColor[i] =
                new Color(wormholeSegmentMaterial[i].color.r, wormholeSegmentMaterial[i].color.g, wormholeSegmentMaterial[i].color.b);
        }
    }
    private void Update()
    {
        //Start here the replacement code for eliminating rigidbody&movement and replace with transform.position movement 
        //AudioSource audiosource;
        if (roundInProgress)  //Set true on Go Button press  // set to false on destroyed OnPlayerMsgTypeReport event from PlayerCollisions.cs
        {
            MoveWormholeSegments();
           // timeElapsed = Time.time - timeRoundStart;
            if (scoreKeeper.roundTime <=  10f && !fadeRoutinesStarted)
            {
                fadeRoutinesStarted = true;
                StartCoroutine(FadeTheSpheresAndWormhole());
            }
            //BELOW 11 Lines unused &  commented on 7/19/22 
           // runningFrameCount = Time.frameCount;
           // int secondsGap = (runningFrameCount - lastRunningFrameCount) / Application.targetFrameRate;
           //// if (runningFrameCount - lastRunningFrameCount > Application.targetFrameRate * 13)    //here appTFR = 60, so every X(i.e. 10) secs
           // if (secondsGap > 12)
           // {
           //     // SetLightColor();
           //     // Debug.Log("CHANGE the Lights");
           //     segmentColorChangeCount++;
           //     //ChangeTheWormholeSegmentColors();
           //     lastRunningFrameCount = runningFrameCount;
           // }

            uniSphere.transform.Rotate(00.0f, 0.0f, 0.2f, Space.Self);
            uniSphere.transform.position += Vector3.back  * Time.deltaTime;
            //uniSphere2.transform.Rotate(00.2f, 0.0f, 0.0f, Space.Self);
            //uniSphere2.transform.position += Vector3.back * Time.deltaTime;
        }
    }

    private void MoveWormholeSegments()
    {
        for (int i = 0; i <= wormholeSegments.Length - 1; i++)
        {
            //Segments get Recycled by  void OnBackgroundObjectEnteredTrigger(GameObject other)
            wormholeSegments[i].transform.position += Vector3.back * Time.deltaTime * objectSpeed;
        }
    }
    IEnumerator FadeTheSpheresAndWormhole()
    {
        audiosource.clip = earth10Seconds;
        audiosource.Play();
        // Debug.Log("elapsed time = " + timeElapsed  + "   time.time = " + Time.time); //meaningless for us
        Debug.Log("skeeper time display = " + scoreKeeper.roundTime + "  Starting fades");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeOutTheUnisphere());
        StartCoroutine(FadeInTheEarthsphere());
        StartCoroutine(FadeOutTheWormhole());
    }
    //private void ChangeTheWormholeSegmentColors()
    //{
    //    //Debug.Log("Change The Segments' Color!!!");
    //    switch (segmentColorChangeCount)
    //    {
    //        case 1:
    //            Debug.Log("Change The Segments' Material  to Orange!!!");
    //            wormholeSegments[1].GetComponent<MeshRenderer>().material = wormholeOrange;
    //            wormholeSegments[3].GetComponent<MeshRenderer>().material = wormholeOrange;
    //            wormholeSegments[5].GetComponent<MeshRenderer>().material = wormholeOrange;
    //            //wormholeSegmentMaterial[1] = wormholeOrange;
    //            //wormholeSegmentMaterial[3] = wormholeOrange;
    //            //wormholeSegmentMaterial[5] = wormholeOrange;
    //            break;
    //        case 2:
    //            Debug.Log("Change The Segments' Material  to Yellow!!!");
    //            wormholeSegments[1].GetComponent<MeshRenderer>().material = wormholeYellow;
    //            wormholeSegments[3].GetComponent<MeshRenderer>().material = wormholeYellow;
    //            wormholeSegments[5].GetComponent<MeshRenderer>().material = wormholeYellow;
    //            //wormholeSegmentMaterial[1] = wormholeYellow;
    //            //wormholeSegmentMaterial[3] = wormholeYellow;
    //            //wormholeSegmentMaterial[5] = wormholeYellow;
    //            break;
    //        case 3:
    //            Debug.Log("Change The Segments' Material  to Green!!!");
    //            wormholeSegments[1].GetComponent<MeshRenderer>().material = wormholeGreen;
    //            wormholeSegments[3].GetComponent<MeshRenderer>().material = wormholeGreen;
    //            wormholeSegments[5].GetComponent<MeshRenderer>().material = wormholeGreen;
    //            //wormholeSegmentMaterial[1] = wormholeGreen;
    //            //wormholeSegmentMaterial[3] = wormholeGreen;
    //            //wormholeSegmentMaterial[5] = wormholeGreen;
    //            break;
    //        case 4:
    //            Debug.Log("Change The Segments' Material  to Blue!!!");
    //            wormholeSegments[1].GetComponent<MeshRenderer>().material = wormholeBlue;
    //            wormholeSegments[3].GetComponent<MeshRenderer>().material = wormholeBlue;
    //            wormholeSegments[5].GetComponent<MeshRenderer>().material = wormholeBlue;
    //            //wormholeSegmentMaterial[1] = wormholeBlue;
    //            //wormholeSegmentMaterial[3] = wormholeBlue;
    //            //wormholeSegmentMaterial[5] = wormholeBlue;
    //            break;
    //        default:
    //            Debug.Log("Change The Segments' Color  to INVALID value passed???");
    //            break;
    //    }
    //    for (int i = 0; i <= wormholeSegments.Length - 1; i++)
    //    {
    //        //wormholeSegmentMaterial[i] = wormholeSegments[i].GetComponent<MeshRenderer>().material;
    //        wormholeSegmentColor[i] =
    //            new Color(wormholeSegmentMaterial[i].color.r, wormholeSegmentMaterial[i].color.g, wormholeSegmentMaterial[i].color.b);
    //    }


    //}

    //private void SetLightColor()
    //{
    //    if (shiftLightIndex > shiftHexColors.Length - 1) shiftLightIndex--;  //push the index back if framecount varies?
    //    string shiftString = shiftHexColors[shiftLightIndex];
    //    var colorChange = _light.color;
    //    if (ColorUtility.TryParseHtmlString(shiftString, out colorChange))
    //    {
    //        _light.color = colorChange;
    //    }
    //    shiftLightIndex++;
    //}
    IEnumerator FadeOutTheWormhole()
    {
        float alphaColor = 1f;
        //Color tempColor = new Color(1f, 1f, 1f, alphaColor);  //blue with no transparency
        //Color tempColor = new Color(wormholeSegmentMaterial[0].color.r, wormholeSegmentMaterial[0].color.g, wormholeSegmentMaterial[0].color.b, alphaColor);
        //Color tempColor;// = new Color (wormholeSegmentColor[0].r, wormholeSegmentColor[0].g, wormholeSegmentColor[0].b, alphaColor);
        while (alphaColor > 0f)
        {
            // Debug.Log("In fadeout alphacolor = " + alphaColor);
            alphaColor -= .1f;
            //tempColor.a = alphaColor; //
            for (int i = 0; i <= wormholeSegments.Length - 1; i++)
            {
                //  tempColor = new Color(wormholeSegmentColor[i].r, wormholeSegmentColor[i].g, wormholeSegmentColor[i].b, alphaColor);
                //  wormholeSegmentMaterial[i].color = tempColor;
                wormholeSegmentColor[i].a = alphaColor;
                wormholeSegmentMaterial[i].color = wormholeSegmentColor[i]; //new test -- its good!
            }
           // wormholeSegmentMaterial.color = tempColor;
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator FadeInTheWormhole()
    {
        float alphaColor = .5f;

        while (alphaColor <= 1f)
        {
            alphaColor += .05f;
            for (int i = 0; i <= wormholeSegments.Length - 1; i++)
            {
                wormholeSegmentColor[i].a = alphaColor;
                wormholeSegmentMaterial[i].color = wormholeSegmentColor[i];
            }
            yield return new WaitForSeconds(1);
        }
    }
    private void RecycleWormholeObject(GameObject other)  //probably won't need bool applyForce since we already did it 
    {
        trailingSegmentInt = wormholeSegments.Length - 1; //so its always 4 (array length -1) unless array size changes
        _ = wormholeSequence <= 0 ? trailingSegmentInt = wormholeSegments.Length - 1 : trailingSegmentInt = wormholeSequence - 1;

        vector3OfObjectToBeRecycled = wormholeSegments[trailingSegmentInt].GetComponent<Transform>().position;  // trailingSegmentCurrent position - I hope 
        vector3OfObjectToBeRecycled += new Vector3(0, 0, wormholeRecycleZPosition);  //move  back 1 wormholeSegment 
                                                                            //Debug.Log(" trailingSegment pos = " + tempTrail.x + ",  " + tempTrail.y + ",  " + tempTrail.z + "   TRS = " + trailingSegmentInt);
        other.gameObject.transform.position = vector3OfObjectToBeRecycled;

         WormholeCycleCompleted?.Invoke(); //12/23 to change stars on each segment //this fucked it up 

        _ = wormholeSequence >= wormholeSegments.Length - 1 ? wormholeSequence = 0 : wormholeSequence++;

        //Debug.Log(" wormhole recy " + other.name + " " + tempRB.position.x + ",  " + tempRB.position.y + ",  " + tempRB.position.z);
    }
    void OnBackgroundObjectEnteredTrigger(GameObject other)  //The FRONT as player/viewer see it (the NEAR/front of screen, lower on Z axis)
    {
       if (other.gameObject.CompareTag("WormholePipe"))
        RecycleWormholeObject(other);                
    }
    

    private void OnRestartPressed()    //is this anymore??
    {
        //  Debug.Log("Restart pressed calling GenerateObjects from ON event");
        // GenerateObjects();
    }
    private void OnGoPressed()
    {
        //Debug.Log("GO pressed STARTROUND = " + Time.time );
        roundInProgress = true;
        StartCoroutine(FadeInTheWormhole());
        //timeRoundStart = Time.time;
        //  GenerateWormholeObject();
        //AddInitialForceToWormholeSegments(); // which will be multiple game objects
        //  addInitialForce = true;
    }

    private void BlankOutTheWormhole()
    {
        for (int i = 0; i <= wormholeMeshrendererToggle.Length - 1; i++)
        {
            wormholeMeshrendererToggle[i] = wormholeSegments[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    private void OnTimerExpired()  //here we can play around ...
    {
        //scoreKeeper.UpdateScore(0);  //here we need to calculate score/time bonus if any, and stop the round - we'll see...turns on BUTTON
        // scoreKeeper.EnableRestartButtonAndStopAudio();  // commented 6/15/22, sKeeper does this on timer expiration 
        roundInProgress = false;
        _light.color = new Color(1f, 1f, 1f, 1f);  // 5/30/22 set white light
        _light.intensity = .5f;
        BlankOutTheWormhole();


        Debug.Log("FrameMover From Scorekeeper reports SceneXTimerExpired!");
    }
    private void OnPlayerMsgTypeReport(int messageType, int count)
    {
        switch (messageType)
        {
            case 1:
                //Debug.Log("Fmover received a messageType call for type 1 -- Player Destroyed");
                roundInProgress = false;
                //BlankOutTheWormhole(); // 6/19/22 leave wormhole visible  
                StopAllCoroutines();
                break;
            case 2:
                //Debug.Log("FrameMover received a messageType call for type 2");
                break;
            default:
                Debug.Log("Fmover received an unknown messageType call");
                break;
        }
    }

    IEnumerator FadeOutTheUnisphere()
    {
        float alphaColor = 1f;
        Color tempColor = new Color(1f, 1f, 1f, alphaColor);

        while (alphaColor > 0f)
        {
            // Debug.Log("In fadeout alphacolor = " + alphaColor);
            alphaColor -= .1f;
            tempColor.a = alphaColor; //
            uniSphereMaterial.color = tempColor;
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator FadeInTheEarthsphere()
    {
        float alphaColor = 0f;
        Color tempColor = new Color(1f, 1f, 1f, alphaColor);

        while (alphaColor < 1f)
        {
            // Debug.Log("In fadeout alphacolor = " + alphaColor);
            alphaColor += .1f;
            tempColor.a = alphaColor; //
            earthSphereMaterial.color = tempColor;
            yield return new WaitForSeconds(1);
        }
        nasaBlueMarble.SetActive(true);
    }
    private void OnEnable()
    {
        ScoreKeeper.GoButtonPressed += OnGoPressed;
        ScoreKeeper.RestartButtonPressed += OnRestartPressed;
        ScoreKeeper.SceneXTimerExpired += OnTimerExpired;
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger += OnBackgroundObjectEnteredTrigger;
        PlayerCollisions.PlayerMsgTypeReport += OnPlayerMsgTypeReport;

        // RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger += OnBackgroundObjectEnteredRearTrigger;
    }
    private void OnDisable()
    {
        ScoreKeeper.GoButtonPressed -= OnGoPressed;
        ScoreKeeper.RestartButtonPressed -= OnRestartPressed;
        ScoreKeeper.SceneXTimerExpired -= OnTimerExpired;
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger -= OnBackgroundObjectEnteredTrigger;
        PlayerCollisions.PlayerMsgTypeReport -= OnPlayerMsgTypeReport;
        StopAllCoroutines();

        // RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger -= OnBackgroundObjectEnteredRearTrigger;
    }

}
