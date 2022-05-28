using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected Entities enemy;
    //Walks speed increased based on 0 seconds to 1 seconds(max speed)
    SphereCollider spher;
    private void Awake()
    {
        spher = GetComponent<SphereCollider>();
    }
    private void Start()
    {
        spher.radius = enemy.VisionRange;
    }

    private void Update()
    {
        //Just walks
        //enemy.verticalspeed;
    }
}
