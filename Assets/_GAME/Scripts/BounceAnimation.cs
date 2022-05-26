using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnimation : MonoBehaviour
{
    public float bounceSpeed = 8;
    public float bounceAmplitude = 0.05f;
    public float rotationSpeed = 90;


    private float startingHeight;
    private float timeOffset;
    // Start is called before the first frame update
    void Start()
    {
        startingHeight = transform.localPosition.y;
        timeOffset = Random.value * Mathf.PI * 2;
    }

    // Update is called once per frame
    void Update()
    {
        float finalHeight = startingHeight + Mathf.Sin(Time.time * bounceSpeed + timeOffset) * bounceAmplitude;
        var position = transform.localPosition;

        position.y = finalHeight;
        transform.localPosition = position;



        Vector3 rotation = transform.localRotation.eulerAngles;
        rotation.y += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }
}
