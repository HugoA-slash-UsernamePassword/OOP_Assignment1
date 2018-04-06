using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkQuest.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MouseLook))]
    public class CharacterHandler : MonoBehaviour
    {
        #region Variables
        #region Stats
        [Header("Stats")]
        public bool alive; //Check whether the player should respawn
        public int maxHealth, maxStamina; //The value to reset the health to when they respawn, the max value of health & stamina
        private float curHealth, curStamina;//Current health & stamina
        private bool run; //Require the player to re-press the shift key to run again when stamina runs out

        public int level = 0; //Current level
        private int curExp, maxExp; //Current exp and exp required for next level
        public int[] ExpCurve; //The exp required to level up each level
        [Space(3)]

        [Header("Weapons & Mouse interaction")]
        public int weaponDamage;//The value to decrease the enemies health by
        public int fireDelay;//The delay between shots in frames
        private int fireCount = 0; //Counter for fireDelay
        public float rayDistance = 2; //The minimum distance from an object required to interact with it.
        [Space(3)]
        #endregion
        #region Movement
        [Header("Movement")]
        private Vector3 moveDir; //Direction of movement

        public float jumpSpeed = 8.0f;
        private float speed; //Set to the values below depending on whether the player is crouching or sprinting
        public float walkSpeed = 6, crouchSpeed = 2, sprintSpeed = 14;
        public float gravity = 20.0f;
        [Space(3)]
        #endregion
        #region Reference
        [Header("Component Reference")]
        private CharacterController charC; //Reference to character controller
        public Camera mainCam;//the main camera we are using for Mouselook
        public GameObject curCheckPoint;//GameObject for our currentCheck
        [Space(3)]
        [Header("GUIStyles")]
        public GUIStyle healthStyle, staminaStyle, expStyle; //Styles for GUI elements
        #endregion
        #endregion
        #region Unity Functions
        void Start()
        {
            charC = this.GetComponent<CharacterController>();

            curHealth = maxHealth;
            curStamina = maxStamina;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        void Update()
        {
            Movement();
            CheckIfDead();
            CheckIfLevelup();
            Interact();
        }
        void LateUpdate()
        {
            StatCaps();
        }
        void OnGUI()
        {
            //set up the aspect ratio for GUI elements
            float scrW = Screen.width / 16;
            float scrH = Screen.height / 10;

            //HEALTH
            //GUI Box for the healthbar background
            GUI.Box(new Rect(2 * scrW, 0.4f * scrH, 2 * scrW, 0.3f * scrH), "");
            //GUI Box for current health
            GUI.Box(new Rect(2 * scrW, 0.4f * scrH, curHealth * (2 * scrW) / maxHealth, 0.3f * scrH), "");
            //STAMNINA
            //GUI Box for the stamina background
            GUI.Box(new Rect(2 * scrW, 1.2f * scrH, 2 * scrW, 0.3f * scrH), "");
            //GUI Box for current stamina
            if (curStamina > maxStamina / 20) //Fixes visual bug when width/height = 0
            {
                GUI.Box(new Rect(2 * scrW, 1.2f * scrH, curStamina * (2 * scrW) / maxStamina, 0.3f * scrH), "");
            }

            //EXP
            //GUI Box for the exp background
            GUI.Box(new Rect((8f - 1.625f) * scrW, 8.75f * scrH, 3.25f * scrW, 0.5f * scrH), "");
            //GUI Box for current exp
            if (curExp > maxExp / 20)
            {
                GUI.Box(new Rect((8f - 1.625f) * scrW, 8.75f * scrH, curExp * (3.25f * scrW) / maxExp, 0.5f * scrH), "");
            }
        }
        void OnTriggerEnter(Collider other)
        {
            //Set curCheckpoint to the new checkpoint
            if (other.CompareTag("Checkpoint"))
            {
                curCheckPoint = other.gameObject;
            }
        }
        void OnTriggerStay(Collider other)
        {
            if (other.tag == "ASlowAndPainfulDemise")
            {
                curHealth -= 1;
            }
        }
        #endregion
        #region Functions
        void Movement()
        {
            if (charC.isGrounded) //If the character controller is touching the ground
            {
                moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //Sets moveDir to the Horizontal and Vertical inputs, defined in ProjectSettings
                moveDir = transform.TransformDirection(moveDir); //Transforms direction from local space to world space.
                moveDir *= speed; 
                if (Input.GetKey(KeyCode.C)) //If C is being pressed
                {
                    speed = crouchSpeed; //The player is crouching and walks slower
                    if (curStamina < maxStamina) //The player is not running and therefore regains stamina
                    {
                        curStamina++;
                    }
                }
                else if (Input.GetKey(KeyCode.LeftShift) && run == true) //Else If shift is being pressed
                {
                    speed = sprintSpeed; //The player is sprinting and their speed is increased
                    curStamina--;

                }
                else //Else they are walking
                {
                    speed = walkSpeed;
                    if (curStamina < maxStamina) //The player is not running and therefore regains stamina
                    {
                        curStamina++;
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    run = true;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    moveDir.y = jumpSpeed;
                }
                if (Input.GetKey(KeyCode.Mouse0) && fireCount <= 0)
                {
                    //Shoot();
                    fireCount = fireDelay;
                }
            }
            moveDir.y -= gravity * Time.deltaTime;
            charC.Move(moveDir * Time.deltaTime);
            fireCount--;
        }
        void StatCaps()
        {
            //This has to be in LateUpdate so that the health & stamina bars do not go over/under the background for a frame 
            //Health cap
            if (curHealth > maxHealth)
            {
                curHealth = maxHealth;
            }
            //Kill the player if they are unhealthy
            if (alive == false || curHealth < 0)
            {
                alive = false;
                curHealth = 0;
            }
            //Stop the player from running endlessly and make them re-press SHIFT
            if (curStamina < 0)
            {
                curStamina = 0;
                run = false;
            }
        }
        void CheckIfLevelup()
        {
            //Code for levelup & new exp goal
            if (curExp >= maxExp)
            {
                curExp -= maxExp;
                level++;
                maxExp = ExpCurve[level];
            }
        }
        void CheckIfDead()
        {
            //if our character is deceased, reset health values and move the transform to that of the checkpoint
            if (!alive)
            {
                transform.position = curCheckPoint.transform.position;
                curHealth = maxHealth;
                alive = true;
            }
        }
        void Interact()
        {
            //if the interact key is pressed (currently 'E')
            if (Input.GetKey(KeyCode.E))
            {
                Ray interact = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); //Shoot a ray from the centre of the screen
                RaycastHit hitinfo; //Stores the collision info
                if (Physics.Raycast(interact, out hitinfo, rayDistance)) //if the ray hit something within 'rayDistance' units
                {
                    #region NPC
                    if (hitinfo.collider.CompareTag("NPC")) //if we hit an NPC
                    {
                        Debug.Log("Hit the NPC");

                        NPC.Dialogue dlg = hitinfo.transform.GetComponent<NPC.Dialogue>(); //Get the NPC's dialogue script
                        if (dlg != null) //If the NPC has a dialogue script attached
                        {
                            //disable movement and begin dialogue
                            GetComponent<MouseLook>().enabled = false;
                            hitinfo.transform.GetComponent<NPC.Dialogue>().Begin();
                            GetComponent<CharacterHandler>().enabled = false;
                            mainCam.GetComponent<MouseLook>().enabled = false;

                            //Enable the cursor
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                        }
                    }
                    #endregion
                    #region Item
                    if (hitinfo.collider.CompareTag("Item")) //if we interacted with an Item
                    {
                        Debug.Log("Hit an Item");
                    }
                    #endregion
                }
            }
        }
        #endregion
    }
}