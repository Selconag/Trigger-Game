using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerCollect : MonoBehaviour
{
   public int armourAmount { get; private set; }

    public UnityEvent<PlayerCollect> OnArmourCollected;


    public void CollectArmour()
    {
        armourAmount+=10;
        OnArmourCollected.Invoke(this);
        Debug.Log("Total Armour: "+armourAmount);
    }

    public void HitTrap()
    {

    }
}
