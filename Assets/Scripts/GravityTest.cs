using TMPro;
using UnityEngine;
using UnityEngine.UI;

//     On 10/24/21 all controls EXCEPT Velocity taken out in prep for normal Input System movement -- so much for using Gravity :(
// RETIRED this Script on 7/13/22
public class GravityTest : MonoBehaviour
{
    ScoreKeeper scoreKeeper;
    public float timeRemaining = 60;
    public float objectSpeed = 600;
    GameObject gOStarFieldSpeedSlider, gOWormholeSpeedSlider, gOTimeRemainingSlider;  
    Slider starFieldSpeedSlider, wormholeSpeedSlider, timeRemainingSlider;          
    TextMeshProUGUI wormholeSpeedValueText;
    TextMeshProUGUI xGravityValueText, yGravityValueText, zGravityValueText, gravityMinMaxText,
        hoverIndicator, hoverButtonText, dynamicGravityValueText, starFieldSpeedValueText;

    void Start()           // Start is called before the first frame update
    {
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        Debug.Log(" Hello from GravityTest.cs" + "  round time =" + scoreKeeper.initialRoundTime);
        gOTimeRemainingSlider = GameObject.Find("TimeRemainingSlider");
        if (gOTimeRemainingSlider) timeRemainingSlider = gOTimeRemainingSlider.GetComponent<Slider>();
        if (timeRemainingSlider)
        {
          //  Debug.Log("we have time slider ");
            timeRemaining = scoreKeeper.initialRoundTime;
         //   starFieldSpeedValueText = GameObject.Find("StarFieldSpeedText").GetComponent<TextMeshProUGUI>();
         //   starFieldSpeedValueText.text = starFieldSpeedSlider.value.ToString();
        }

        gOStarFieldSpeedSlider = GameObject.Find("StarFieldSpeedSlider");
        if (gOStarFieldSpeedSlider) starFieldSpeedSlider = gOStarFieldSpeedSlider.GetComponent<Slider>();
        if (starFieldSpeedSlider)
        {
            starFieldSpeedValueText = GameObject.Find("StarFieldSpeedText").GetComponent<TextMeshProUGUI>();
            starFieldSpeedValueText.text = starFieldSpeedSlider.value.ToString();
        }
       
        gOWormholeSpeedSlider = GameObject.Find("WormholeSpeedSlider");
        if (gOWormholeSpeedSlider)  wormholeSpeedSlider = gOWormholeSpeedSlider.GetComponent<Slider>();
        if (wormholeSpeedSlider)
        {
              wormholeSpeedValueText = GameObject.Find("WormholeSpeedText").GetComponent<TextMeshProUGUI>();
              wormholeSpeedValueText.text = wormholeSpeedSlider.value.ToString();
        }
    }
    void Update()
    {
        if (timeRemainingSlider)
        {

            if (timeRemaining >= 0 && scoreKeeper.roundInProgress)
            {
                timeRemaining -= Time.deltaTime;
                timeRemainingSlider.value = timeRemaining;
            }
        }
    }

    public void OnWormholeVelocityChanged(float value)
    {
        // Debug.Log("Wormhole slider moved :  " + value);
        string v = wormholeSpeedSlider.value.ToString("##.#");
        wormholeSpeedValueText.text = v;
        objectSpeed = wormholeSpeedSlider.value;  //invalid here see next line 
        //here we should set an event to end up being sensed in FrameMover's FixedUpdate

    }
    public void OnStarFieldSpeedChanged(float value)
    {
      //  Debug.Log("StarFieldSlider moved :  " + value);

        string v = starFieldSpeedSlider.value.ToString("##.#");
        starFieldSpeedValueText.text = v;
        StaticBackground.objectSpeed = starFieldSpeedSlider.value;
    }

}  //end class 


