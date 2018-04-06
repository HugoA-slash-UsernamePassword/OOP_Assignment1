using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; //Required so that events can be set in the editor

namespace MonkQuest.NPC
{
    public class Dialogue : MonoBehaviour
    {
        #region Variables
        public bool showDlg;        //boolean to toggle the characters dialogue box
        public int index;        //index for the current line of dialogue
        public int[] optionsIndex; //Index for options menu
        public int[] optionsStop; //The value to change the index to if the player chooses "No". If they choose "Yes", the index increments as normal
        public int[] lastIndex; //Index for exit menu, after which the dialogue closes
        public int[] goodIndex; //Index for Invoking the Good UnityEvent after the dialogue closes
        public int[] badIndex; //Index for Invoking the Bad UnityEvent after the dialogue closes
        [Header("NPC Name and Dialogue")]
        public string npcName; //name of this NPC
        public string[] text; //array for the dialogue

        public UnityEvent Good; //If the player accepts the option dialogue
        public UnityEvent Bad; //If the player regretfully declines
        public UnityEvent Magical; //At this moment, I'm not sure how to set events in code so i just created a third ending to the dialogue. Once i can set that up, I will be able to create more emmersive dialogue with far more endings.
        public bool isMagical = false; //Allows this ending to be set at an time

        [Header("References")]
        public GameObject player;         //Object reference to the Player
        public Player.MouseLook mainCam;        //Mouselook script reference to the MainCamera
        [Header("GUIStyles")]
        public GUIStyle dlgBg, next, optBut, exit;
        #endregion
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player.MouseLook>();
        }
        public void Begin()
        {
            showDlg = true;
        }
        public void Key()
        {
            isMagical = true;
        }
        public void TextChange(int[] newoptions, int[] newoptionstop, int[] newlast, int[] newgood, int[] newbad, string newname, string[] newtext)
        {
            optionsIndex = newoptions;
            optionsStop = newoptionstop;
            lastIndex = newlast;
            goodIndex = newgood;
            badIndex = newbad;
            npcName = newname;
            text = newtext;
        }
        public void SetMagic()
        {
            isMagical = true;
        }
        void OnGUI()
        {
            //if our dialogue can be seen on screen
            if (showDlg)
            {
                //set up ratio for 16:10
                float scrW = Screen.width / 16;
                float scrH = Screen.height / 10;

                //Loop through each item in optionsIndex and check if it is equal to the current index
                int tempOption = -1;
                for (int i = 0; i < optionsIndex.Length; i++) //I can't use a foreach loop in this case because i need the array index
                {
                    if (index == optionsIndex[i])
                    {
                        tempOption = i;
                    }
                }
                //Loop through each item in lastIndex and check if it is equal to the current index
                bool end = false;
                foreach (int item in lastIndex)
                {
                    if (index == item)
                    {
                        end = true;
                    }
                }
                //the dialogue box takes up the whole bottom 3rd of the screen and displays the NPC's name and current dialogue line
                GUI.Box(new Rect(0, 6 * scrH, 16 * scrW, 3 * scrH), npcName + ": " + text[index], dlgBg);
                if (!(end || tempOption != -1)) //Normal dialogue
                {
                    //Clicking this button advances the dialogue
                    if (GUI.Button(new Rect(14.5f * scrW, 8f * scrH, scrW, 0.5f * scrH), "", next))
                    {
                        index++;
                    }
                }
                else if (tempOption != -1) //If we are at an options menu as specified in optionsIndex
                {
                    //Accept button advances the dialogue
                    if (GUI.Button(new Rect(14.5f * scrW, 8f * scrH, scrW, 0.5f * scrH), "Y", optBut))
                    {
                        index++;
                    }
                    //Decline button skips to the corresponding optionsStop
                    if (GUI.Button(new Rect(12.5f * scrW, 8f * scrH, scrW, 0.5f * scrH), "N", optBut))
                    {
                        index = optionsStop[tempOption];
                    }
                }
                else //If we are at an exit menu as specified in lastIndex
                {
                    if (GUI.Button(new Rect(14.5f * scrW, 8f * scrH, scrW, 0.5f * scrH), "", exit))
                    {
                        if (isMagical)
                        {
                            Magical.Invoke();
                        }
                        else
                        {
                            //Invoking the 'Good' and 'Bad' UnityEvents similarly to when lastIndex was checked
                            for (int i = 0; i < goodIndex.Length; i++)
                            {
                                if (index == goodIndex[i])
                                {
                                    Good.Invoke();
                                }
                            }
                            for (int i = 0; i < badIndex.Length; i++)
                            {
                                if (index == badIndex[i])
                                {
                                    Bad.Invoke();
                                }
                            }
                        }
                        //Enable movement and close dialogue
                        showDlg = false;
                        index = 0;
                        mainCam.enabled = true;
                        player.GetComponent<Player.MouseLook>().enabled = true;
                        player.GetComponent<Player.CharacterHandler>().enabled = true;
                        //lock and hide the mouse cursor
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
        }
    }
}

/*
 * Todo:
 * Failsafe at end of text[]
 * 
 */