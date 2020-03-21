/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using System.Collections.Generic;

namespace DOFprojFPS
{
    public class FPSController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 1f;
        public float crouchSpeed = 0.4f;
        public float runSpeedMultiplier = 2f;
        public float jumpForce = 4f;
        
        public float crouchHeight = 0.5f;

        [HideInInspector]
        public bool crouch = false;
        public bool lockCursor;
        
        [HideInInspector]
        public Rigidbody controllerRigidbody;
        
        private CapsuleCollider controllerCollider;
        public Transform camHolder;
        private float moveSpeedLocal;
        
        private float distanceToGround;

        private Animator weaponHolderAnimator;

        public bool isClimbing = false;

        private float inAirTime;

        [HideInInspector]
        public bool mouseLookEnabled = true;

        //Velocity calculation variable
        private Vector3 previousPos = new Vector3();
        
        Vector3 dirVector;

        InputManager inputManager;

        private float defaultColliderHeight;

        public static bool canMove = true;

        [Header("MouseLook Settings")]

        private float clampY = 160;
        public Vector2 sensitivity = new Vector2(0.5f, 0.5f);
        public Vector2 smoothing = new Vector2(3, 3);

        [HideInInspector]
        public Vector2 targetDirection;
        [HideInInspector]
        public Vector2 _mouseAbsolute;
        [HideInInspector]
        public Vector2 _smoothMouse;

        private void OnEnable()
        {
            controllerRigidbody = GetComponent<Rigidbody>();
            controllerCollider = GetComponent<CapsuleCollider>();

            defaultColliderHeight = GetComponent<CapsuleCollider>().height;

            distanceToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
            weaponHolderAnimator = GameObject.Find("Weapon holder").GetComponent<Animator>();

            inputManager = FindObjectOfType<InputManager>();
        }

