using UnityEngine;
using System.Collections;

public class InteractFakeBox : MonoBehaviour
{
    // “other” refers to the collider on the GameObject inside this trigger
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("A collider has entered the DoorObject trigger");
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("A collider is inside the DoorObject trigger");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("A collider has exited the DoorObject trigger");
    }
}
