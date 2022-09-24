using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameMover : MonoBehaviour 
{
    //Transform firstFrame;
    //float transX, transY, transZ;
    //Vector3 newTransformPosition, startingTransformPosition;  //Points to tansform of GObj script - REMEMBER! using them overrides everything
    //Vector3 nextTransformPosition;
    //Quaternion startingRotation;
    //bool movingToFront = true;
    //[Tooltip("Default is .01f   a bit slow.")]
    //public float frameMoveSpeed = 25f;
    //public float zPosMax = 200f;
    //public float zPosMin = -70f;
    //public float positiveZOffset = 180;
    bool flipMeshes;
    ScoreKeeper scoreKeeper;
    GameObject gOSecondSegment, theClone;
    MeshRenderer[] meshSegment;
    Material[] materials;
    //public GameObject[] wormholeObjects;
    GameObject[] wormholeSegments;
    Vector3[] wormholePositions;
    Vector3 trailingSegmentV3;
    int numberOfWormholeObjects, wormholeSequence , trailingSegmentInt;
    Rigidbody[] rb;     //
    Vector3[] clonePosition;
    int wormholeObjectsCycled;
    String[] wormholeSegmentStrings;
    public int wormholeForce = 2000;
    public float wormholeRecycleZPosition = 235f;
    private bool addInitialForce, recycleWormholeObject;
    Vector3 objectToBeRecycled;
    Rigidbody rigidBodyToBeRecycled;

    //Vector3 brakeForce;


    // Start is called before the first frame update
    void Start()
    {
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        // Debug.Log("Hello from FrameMover... My frame Transform is: " + transform.position.z );
        //materials = new Material[5];
        //materials[0] = GetComponent<MeshRenderer>().material;
        //materials[1] = GameObject.Find("TorusFrameSecondSegment").GetComponent<MeshRenderer>().material;
        //meshSegment = new MeshRenderer[10];
        //meshSegment[0] = GetComponent<MeshRenderer>();
        //meshSegment[1] =  GameObject.Find("TorusFrameSecondSegment").GetComponent<MeshRenderer>();
        //gOSecondSegment = GameObject.Find("TorusFrameSecondSegment");

        // wormholeObjects = new GameObject[5];  //overrides the editor and the running result, stupid!!! 
        //numberOfWormholeObjects = wormholeObjects.Length;
        rb = new Rigidbody[5];
        wormholeSegments = new GameObject[5];
        wormholePositions = new Vector3[5];
        // clonePosition = new Vector3[5];

        // gameSurfaceMaterial = meshRenderer.material;
        //transZ = transform.position.z;
        //newTransformPosition = transform.position;  //remember transform.position here = game obj holding script
        //startingTransformPosition = transform.position;
        //startingRotation = transform.rotation;
        //nextTransformPosition = startingTransformPosition;
        //nextTransformPosition.z = startingTransformPosition.z + positiveZOffset;
        //brakeForce = new Vector3(0, 0, 0);
        wormholeSegments = GameObject.FindGameObjectsWithTag("WormholeSegment");  //returns a fucking random order - so we sort :[
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
            wormholePositions[i] = wormholeSegments[i].GetComponent<Transform>().position;
            rb[i] = wormholeSegments[i].GetComponent<Rigidbody>();
        }

    }
    private void FixedUpdate()
    {
        if (addInitialForce)  //Set true on Go Button press
        {
            for (int i = 0; i <= wormholeSegments.Length - 1; i++)
            {
               // wormholePositions[i] = wormholeSegments[i].GetComponent<Transform>().position;
                //      Debug.Log("wormhole seg " + wormholeSegments[i].name + " " + wormholePositions[i].x + ",  " +wormholePositions[i].y + ",  " +wormholePositions[i].z);
              //  rb[i] = wormholeSegments[i].GetComponent<Rigidbody>();
                rb[i].AddForce(0, 0, -1 * wormholeForce);
            }
            addInitialForce = false;
        }
        if (recycleWormholeObject)
        {
            rigidBodyToBeRecycled.position = objectToBeRecycled;   //set in 
        }
        recycleWormholeObject = false;
        //Start here the replacement code for eliminating rigidbody&movement and replace with transform.position movement 
    }

    private void RecycleWormholeObject(GameObject other, bool applyForce)  //probably won't need bool applyForce since we already did it 
    {
        trailingSegmentInt = wormholeSegments.Length - 1; //so its always 4 (array length -1) unless array size changes
        _ = wormholeSequence <= 0 ? trailingSegmentInt = wormholeSegments.Length - 1 : trailingSegmentInt = wormholeSequence - 1;

        objectToBeRecycled =  wormholeSegments[trailingSegmentInt].GetComponent<Transform>().position;  // trailingSegmentCurrent position - I hope 
        objectToBeRecycled += new Vector3(0, 0, wormholeRecycleZPosition);  //move  back 1 wormholeSegment 
                   //Debug.Log(" trailingSegment pos = " + tempTrail.x + ",  " + tempTrail.y + ",  " + tempTrail.z + "   TRS = " + trailingSegmentInt);
        rigidBodyToBeRecycled = other.gameObject.GetComponent<Rigidbody>();
        recycleWormholeObject = true;
       // rigidBodyToBeRecycled.position = objectToBeRecycled;        // moved to FixedUpdate()

        _ = wormholeSequence >= wormholeSegments.Length - 1 ? wormholeSequence = 0 : wormholeSequence++;   

                  //Debug.Log(" wormhole recy " + other.name + " " + tempRB.position.x + ",  " + tempRB.position.y + ",  " + tempRB.position.z);
    }
    void OnBackgroundObjectEnteredTrigger(GameObject other)  //The FRONT as player/viewer see it (the NEAR/front of screen, lower on Z axis)
    {
     //   other.gameObject.SetActive(false);  //apparently does not cancel force 
        
        RecycleWormholeObject(other, false);                  // no need to apply force

    }
    //private void GenerateWormholeObject()
    //{
    //    for (int i = 0; i <= numberOfWormholeObjects - 1; i++)
    //    {
    //        var wormholePosition = SetWormholeObjectPosition(i);
    //        theClone = Instantiate(wormholeObjects[i], wormholePosition, startingRotation);
    //        clonePosition[i] = theClone.transform.position;
    //        rb[i] = theClone.GetComponent<Rigidbody>();
    //        rb[i].AddForce(0, 0, -1 * wormholeForce);
    //    }
    //}
    //private Vector3 SetWormholeObjectPosition(int positionToSet)
    //{
    //    return positionToSet switch   //The newfangled case statement :[
    //    {
    //        0 => startingTransformPosition,
    //        1 => nextTransformPosition,
    //        _ => startingTransformPosition,
    //    };
    //}

    private void OnRestartPressed()    //is this anymore??
    {
        //  Debug.Log("Restart pressed calling GenerateObjects from ON event");
       // GenerateObjects();
    }
    private void OnGoPressed()
    {
        // Debug.Log("GO pressed calling GenerateObjects from ON event");
      // roundInProgress = true;
        //  GenerateWormholeObject();
        //AddInitialForceToWormholeSegments(); // which will be multiple game objects
        addInitialForce = true;
    }
    private void OnTimerExpired()
    {
        Debug.Log(" Scorekeeper reports Scene2TimerExpired!");
    }
    private void OnEnable()
    {
        ScoreKeeper.GoButtonPressed += OnGoPressed;
        ScoreKeeper.RestartButtonPressed += OnRestartPressed;
        ScoreKeeper.SceneXTimerExpired += OnTimerExpired;
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger += OnBackgroundObjectEnteredTrigger;
       // RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger += OnBackgroundObjectEnteredRearTrigger;
    }
    private void OnDisable()
    {
        ScoreKeeper.GoButtonPressed -= OnGoPressed;
        ScoreKeeper.RestartButtonPressed -= OnRestartPressed;
        ScoreKeeper.SceneXTimerExpired -= OnTimerExpired;
        BackgroundObjectTrigger.OnBackgroundObjectEnteredTrigger -= OnBackgroundObjectEnteredTrigger;
       // RearBackgroundObjectTrigger.OnBackgroundObjectEnteredRearTrigger -= OnBackgroundObjectEnteredRearTrigger;
    }

}
// *********************************** End Class ************************************ 
//   zollner2Material = Resources.Load<Material>("ZollnerMat");
//       opticalIllusion2Material = Resources.Load<Material>("OpticalIllusion2");
//    case 1: meshRenderer.material = gameSurfaceMaterial; break;
//case 2:
//        transform1 = GameObject.Find("Vortex1").GetComponent<Transform>();
//        transform1.position = v3SurfaceEye;//swap positions 
//        transform2 = GameObject.Find("SurfaceEye").GetComponent<Transform>(); 
//        transform2.position = v3Vortex1; //swap positions 
//        meshRenderer.material = zollner2Material;
//        break;
//case 3:
//       transform1 = GameObject.Find("Vortex1").GetComponent<Transform>();
//       transform1.position = v3Vortex1; //swap to original
//       transform2 = GameObject.Find("SurfaceEye").GetComponent<Transform>();
//       transform2.position = v3SurfaceEye; //swap to original
//       meshRenderer.material = opticalIllusion2Material; break;
//default: meshRenderer.material = gameSurfaceMaterial; break;
// Update is called once per frame
//void Update()
//{
//    if (roundInProgress)
//       {
//          // movingToFront = true;
//          transZ -= frameMoveSpeed * Time.deltaTime;  //inch toward the front
//          newTransformPosition = new Vector3(transform.position.x, transform.position.y, transZ);
//          transform.position = newTransformPosition;

