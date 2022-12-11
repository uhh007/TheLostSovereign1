using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Checkpoint.SaveGame();
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
        }
    }
}
