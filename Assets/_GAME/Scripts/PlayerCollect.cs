using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerCollect : MonoBehaviour
{
    public int armourAmount { get; private set; }
    public int ammoAmount { get; private set; }

    public UnityEvent<PlayerCollect> OnArmourCollected;
    public UnityEvent<PlayerCollect> onAmmoCollected;
    
    
    public void CollectArmour()
    {
        armourAmount+=10;
        OnArmourCollected.Invoke(this);
        Debug.Log("Total Armour: "+armourAmount);
    }

    public void CollectAmmo()
    {
        ammoAmount += 10;
        onAmmoCollected.Invoke(this);
        Debug.Log("Total Ammo: " + ammoAmount);
    }

    public void HitTrap()
    {

    }
}
