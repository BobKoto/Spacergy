using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class StaticBackground : MonoBehaviour
{
    public Transform staticBackgroundParentTransform, playerTransform, vCamTransform;
    Vector3 bGround, bGroundStart, bGroundEnd;
    public GameObject[] backGroundObjects;  //
   
    GameObject[] generatedObject;
    GameObject[] generatedObjectWave2;
    public static float objectSpeed = 90; // orig 200;
   
    GameObject theClone;

    float parentZStartPosition;
    Vector3 quadrant1, quadrant2, quadrant3, quadrant4;
    int q1Divisor = 3, q2Divisor = 6, q3Divisor = 6, q4Divisor = 3;   //dividing 300 for no. of objects

    //private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        quadrant1 = new Vector3(Random.Range(-250f, 200f),  Random.Range(12.0f, 175f), Random.Range(-550, 550));
        quadrant2 = new Vector3(Random.Range(-250f, -40f),  Random.Range(0f, 12f),     Random.Range(-550, 550));
        quadrant3 = new Vector3(Random.Range(30f, 200f),    Random.Range(0f, 12f),     Random.Range(-550, 550));
        quadrant4 = new Vector3(Random.Range(-250f, 200f),  Random.Range(-45f, 0f),    Random.Range(-550, 550));

        //bGroundStart = staticBackgroundParentTransform.position;  // 2/25/22  unused
        //bGroundEnd = new Vector3(0, 0, -172);                     // 2/25/22  unused
        //Debug.Log("player position.x = " + playerTransform.position.x);
        //Debug.Log("vcam position.z = " + vCamTransform.position.z);
        //parentZStartPosition = staticBackgroundParentTransform.position.z;  // 2/25/22  unused
        generatedObject = new GameObject[300];
        generatedObjectWave2 = new GameObject[200];
        GenerateObjects();
    }
    //                          GetAScramble is a central place to control/config the "hole" in our starfield 
    // quadrant 1: for (numobjs /4)   position = new Vector3(Random.Range(-250f, 200f),  Random.Range(12.0f, 175f), Random.Range(-550, 550));
    // quadrant 2: for (numobjs /4)   position = new Vector3(Random.Range(-250f, -40f),  Random.Range(0f, 12f),     Random.Range(-550, 550));
    // quadrant 3: for (numobjs /4)   position = new Vector3(Random.Range(30f, 200f),    Random.Range(0f, 12f),     Random.Range(-550, 550))
    // quadrant 4: for (numobjs /4)   position = new Vector3(Random.Range(-250f, 200f),  Random.Range(-45f, 0f),    Random.Range(-550, 550));
    // the ship is (-32, 6, 23) with a scale of x9      so ship should occupy -7 to 
    private Vector3 GetAScramble(int quadrant)
    {
        Vector3 newVector;
        float zFar = 550f, zNear = -1f;     //cam pos Z is -68.6f    Player is X -32, Y 4, Z 93.8
        float playerPositionX = playerTransform.position.x;
        float playerPositionY = playerTransform.position.y;

        switch (quadrant)    //fill screen while maintaining a void of stars around ship  (a hole!)
        {
         case 1: // LEFT
             newVector = new Vector3(Random.Range(playerPositionX-218, playerPositionX-18), Random.Range(playerPositionY-41, playerPositionY+171), Random.Range(zNear, zFar));
             break;
         case 2: // TOP MIDDLE
             newVector = new Vector3(Random.Range(playerPositionX-18, playerPositionX+18), Random.Range(playerPositionY+20, playerPositionY+155), Random.Range(zNear, zFar));
             break;
         case 3: // BOTTOM MIDDLE
             newVector = new Vector3(Random.Range(playerPositionX-18, playerPositionX+18),Random.Range(playerPositionY-20, playerPositionY-45), Random.Range(zNear, zFar));
             break;
         case 4: //  RIGHT
             newVector = new Vector3(Random.Range(playerPositionX+18, playerPositionX+186), Random.Range(playerPositionY-41, playerPositionY+171), Random.Range(zNear, zFar));
             break;
          
         default: newVector = new Vector3(Random.Range(-250f, 200f), Random.Range(12.0f, 175f), Random.Range(zNear, zFar));  //hope never
             break;
        } 
        return newVector;
    }
    void GenerateObjects()
    {
        Vector3 position;
        int x = 0;  //for the Prefab array
        int index = 0;  //this is LOCAL index (not the one referenced in Update() )
        for (int i = 0; i <= (generatedObject.Length /q1Divisor) -1; i++)  //QUADRANT ONE   LEFT  100
        {
            PositionObject(1);
        }

        for (int i = 0; i <= (generatedObject.Length / q2Divisor) - 1; i++)  //QUADRANT TWO   TOP MIDDLE   50
        {
            PositionObject(2);
        }

        for (int i = 0; i <= (generatedObject.Length / q3Divisor) - 1; i++)  //QUADRANT THREE    BOTTOM MIDDLE   50
        {
            PositionObject(3);
        }

        for (int i = 0; i <= (generatedObject.Length / q4Divisor) - 1; i++)  //QUADRANT FOUR    RIGHT  100
        {
            PositionObject(4);
        }
        void PositionObject(int quadrant)  //Here we are instantiating so we probably can't reuse
        {
            //position = new Vector3(Random.Range(-250f, 200f), Random.Range(12.0f, 175f), Random.Range(-200, 550));
            position = GetAScramble(quadrant);
            theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
            theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
            generatedObject[index] = theClone.gameObject;
            index++;
            x++;
            if (x >= backGroundObjects.Length) x = 0;
        }

    }
    private void Update()
    {
     Vector3 position;
     int index =0;

        UpdateObject(q1Divisor, 1);
        UpdateObject(q2Divisor, 2);
        UpdateObject(q3Divisor, 3);
        UpdateObject(q4Divisor, 4);

        void UpdateObject (int divisor, int quadrant)
        {
            for (int i = 0; i <= (generatedObject.Length / divisor) - 1; i++)
            {
                if (generatedObject[index].transform.position.z < -170)  //-170  is Camera position -- This scramble code replaces ScrambleObjects() Method
                {
                    position = GetAScramble(quadrant);
                    generatedObject[index].transform.SetPositionAndRotation(position, Quaternion.identity);
                }
                else  //keep pushing toward the front of screen
                    generatedObject[index].transform.Translate(Vector3.back * Time.deltaTime * objectSpeed);
                index++;
            }
        }

    }  
  
}

