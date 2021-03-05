using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if this object touch the player stop the time the game is finished
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f;
        }
    }
}
