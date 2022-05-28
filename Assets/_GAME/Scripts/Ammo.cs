using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerCollect playerCollect = other.GetComponent<PlayerCollect>();

        if(playerCollect != null)
        {
            playerCollect.CollectAmmo();
            gameObject.SetActive(false);
        }
    }
}
