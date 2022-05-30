using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerCollect : MonoBehaviour
{
    public int armourAmount { get; private set; }
    public int ammoAmount { get; private set; }

    public UnityEvent<PlayerCollect> OnArmourCollected;
    public UnityEvent<PlayerCollect> OnAmmoCollected;
    
    
    public void CollectArmour()
    {
        armourAmount+=1;
        OnArmourCollected.Invoke(this);
        Debug.Log("Total Armour: "+armourAmount);
    }

    public void CollectAmmo()
    {
        ammoAmount += 5;
        OnAmmoCollected.Invoke(this);
        Debug.Log("Total Ammo: " + ammoAmount);
    }

    public void HitTrap()
    {

    }
}