//    if (transZ <= zPosMin)  // now we have moved all the way forward -- so set it all back 
//      {
//            //movingToFront = false;
//          flipMeshes = !flipMeshes;
//            if (flipMeshes)
//            {
//                meshSegment[0].material = materials[1];
//                meshSegment[1].material = materials[0];
//            }
//            else
//            {
//                meshSegment[1].material = materials[1];
//                meshSegment[0].material = materials[0];
//            }
//          transform.SetPositionAndRotation(startingTransformPosition, startingRotation);
//          transZ = startingTransformPosition.z;
//          scoreKeeper.UpdateScore(50);
//       }
//    }
//}
//private void OnCollisionEnter(Collision collision)
//{
//     Debug.Log("WormholeFrame hit  " + collision.gameObject.name);

//}
//private void OnTriggerEnter(Collider other)
//{
//    Debug.Log("WormholeFrame hit trigger " + other.gameObject.name);

//}

//if (wormholeSequence == 0) trailingSegmentInt = 4;  // the if/else code here works  
//else

//    if (wormholeSequence == 1) trailingSegmentInt = 0;
//else
//    if (wormholeSequence == 2) trailingSegmentInt = 1;
//else
//    if (wormholeSequence == 3) trailingSegmentInt = 2;
//else
//    if (wormholeSequence == 4) trailingSegmentInt = 3;

    //private void AddInitialForceToWormholeSegments()   //moved to FixedUpdate()  fired off by bool addInitialForce
    //{

    //    for (int i = 0; i <= wormholeSegments.Length - 1; i++)
    //    {
    //        wormholePositions[i] = wormholeSegments[i].GetComponent<Transform>().position;
    //  //      Debug.Log("wormhole seg " + wormholeSegments[i].name + " " + wormholePositions[i].x + ",  " +wormholePositions[i].y + ",  " +wormholePositions[i].z);
    //        rb[i] = wormholeSegments[i].GetComponent<Rigidbody>();
    //        rb[i].AddForce(0, 0, -1 * wormholeForce);
    //    }
    //}
