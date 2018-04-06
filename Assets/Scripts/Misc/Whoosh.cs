using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whoosh : MonoBehaviour
{
    public Transform moved;
    public float speed = 1.25f;
    public bool stage1 = false;
    private bool stage2 = false;
    // Use this for initialization
    public void Activate()
    {
        stage1 = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && stage1 == true)
        {
            stage2 = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (stage2)
        {
            moved.position = moved.position + new Vector3(0, speed * Time.deltaTime, 0);
        }
    }


}