//public void OnButtonIncreaseGPressed()
//{
//  //  Debug.Log("increase pressed");
//    dynamicSlidersValues += 1f;
//    if (dynamicSlidersValues <= maxGravityForce)
//    {
//        OverrideSlidersValues(dynamicSlidersValues);
//    }
//    else
//    {
//        dynamicSlidersValues = 9.5f;
//        //   Debug.Log("Play a Beep Here   -- tried to go ABOVE " + maxGravityForce);
//        can.Play();
//    }
//}
//public void OnButtonDecreaseGPressed()
//{
//  //  Debug.Log("DECREASE PRESSED");
//    dynamicSlidersValues -= 1f;
//    if (dynamicSlidersValues >= minGravityForce)
//    {
//    OverrideSlidersValues(dynamicSlidersValues);
//    }
//    else
//    {
//        dynamicSlidersValues = .5f;
//        OverrideSlidersValues(dynamicSlidersValues);
//        //     Debug.Log("Play a Beep Here   -- tried to go below " + minGravityForce + "  GravForce set to " + dynamicSlidersValues);
//        can.Play();
//    }

//}


//private void OnDisable()
//{
//    StopAllCoroutines();
//}
// Physics.gravity = new Vector3(xGravityValue, yGravityValue, 0f);   //starts ship at a standstill  //See 10/24 note above class declaration 

// gOStarFieldSpeedSlider = GameObject.Find("StarFieldSpeedSlider");

//gOWormholeSpeedSlider = GameObject.Find("WormholeSpeedSlider");
//if (WormholeSpeedSlider)
//{
//    WormholeSpeedSlider = gOWormholeSpeedSlider.GetComponent<Slider>();
//    Debug.Log("WormholeSpeedSlider FOUND ");
//}
//else Debug.Log("WormholeSpeedSlider NOT FOUND ");
//gOxAxisSlider = GameObject.Find("XAxisSlider");
//if (xAxisSlider) xAxisSlider = gOxAxisSlider.GetComponent<Slider>();

//gOyAxisSlider = GameObject.Find("YAxisSlider");
//if (yAxisSlider) yAxisSlider = gOyAxisSlider.GetComponent<Slider>();

//gOzAxisSlider = GameObject.Find("ZAxisSlider");
//if (zAxisSlider) zAxisSlider = gOzAxisSlider.GetComponent<Slider>();

//gOGForceSlider = GameObject.Find("GForceSlider");
//if (GForceSlider) GForceSlider = gOGForceSlider.GetComponent<Slider>();

//if (xGravityValueText)
//{
//    xGravityValueText = GameObject.Find("XGravityText").GetComponent<TextMeshProUGUI>();   //do we want this anymore ??? would need to replicate for Y axis
//    xGravityValueText.text = xAxisSlider.value.ToString();
//}

//if (yGravityValueText)
//{
//    yGravityValueText = GameObject.Find("YGravityText").GetComponent<TextMeshProUGUI>();   //do we want this anymore ??? would need to replicate for Y axis
//    yGravityValueText.text = yAxisSlider.value.ToString();
//}

//if (zGravityValueText)
//{
//    zGravityValueText = GameObject.Find("ZGravityText").GetComponent<TextMeshProUGUI>();   //do we want this anymore ??? would need to replicate for Y axis
//    zGravityValueText.text = zAxisSlider.value.ToString();
//}

//if (wormholeSpeedValueText)
//{
//    wormholeSpeedValueText = GameObject.Find("WormholeSpeedText").GetComponent<TextMeshProUGUI>();
//    wormholeSpeedValueText.text = WormholeSpeedSlider.value.ToString();
//}
//else Debug.Log("wormholeVelocityText NOT FOUND so not true");
// if (dynamicGravityValueText) dynamicGravityValueText = GameObject.Find("DynamicGravityValueText").GetComponent<TextMeshProUGUI>();
// gOyAxisSlider.SetActive(false);  //Start of testing leveling of game 
//if (gOzAxisSlider)  gOzAxisSlider.SetActive(false);