        [HideInInspector]
        public Vector2 recoil;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                recoil += new Vector2(0, .2f);
            }

            if (!canMove)
                return;
            
            if (!InputManager.useMobileInput)
            {
                StandaloneMovement();
            }
            else
            {
                MobileMovement();
            }

            if (lockCursor && !InputManager.useMobileInput)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            Crouch();
            Landing();
        }

        void MouseLook()
        {
            Quaternion targetOrientation = Quaternion.Euler(targetDirection);

            Vector2 mouseDelta = new Vector2();

            if (InputManager.useMobileInput)
            {
                mouseDelta = new Vector2(InputManager.touchPanelLook.x * Time.deltaTime, InputManager.touchPanelLook.y * Time.deltaTime);
            }
            else if (!InputManager.useMobileInput)
            {
                mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }

            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
            
            _mouseAbsolute += _smoothMouse;

            if (recoil != Vector2.zero)
            {
                _mouseAbsolute.x += recoil.x;
                _mouseAbsolute.y += recoil.y;
                recoil = Vector2.zero;
            }

            if (_mouseAbsolute.y < -clampY * 0.5f)
                _mouseAbsolute.y = -clampY * 0.5f;

            if (_mouseAbsolute.y > clampY * 0.5f)
                _mouseAbsolute.y = clampY * 0.5f;
            
            var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
            camHolder.transform.localRotation = xRotation;

            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, camHolder.transform.InverseTransformDirection(Vector3.up));
            camHolder.transform.localRotation *= yRotation;
            camHolder.transform.rotation *= targetOrientation;
        }


        void StandaloneMovement()
        {
            if(weaponHolderAnimator == null)
            {
                weaponHolderAnimator = GameObject.Find("Weapon holder").GetComponent<Animator>();
            }

            if (isGrounded())
            {
                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Walk", true);
                    moveSpeedLocal = moveSpeed;
                }
                else
                    weaponHolderAnimator.SetBool("Walk", false);

                if (Input.GetKey(inputManager.Run) && !isClimbing && !crouch && weaponHolderAnimator.GetBool("Walk") == true)
                {
                    moveSpeedLocal = runSpeedMultiplier * moveSpeed;
                    weaponHolderAnimator.SetBool("Run", true);
                }
                else
                    weaponHolderAnimator.SetBool("Run", false);
            }
            else
            {
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
            }

            if (crouch)
            {
                moveSpeedLocal = crouchSpeed;
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Crouch", true);
                }
                else
                    weaponHolderAnimator.SetBool("Crouch", false);
            }
            else
                weaponHolderAnimator.SetBool("Crouch", false);

            if (Input.GetKeyDown(inputManager.Crouch))
            {
                crouch = !crouch;
            }
            if (Input.GetKeyDown(inputManager.Jump))
            {
                Jump();
                crouch = false;
            }
        }

        public void MobileMovement()
        {
            if (isGrounded())
            {
                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Walk", true);
                    moveSpeedLocal = moveSpeed;
                }
                else
                    weaponHolderAnimator.SetBool("Walk", false);

                if (InputManager.joystickInputVector.y > 0.5f && !isClimbing && !crouch && weaponHolderAnimator.GetBool("Walk") == true)
                {
                    moveSpeedLocal = runSpeedMultiplier * moveSpeed;
                    weaponHolderAnimator.SetBool("Run", true);
                }
                else
                    weaponHolderAnimator.SetBool("Run", false);
            }
            else
            {
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
            }

            if (crouch)
            {
                moveSpeedLocal = crouchSpeed;
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Crouch", true);
                }
                else
                    weaponHolderAnimator.SetBool("Crouch", false);
            }
            else
                weaponHolderAnimator.SetBool("Crouch", false);

            if (Input.GetKeyDown(inputManager.Crouch))
            {
                crouch = !crouch;
            }
            if (Input.GetKeyDown(inputManager.Jump))
            {
                Jump();
                crouch = false;
            }
            
        }
        private bool _freezeMovement = false;
        public bool freezeMovement
        {
            get { return _freezeMovement; }
            set { _freezeMovement = value; }
        }

        void FixedUpdate()
        {
            if (!_freezeMovement)
            {
                CharacterMovement();
                if (mouseLookEnabled && !InventoryManager.showInventory && lockCursor)
                    MouseLook();
            }
        }
        
        void CharacterMovement()
        {
            if(weaponHolderAnimator == null)
            {
                weaponHolderAnimator = GameObject.Find("Weapon holder").GetComponent<Animator>();
            }

            var camForward = camHolder.transform.forward;
            var camRight = camHolder.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            
            if (isClimbing)
            {
                crouch = false;

                weaponHolderAnimator.SetBool("HideWeapon", true);
                controllerRigidbody.useGravity = false;

                
                if (!InputManager.useMobileInput)
                {
                    dirVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
                }
                else if(InputManager.useMobileInput)
                {
                    dirVector = new Vector3(InputManager.joystickInputVector.x, 0, InputManager.joystickInputVector.y);
                }
                
                Vector3 verticalDirection = transform.up;
                Vector3 moveDirection = (verticalDirection) * dirVector.z+ camRight * dirVector.x;
                
                controllerRigidbody.MovePosition(transform.position + moveDirection * moveSpeedLocal * Time.deltaTime);
            }
            else
            {
                weaponHolderAnimator.SetBool("HideWeapon", false);
                controllerRigidbody.useGravity = true;

                if (!InputManager.useMobileInput)
                {
                    dirVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
                }
                else if(InputManager.useMobileInput)
                {
                    dirVector = new Vector3(InputManager.joystickInputVector.x, 0, InputManager.joystickInputVector.y);
                }
                Vector3 moveDirection = camForward * dirVector.z + camRight * dirVector.x;

                controllerRigidbody.MovePosition(transform.position + moveDirection * moveSpeedLocal * Time.deltaTime);
            }
        }

        bool CheckMovement()
        {
            if (!InputManager.useMobileInput)
            {
                if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0 || Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
                {
                    return true;
                }
            }

            if(InputManager.useMobileInput)
            {
                if (InputManager.joystickInputVector.x > 0 || InputManager.joystickInputVector.x < 0 || InputManager.joystickInputVector.y > 0 || InputManager.joystickInputVector.y < 0)
                    return true;
            }

            return false;
        }
        
        void Crouch()
        {
            if (crouch == true)
            {
                controllerCollider.height = Mathf.Lerp(controllerCollider.height, crouchHeight, Time.deltaTime * 8);
                camHolder.transform.localPosition = Vector3.Lerp(camHolder.transform.localPosition, new Vector3(0, 0.2f, 0), Time.deltaTime * 4 );
                //Ray ray = new Ray();
                //ray.origin = transform.position;
                //ray.direction = transform.up;
            }
            else
            {
                controllerCollider.height = Mathf.Lerp(controllerCollider.height, defaultColliderHeight, Time.deltaTime * 8);
                camHolder.transform.localPosition = Vector3.Lerp(camHolder.transform.localPosition, new Vector3(0, 0.4f, 0), Time.deltaTime * 2);
            }
        }
        
        public float GetVelocityMagnitude()
        {
            var velocity = ((transform.position - previousPos).magnitude) / Time.deltaTime;
            previousPos = transform.position;
            return velocity;
        }

        public void Jump()
        {
            if (isGrounded())
                controllerRigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        }

        public void CrouchMobile()
        {
            crouch = !crouch;
        }

        public bool haveCollision = false;

        private void OnCollisionEnter(Collision collision)
        {
            if(collision != null)
            {
                haveCollision = true;
            }
            else
            {
                haveCollision = false;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            haveCollision = false;
        }

        public bool isGrounded()
        {
            if (!haveCollision)
            {
                return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.5f);
            }

            return true;
        }

        void Landing()
        {
            if(!isGrounded())
            {
                inAirTime += Time.deltaTime;
            }
            else
            {
                if (inAirTime > 0.5f)
                    weaponHolderAnimator.Play("Landing");

                inAirTime = 0;
            }
        }
    }
}