//void ScrambleObjects()
//{
//    float zValue;
//       for (int i = 0; i <= generatedObject.Length - 1; i++)
//       {
//        zValue = generatedObject[i].transform.position.z;
//        if (zValue < 0 )
//        {
//           var position = new Vector3(Random.Range(-250f, 200f), Random.Range(-45.0f, 175f), Random.Range(-550, -1));  // sets up a new random position
//           generatedObject[i].transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));  //change scale no matter where object is 
//           generatedObject[i].transform.SetPositionAndRotation(position, Quaternion.identity);
//        }

//       }
//}
// THe original generations position = new Vector3(Random.Range(-250f, 200f), Random.Range(-45.0f, 175f), Random.Range(-550, 550)); //randomZMax + 20); // ;//123 z

//                position = new Vector3(Random.Range(-250f, -40f), Random.Range(-45.0f, 0f), Random.Range(-550, 550));
//                position = new Vector3(Random.Range(-250f, -24f), Random.Range(-45.0f, 0f), Random.Range(-550, 550));
//                position = new Vector3(Random.Range(-250f, -40f), Random.Range(12f, 175f),  Random.Range(-550, 550))
//                position = new Vector3(Random.Range(-24f, 200f),  Random.Range(12f, 175f),  Random.Range(-550, 550));

