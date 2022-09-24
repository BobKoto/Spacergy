using System.Collections;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;
//using UnityEngine.InputSystem;

//using System.Collections.Generic;
//using System.Numerics;
//using System.Runtime.CompilerServices;
//using UnityEngine.InputSystem.Controls;
//using UnityEngine.InputSystem.EnhancedTouch;
//using UnityEngine.SceneManagement;
//using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.InputSystem
{
    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// WARNING KeyBoard Mouse scheme in Input Assets files appears to break Gamepad (Touch on Canvas)  -- I had to remove it 
    /// But it also seems it would be/work OK to run directly on Mobile Device 
    /// UPDATE Keyboard ONLY scheme seems to work switching properly between Keyboard and Gamepad scheme ...
    /// </summary>
    public class ShipMover : MonoBehaviour
    {
        [SerializeField]
        public static float moveSpeed = 20; //was 10 b4 prefab 
        public static float rotateSpeed = 15; //60 w/ori eulers
        float burstSpeed = 10;     //WAS PUBLIC without the "= 10"
        public GameObject projectile;

        private bool m_Charging;

        private Vector3 m_Rotation;
        private Vector2 m_Look;

        private Vector3 m3_Move;

        //From W project
        private bool currentInput;
        private Vector3 inputDirection;
        private Vector3 smoothDirection;
        private Vector3 rawDirection;
        Camera mainCamera;   //WAS PUBLIC
        //End from W project


        public float jumpHeight = 50f;
        CharacterController characterController;
        public float gravity = 50f;
       // public float laserForce = 1500f;
        Vector3 move = Vector3.zero;  //WAS PUBLIC
        Vector3 lookMove;             //WAS PUBLIC 
        bool jumpPressed;
        float scaledMoveSpeed, smoothingSpeed;
        public static Vector3 playerStartPos;
        public static Transform playerPosT;
        bool should_Move;

        //LineRenderer line;


        protected void OnEnable()
        {
            //  EnhancedTouchSupport.Enable();  //Commented to UNuse Touchscreen
        }
        protected void OnDisable()
        {
            //   EnhancedTouchSupport.Disable();  //Commented to UNuse Touchscreen
        }
        public void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            characterController = GetComponent<CharacterController>();
            playerPosT = GetComponent<Transform>();
            playerStartPos = playerPosT.position;
            //line = gameObject.GetComponent<LineRenderer>();  LINE was a laser -- maybe useful in future  
            //line.enabled = false;

        }
        public void Update()
        {
            CalculateMovementInput(); //Just checks  if not Vector3.zero
        }
 /*       public void FixedUpdate()
        {
            // Update orientation (Look) first, then move. Otherwise move orientation will lag behind by one frame.
            Look(m_Look); // Ori working button //NOT Touchscreen --- BUT Canvas RightStick !!  //working 5/1/21
            Move(); // Moved from Just after Look(m_Look) above     //sets off CharacterController Move 

        }  // end Update   */

        void CalculateMovementInput()
        {
            if (m3_Move == Vector3.zero)
            {
                currentInput = false;
            }
            else if (m3_Move != Vector3.zero)
            {
                currentInput = true;
            }
        }
        void CalculateDesiredDirection()
        {
            //Camera Direction   //HOWEVER in this script the camera moves w/the player -- ORI W project has stationary Camera  -- so it is funky
            var cameraForward = mainCamera.transform.forward;
            var cameraRight = mainCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            rawDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;  //inputDirection is set by OnMove event
                                                                                               // like so:     inputDirection = new Vector3(m3_Move.x, 0, m3_Move.y); 
                                                                                               // or like so:  inputDirection = new Vector3(m3_Move.x, m3_Move.y, 0); 
        }
        void ConvertDirectionFromRawToSmooth()
        {
            if (currentInput == true)
            {
                smoothDirection = Vector3.Lerp(smoothDirection, rawDirection, Time.deltaTime * smoothingSpeed);  //try using variant of this in Move
            }
            else if (currentInput == false)
            {
                smoothDirection = Vector3.zero;
            }

        }
        void MoveThePlayer()   //MOVES A RIGIDBODY so save 
        {
            if (currentInput == true)
            {
                move.Set(smoothDirection.x, 0f, smoothDirection.z);
                // move = move.normalized * movementSpeed * Time.deltaTime;    //probably can't just relay into Move?
                // ////////////////////////////  playerRigidbody.MovePosition(transform.position + movement);  // have to adapt this MovePosition for Character controller Move 
            }
        }
        private void Move()   //REVAMP this to MOVE ONLY on X and Y (up/down) Axes   BK 10/23/21
        {

            move.Set(m3_Move.x, m3_Move.y, m3_Move.z);

            if (move.sqrMagnitude > 0.01 || !characterController.isGrounded)  // if moved or we jumped(not grounded) do this below
            {
                should_Move = true;
                scaledMoveSpeed = moveSpeed * Time.fixedDeltaTime;
                if (!characterController.isGrounded)    //if we are not grounded apply gravity
                    //ORI  move = new Vector3(m3_Move.x, -gravity * Time.fixedDeltaTime, m3_Move.y);  // Try this by Me (changed y(up/down) move value to z (back/forth) ) ok good
                    move.Set(move.x, -gravity * Time.fixedDeltaTime, move.y);  // Try this by Me (changed y(up/down) move value to z (back/forth) ) ok good
                else
                    //ORI move = new Vector3(m3_Move.x, 0, m3_Move.y);             // otherwise move along -- Should we Lerp here?
                    move.Set(move.x, 0, move.y);             // otherwise move along -- Should we Lerp here?
            }
            if (jumpPressed && characterController.isGrounded)
                move.y = jumpHeight;

            if (should_Move || jumpPressed || !characterController.isGrounded)
            {
                smoothDirection = Vector3.Lerp(smoothDirection, move, Time.deltaTime * smoothingSpeed);
                smoothDirection = transform.TransformDirection(smoothDirection);// needed to move in "Look"  Worldspace direction 
                characterController.Move(smoothDirection * scaledMoveSpeed);
                should_Move = false;
                jumpPressed = false;
            }
            move = Vector3.zero;// 
        }
        void TurnThePlayer()
        {
            if (currentInput == true) //Added   '&& smoothDirection != Vector3.zero' 1/20/21 to stop 'Look rotation viewing vector is zero' warning
            {
                float sqrLen = smoothDirection.sqrMagnitude;
                if (sqrLen > .01f)                     //(smoothDirection != Vector3.zero)
                {
                    Quaternion newRotation = Quaternion.LookRotation(smoothDirection * Time.deltaTime);

                    //Debug.Log("newRotation " + newRotation);
                    // Debug.Log("smoothDirection " + smoothDirection.x + "   " +  smoothDirection.y + "   " + smoothDirection.z);
                    transform.rotation = newRotation; // have to adapt this for Character controller Move OR just do it
                }
            }
        }
        public void OnMove(InputAction.CallbackContext context)
        // public void OnMove()
        {
            m3_Move = context.ReadValue<Vector2>();
            inputDirection = new Vector3(m3_Move.x, 0, m3_Move.y);  //Maybe NOT this Simple?
                                                                    // inputDirection = new Vector3(m3_Move.x, m3_Move.y, 0);  //Maybe NOT this Simple?
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            m_Look = context.ReadValue<Vector2>();
            // playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0); // Test from sample code 
            //  Debug.Log("OnLook triggered..." + context.ReadValue<Vector2>()); //Temporary
        }
        private void Look(Vector2 rotate)  //original working 5/1/21
        {
            // float tempRotSpeedBoost = 0f; //temp replace for scaledRotateSpeed
            if (rotate.sqrMagnitude < 0.01)
                return;
            // Debug.Log("rotate:  " + rotate);
            var scaledRotateSpeed = rotateSpeed * Time.fixedDeltaTime;
            m_Rotation.y += rotate.x * scaledRotateSpeed;
            m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, 0, 0); //ori -89,89  //next -20,20  //Maybe just delete the line, 4get rotate x 
            transform.localEulerAngles = m_Rotation;  //ORI works fwiw
                                                      //  Physics.SyncTransforms(); //no help
                                                      //Debug.Log("rotate:  " + rotate +       "  m_Rotation:  " + m_Rotation);
                                                      //  transform.Rotate(0, rotate.y * rotateSpeed , 0, Space.World);
                                                      // characterController.Move(m_Rotation ); // not quite!
        }

        private IEnumerator BurstFire(int burstAmount)
        {
            for (var i = 0; i < burstAmount; ++i)
            {
                Fire();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void Fire()
        {
            var transform = this.transform;
            var newProjectile = Instantiate(projectile);
            newProjectile.transform.position = transform.position + transform.forward * 0.6f;
            newProjectile.transform.rotation = transform.rotation;
            const int size = 1;
            newProjectile.transform.localScale *= size;
            newProjectile.GetComponent<Rigidbody>().mass = Mathf.Pow(size, 3);
            newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
            newProjectile.GetComponent<MeshRenderer>().material.color =
                new Color(Random.value, Random.value, Random.value, 1.0f);
        }
      /*  private void FireLaser()
        {
            //  Debug.Log("FireLaser Performed Phase No slowtap");

            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            line.enabled = true;


            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            line.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                // Debug.Log(hit.collider);
                line.SetPosition(1, hit.point);
                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(transform.forward * laserForce, hit.point);
                    line.enabled = false;
                }
            }

            else
            {
                //     print("no Hit");
                line.SetPosition(1, ray.GetPoint(300));

            }
        } */
        public void OnFire(InputAction.CallbackContext context)  //The FIRE Button was pressed
        {
            //   Debug.Log("OnFIRE CALLED......."); //Temporary
            // Debug.Log("OnFire Context" + context);
            switch (context.phase)
            {

                case InputActionPhase.Performed:
                    //  Debug.Log("Input action phase Performed");
                    if (context.interaction is SlowTapInteraction)
                    {
                        Debug.Log("slow tap in Performed Phase  Start Burst Coroutine with " + (int)(context.duration * burstSpeed));

                        StartCoroutine(BurstFire((int)(context.duration * burstSpeed)));
                    }
                    else
                    {
                        //FireLaser();  //maybe future use 
                    }
                    m_Charging = false;
                    break;

                case InputActionPhase.Started:
                    if (context.interaction is SlowTapInteraction)
                    {
                        Debug.Log("Input Phase STARTED  and SlowTapInteraction is TRUE?");
                        m_Charging = true;
                    }
                    break;

                case InputActionPhase.Canceled:
                    m_Charging = false;
                    //   Debug.Log("Input CANCELLED or just Done");
                    break;
            }
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            //   Debug.Log("OnJump CALLED..." );
            //  Debug.Log("end Jump");
            //  if (characterController.isGrounded  || transform.position.y < .2f)
            {

                switch (context.phase)
                {

                    case InputActionPhase.Performed:
                        // if (characterController.isGrounded || transform.position.y < .2f) jumpPressed = true;
                        // if (grounded) jumpPressed = true;

                        jumpPressed = true;

                        // Debug.Log("OnJump CALLED... we ae in switch "  + characterController.isGrounded);
                        break;

                    case InputActionPhase.Started:
                        break;

                    case InputActionPhase.Canceled:
                        break;
                }
            }
        }

        public void OnGUI()
        {
            //Debug.Log("GUI Called...");  //We KNOW its called Recursively
            if (m_Charging)
                GUI.Label(new Rect(100, 100, 200, 100), "Charging...");
            if (Event.current.Equals(Event.KeyboardEvent("Escape")))
            {
                //    Debug.Log("ESC was pressed");  //right
                Cursor.lockState = CursorLockMode.None;
                //SceneManager.LoadScene("SceneGameSet");
                return;
            }
        }
        public static void TeleportPlayer(Vector3 destination)
        {
            //playerPos = new Vector3(playerStartPos.x, playerStartPos.y);
            // t = playerPosT;
            playerPosT.position = destination;
            Physics.SyncTransforms();
            // Debug.Log("Move player to:  " + playerStartPos   + "   " + transform.position);
        }
        public static void MovePlayerHome()
        {
            //playerPos = new Vector3(playerStartPos.x, playerStartPos.y);
            // t = playerPosT;
            playerPosT.position = playerStartPos;
            Physics.SyncTransforms();
            // Debug.Log("Move player to:  " + playerStartPos   + "   " + transform.position);
        }

    }
} //end Namespace

