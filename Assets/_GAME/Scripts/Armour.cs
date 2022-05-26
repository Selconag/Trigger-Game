using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerCollect playerCollect = other.GetComponent<PlayerCollect>();

        if(playerCollect != null)
        {
            playerCollect.CollectArmour();
            gameObject.SetActive(false);    
        }
    }
}
