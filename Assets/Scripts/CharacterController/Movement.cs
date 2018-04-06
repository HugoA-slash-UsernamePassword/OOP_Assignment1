using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Movement.CharController
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MonoBehaviour
    {
        //Set variables
        public Vector3 moveDir = Vector3.zero;
        public CharacterController charC;
        public float jumpSpeed = 8; //The value of the Y vector of 'speed' when the Jump button is pressed
        public float normalSpeed = 6;
        public float fastSpeed = 12;
        private float speed;
        public float gravity = 20;
        public Animator anim;
        float timer = 0;
        void Start() //runs when the object is first activated
        {
            charC = this.GetComponent<CharacterController>();
            speed = normalSpeed;
        }

        void Update() //runs each frame
        {
            if(charC.isGrounded) //IF the character controller is touching the ground
            {
                moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //Sets moveDir to the Horizontal and Vertical inputs. ie WASD/arrow keys (default).
                moveDir = transform.TransformDirection(moveDir); //Transforms direction from local space to world space.
                moveDir *= speed;
                if(Input.GetKeyDown(KeyCode.C))
                {
                    anim.SetBool("Crawl", true);
                }
                if (Input.GetKeyUp(KeyCode.C))
                {
                    anim.SetBool("Crawl", false);
                }
                if(Input.GetButton("Jump"))
                {
                    //moveDir.y = jumpSpeed;
                    anim.SetBool("Jump", true);
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speed = fastSpeed;
                    anim.SetBool("Run", true);
                }
                else
                {
                    speed = normalSpeed;
                    anim.SetBool("Run", false);
                }
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    anim.SetBool("Swipe", true);
                }
                else
                {
                    anim.SetBool("Swipe", false);
                }
                anim.SetFloat("Movement", Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")));
            }
            if(anim.GetBool("Jump") == true)
            {
                timer += Time.deltaTime;
                if (timer >= 0.15f)
                {
                    anim.SetBool("Jump", false);
                    timer = 0;
                }
            }
            //if(anim. = false)
            //{
            //    moveDir.y -= gravity * Time.deltaTime;
            //    charC.Move(moveDir * Time.deltaTime);
            //}
        }
    }
}