//// OverrideSlidersValues(dynamicSlidersValues);   //See 10/24 note above class declaration 

//playerRigidBody = myPlayer.gameObject.GetComponent<Rigidbody>();
//myPlayerSphere = GameObject.Find("Player/PlayerSphere");
//playerSphereMaterial = myPlayerSphere.GetComponent<Renderer>().material;

//playerSphereMeshRenderer = myPlayerSphere.GetComponent<MeshRenderer>();

//if (hoverIndicator)
//{
// hoverIndicator = GameObject.Find("HoverIndicator").GetComponent<TextMeshProUGUI>();
// hoverIndicator.enabled = false;
//}
//if (hoverButtonImage)  hoverButtonImage = GameObject.Find("HoverButton").GetComponent<Image>(); ;

//if (hoverButtonText) hoverButtonText = GameObject.Find("HoverButton").GetComponentInChildren<TextMeshProUGUI>();
//can = GetComponent<AudioSource>();

//public void OnWormholeVelocityChanged(float value)
//{
//    // Debug.Log("Wormhole moved :  " + value);

//    string v = WormholeSpeedSlider.value.ToString("##.####");
//    wormholeSpeedValueText.text = v;
//    // StaticBackground.objectSpeed = StarFieldSpeedSlider.value;
//    //here we should set an event to end up being sensed in FrameMover's FixedUpdate

//}
//public void OnXGravitySliderChanged(float value)
//{
//    // Debug.Log("xGravitySlider mived :  " + value);
//    //   gravityValue = gravitySlider.value;
//    string v = xAxisSlider.value.ToString("##.####");
//    xGravityValueText.text = v;

//    Physics.gravity = new Vector3(xAxisSlider.value, yAxisSlider.value, 0); // zAxisSlider.value);   // z = 0 Because zAxisSlider is disabled;
//                                                                            // Physics.gravity = new Vector3(xAxisSlider.value, 0, 0);   //Test zeroing on axis change on Slider - NOT SO GOOD
//}
//public void OnYGravitySliderChanged(float value)
//{
//    // Debug.Log("yGravitySlider mived :  " + value);
//    //   gravityValue = gravitySlider.value;
//    string v = yAxisSlider.value.ToString("##.####");
//    yGravityValueText.text = v;

//    Physics.gravity = new Vector3(xAxisSlider.value, yAxisSlider.value, zAxisSlider.value);
//    // Physics.gravity = new Vector3(0, yAxisSlider.value, 0);//Test zeroing on axis change on Slider - NOT SO GOOD
//}
//public void OnZGravitySliderChanged(float value)
//{
//    // Debug.Log("zGravitySlider mived :  " + value);
//    //   gravityValue = gravitySlider.value;
//    string v = zAxisSlider.value.ToString("##.####");
//    zGravityValueText.text = v;

//    Physics.gravity = new Vector3(xAxisSlider.value, yAxisSlider.value, zAxisSlider.value);
//    //Physics.gravity = new Vector3(0, 0, zAxisSlider.value);//Test zeroing on axis change on Slider - NOT SO GOOD
//}
//public void OnGForceSliderChanged(float value)
//{ /*      //See 10/24 note above class declaration 
//        dynamicSlidersValues = value;
//        OverrideSlidersValues(dynamicSlidersValues);
//        */
//}


//public void OnZeroGButtonPress()
//{
//    //   GameObject clonedCube;
//    //   Debug.Log("Zero button pressed.");
//    xAxisSlider.value = 0f;
//    yAxisSlider.value = 0f;
//    if (zAxisSlider) zAxisSlider.value = 0f;
//    Physics.gravity = new Vector3(0, 0, 0);
//    xGravityValueText.text = "0"; // gravitySlider.value.ToString("##.####");
//    yGravityValueText.text = "0";
//    if (zGravityValueText) zGravityValueText.text = "0";
//    // clonedCube = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
//    // // Pick a random, saturated and not-too-dark color
//    //clonedCube.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

