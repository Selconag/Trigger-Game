using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/Entities", order = 1)]
public class Entities : ScriptableObject
{
    [Range(0,50)]
    public float VeritcalEntitySpeed = 5f,HorizontalEntitySpeed = 5f;
    [Range(0, 50)]
    //Trigger zone radius depends on visionRange
    public float VisionRange = 10f;


}