// quadrant 1: for (numobjs /4)   position = new Vector3(Random.Range(-250f, 200f),  Random.Range(12.0f, 175f), Random.Range(-550, 550));
// quadrant 2: for (numobjs /4)   position = new Vector3(Random.Range(-250f, -40f),  Random.Range(0f, 12f),     Random.Range(-550, 550));
// quadrant 3: for (numobjs /4)   position = new Vector3(Random.Range(30f, 200f),    Random.Range(0f, 12f),     Random.Range(-550, 550))
// quadrant 4: for (numobjs /4)   position = new Vector3(Random.Range(-250f, 200f),  Random.Range(-45f, 0f),    Random.Range(-550, 550));



//int counter =0;    //THE ORIGINAL HERE DOWN
//int x = 0;  //for the Prefab array
//Vector3 position;
//for (int i = 0; i <= generatedObject.Length -1; i++)
//{
//    counter++;
//    // THe original generations position = new Vector3(Random.Range(-250f, 200f), Random.Range(-45.0f, 175f), Random.Range(-550, 550)); //randomZMax + 20); // ;//123 z 
//    if (counter % 2 == 1)  // place the upper in 2 segments skipping the X and Y               //places 2 Objects 
//    {
//        position = new Vector3(Random.Range(-250f, -40f), Random.Range(-45.0f, 0f), Random.Range(-550, 550)); //randomZMax + 20); // ;//123 z
//        theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//        theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//        generatedObject[i] = theClone.gameObject;
//        i++;                                                                       //remember "x" if we add different additional objects 
//        position = new Vector3(Random.Range(-24f, 200f), Random.Range(-45.0f, 0f), Random.Range(-550, 550)); //randomZMax + 20); // ;//123 z
//        theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//        theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//        generatedObject[i] = theClone.gameObject;

//    }
//    else  // place the lower in 2 segments skipping the X and Y                             //places 2 Objects 
//    {
//        position = new Vector3(Random.Range(-250f, -40f), Random.Range(12f, 175f), Random.Range(-550, 550)); //randomZMax + 20); // ;//123 z
//        theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//        theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//        generatedObject[i] = theClone.gameObject;
//        i++;                                                                       //remember "x" if we add different additional objects 
//        position = new Vector3(Random.Range(-24f, 200f), Random.Range(12f, 175f), Random.Range(-550, 550)); //randomZMax + 20); // ;//123 z
//        theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//        theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//        generatedObject[i] = theClone.gameObject;
//    }
//    x++;
//    if (x >= backGroundObjects.Length) x = 0;
//}  END THE ORIGINAL 
/*
 *   void Update()  //LERP EXAMPLE
    {
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);  //start and endMarker are Transforms //fractionOfJ is a float
    }
}
*/
//position = new Vector3(Random.Range(-250f, 200f), Random.Range(12.0f, 175f), Random.Range(-200, 550));  //replaced by GetAScramble()
// next 7 commented lines replaced by PositionObject()
//position = GetAScramble(1);
//theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//generatedObject[index] = theClone.gameObject;
//index++;
//x++;
//if (x >= backGroundObjects.Length) x = 0;
//position = new Vector3(Random.Range(-250f, -40f), Random.Range(0f, 12f), Random.Range(-200, 550));   //replaced by GetAScramble()
//next 7 commented lines replaced by PositionObject()
//position = GetAScramble(2);
//theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//generatedObject[index] = theClone.gameObject;
//index++;
//x++;
//if (x >= backGroundObjects.Length) x = 0;
//position = new Vector3(Random.Range(30f, 200f), Random.Range(0f, 12f), Random.Range(-200, 550));     //replaced by GetAScramble()
//next 7 commented lines replaced by PositionObject()
//position = GetAScramble(3);
//theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//generatedObject[index] = theClone.gameObject;
//index++;
//x++;
//if (x >= backGroundObjects.Length) x = 0;
//position = new Vector3(Random.Range(-250f, 200f), Random.Range(-45f, 0f), Random.Range(-200, 550));    //replaced by GetAScramble()
//next 7 commented lines replaced by PositionObject()
//position = GetAScramble(4);
//theClone = Instantiate(backGroundObjects[x], position, Quaternion.identity, staticBackgroundParentTransform);//, backgroundParentTransform);
//theClone.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
//generatedObject[index] = theClone.gameObject;
//index++;
//x++;
//if (x >= backGroundObjects.Length) x = 0;