//}
//public void OnHoverButtonPress()   //Hover renamed to "Full Stop" in gameplay
//{
//    HoverPressed?.Invoke(); //Control blinking 
//                            //  Debug.Log("HOVER PRESSED! ");//Freezing position OR UNFreezing (i hope)   " + "  ShipIsHovering  " + shipIsHovering); //+ "Blink is " + shipBulb.Blink 

//    HoverShip();

//}
//void HoverShip()
//{
//    // yield return null;
//    if (!shipIsHovering)   //Make it hover 
//    {
//        shipIsHovering = true;
//        //   Debug.Log("shipIsHovering " + shipIsHovering + "  Should be Freezing position HERE");

//        playerRigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
//        //next line undoes FreezePosition?????
//        // playerRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; //don't rotate ever/for now :)
//        //Try
//        playerRigidBody.freezeRotation = true;   // THIS SEEMS TO WORK!!!!!!!!!!!!!!!!!!!!!! finally :{
//        hoverIndicator.enabled = true;
//        hoverButtonText.text = "Engage";
//        hoverButtonImage.color = Color.green;
//    }
//    else     //Stop hovering 
//    {
//        shipIsHovering = false;
//        //     Debug.Log("shipIsHovering " + shipIsHovering);
//        playerRigidBody.constraints = RigidbodyConstraints.None;  //Let our ship move 
//        playerRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; //for now :)
//                                                                                                                                                          //    playerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;

//        hoverIndicator.enabled = false;
//        hoverButtonText.text = "Full Stop";
//        hoverButtonImage.color = Color.red;
//    }
//}
//private void OverrideSlidersValues(float newGravitySliderValue)
//{
//    if (xAxisSlider)
//    {
//        xAxisSlider.maxValue = newGravitySliderValue; xAxisSlider.minValue = -newGravitySliderValue;  //Display uses X axis -- ALL 3 SHOULD ALWAYS AGREE! (unless we want to do weird things!)
//    }
//    if (yAxisSlider)
//    {
//        yAxisSlider.maxValue = newGravitySliderValue; yAxisSlider.minValue = -newGravitySliderValue;
//    }

//    if (zAxisSlider)
//    {
//        zAxisSlider.maxValue = newGravitySliderValue;
//        zAxisSlider.minValue = -newGravitySliderValue;
//    }
//    if (gravityMinMaxText) gravityMinMaxText = GameObject.Find("MinMaxText").GetComponent<TextMeshProUGUI>();
//    if (dynamicGravityValueText) dynamicGravityValueText.text = newGravitySliderValue.ToString("##.###");
//    // Debug.Log("New Grav Value =  " + newGravitySliderValue.ToString());

//}
/* Commented out per 10/24 notice above 
[Tooltip ("Normal Gravity = -9.81. The -1 to +1 range is for slow play.")]
[Range(-1, 1)]
public float xGravityValue = 0f;  //-9.81 is Default for Gravity
public float yGravityValue = 0f;  //-9.81 is Default for Gravity
public float dynamicSlidersValues = 5f;
*/
//Slider xAxisSlider, yAxisSlider, zAxisSlider, GForceSlider;

//GameObject gOxAxisSlider, gOyAxisSlider, gOzAxisSlider, gOGForceSlider, gOWormholeSpeedSlider;
//public GameObject myPlayer, myPlayerSphere;  //See 10/24 note above class declaration re  hoverButton;
//Rigidbody playerRigidBody;
//Material playerSphereMaterial;
//MeshRenderer playerSphereMeshRenderer;
//IEnumerator hoverShipCoroutine;
//bool shipIsHovering = false;

//Image hoverButtonImage;

//BlinkShipBulb shipBulb;
//public delegate void OnHoverButtonPressDelegate();
//public static event OnHoverButtonPressDelegate HoverPressed;

//Audio Stuff
//public AudioSource can;


