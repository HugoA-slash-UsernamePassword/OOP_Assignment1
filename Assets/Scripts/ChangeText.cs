using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkQuest.NPC
{
    public class ChangeText : MonoBehaviour
    {
        //The NPC who's text we are changing
        public Dialogue parent;
        //The new dialogue settings
        public int[] optionsIndex;
        public int[] optionsStop;
        public int[] lastIndex;
        public int[] goodIndex;
        public int[] badIndex;
        public string npcName;
        public string[] text;
        public void Change() //Send the dialogue settings to the parent
        {
            parent.TextChange(optionsIndex, optionsStop, lastIndex, goodIndex, badIndex, npcName, text);
        }
    }
}
