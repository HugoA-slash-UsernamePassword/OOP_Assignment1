using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkQuest.Player
{
    public class MouseLook : MonoBehaviour
    {
        [Header("Sensitivity")]
        public float sensitivityX = 6; //Sensitivity x value
        public float sensitivityY = 6; //Sensitivity y value
        [Header("Max and min Y rotation")] // Makes sure the player cannot view upside down as that causes screen glitches; reduces neck elasticity
        public float minY = -60;
        public float maxY = 60;
        float rotationY = 0;
        [Header("Rotation Type")]
        [Tooltip("MouseX for player and MouseY for camera is recommended for normal camera movement")]
        public RotationalAxis axis = RotationalAxis.MouseX;
        public enum RotationalAxis //Lets the player select how mouse movement effects certain objects
        {
            MouseX,
            MouseY
        }

        void Update()
        {
            Vector2 SquareJoystick = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); //Get the mouse input. 
            if (axis == RotationalAxis.MouseX) //rotate around the y axis depending on the "Mouse X" input
            {
                transform.Rotate(0, SquareJoystick.x * sensitivityX, 0);
            }
            else //MouseY has to be done using Euler Angles because the rotation is being restricted and Euler Angles use rotation instead of Vector3.
            {
                rotationY += SquareJoystick.y * sensitivityY; //Add the "Mouse Y" input to rotationY
                rotationY = Mathf.Clamp(rotationY, minY, maxY); //restrict the rotation between the preset values
                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0); //Apply
            }
        }
    }
}