/*     Before rearranging Quadrants
switch (quadrant)    //fill screen while maintaining a void of stars around ship  (a hole!)
    {
        case 1: //newVector = new Vector3(Random.Range(-250f, 200f), Random.Range(20.0f, 175f), Random.Range(zNear, zFar));
            newVector = new Vector3(Random.Range(playerPositionX - 218, playerPositionX+232), Random.Range(playerPositionY+8, 175f), Random.Range(zNear, zFar));
            break;
        case 2: //newVector = new Vector3(Random.Range(-250f, -50f), Random.Range(0f, 12f), Random.Range(zNear, zFar));
            newVector = new Vector3(Random.Range(playerPositionX-218, playerPositionX-18), Random.Range(0f, 12f), Random.Range(zNear, zFar));
            break;
        case 3: // newVector = new Vector3(Random.Range(40f, 200f), Random.Range(0f, 12f), Random.Range(zNear, zFar));
            newVector = new Vector3(Random.Range(playerPositionX +72, playerPositionX + 232), Random.Range(0f, 12f), Random.Range(zNear, zFar));
            break;
        case 4: // newVector = new Vector3(Random.Range(-250f, 200f), Random.Range(-45f, -15.0f), Random.Range(zNear, zFar));
            newVector = new Vector3(Random.Range(-250f, 200f), Random.Range(-45f, playerPositionY-4), Random.Range(zNear, zFar));
            break;

        default: newVector = new Vector3(Random.Range(-250f, 200f), Random.Range(12.0f, 175f), Random.Range(zNear, zFar));
            break;
    }     */
//for (int i = 0; i <= (generatedObject.Length /q1Divisor) - 1; i++)   //300 div q1Divisor(3) gives us 100
//{
//    if (generatedObject[index].transform.position.z < -170)  //-170  is Camera position -- This scramble code replaces ScrambleObjects() Method
//    {
//         position = GetAScramble(1);
//         generatedObject[index].transform.SetPositionAndRotation(position, Quaternion.identity);
//    }
//    else  //keep pushing toward the front of screen
//         generatedObject[index].transform.Translate(Vector3.back * Time.deltaTime * objectSpeed);
//    index++;
//}
//for (int i = 0; i <= (generatedObject.Length / q2Divisor) - 1; i++)
//{
//    if (generatedObject[index].transform.position.z < -170)  //-170  is Camera position -- This scramble code replaces ScrambleObjects() Method
//    {
//        position = GetAScramble(2);
//        generatedObject[index].transform.SetPositionAndRotation(position, Quaternion.identity);
//    }
//    else  //keep pushing toward the front of screen
//        generatedObject[index].transform.Translate(Vector3.back * Time.deltaTime * objectSpeed);
//    index++;
//}
//for (int i = 0; i <= (generatedObject.Length / q3Divisor) - 1; i++)
//{
//    if (generatedObject[index].transform.position.z < -170)  //-170  is Camera position -- This scramble code replaces ScrambleObjects() Method
//    {
//        position = GetAScramble(3);
//        generatedObject[index].transform.SetPositionAndRotation(position, Quaternion.identity);
//    }
//    else  //keep pushing toward the front of screen
//        generatedObject[index].transform.Translate(Vector3.back * Time.deltaTime * objectSpeed);
//    index++;
//}
//for (int i = 0; i <= (generatedObject.Length / q4Divisor) - 1; i++)
//{
//    if (generatedObject[index].transform.position.z < -170)  //-170  is Camera position -- This scramble code replaces ScrambleObjects() Method
//    {
//        position = GetAScramble(4);
//        generatedObject[index].transform.SetPositionAndRotation(position, Quaternion.identity);
//    }
//    else  //keep pushing toward the front of screen
//        generatedObject[index].transform.Translate(Vector3.back * Time.deltaTime * objectSpeed);
//    index++;
//}
