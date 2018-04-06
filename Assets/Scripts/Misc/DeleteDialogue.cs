using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteDialogue : MonoBehaviour
{
    private MonkQuest.NPC.Dialogue reference;
    // Use this for initialization
    void Start()
    {
        reference = GetComponent<MonkQuest.NPC.Dialogue>();
    }

    public void ADoor()
    {
        reference.enabled = true;
    }
// Update is called once per frame
    public void NotADoor()
    {
        reference.enabled = false;
    }
}
