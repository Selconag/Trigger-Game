using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Range(0f, 30f)]
    [SerializeField] private float playerForwardSpeed;
    [Range(0f, 120f)]
    [SerializeField] private float rotationPerClick = 30f;
    [Range(0f, 1000f)]
    [SerializeField] private float upwardForceModifier = 10;
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private int score = 0;
    [SerializeField] private Text m_ScoreText;
    [SerializeField] private Rigidbody m_Rigid;
    private bool sharpening = false;
    private static Player m_Instance;
    public static Action playerScore;
    //private Joystick m_Joystick;

    private void Awake() => m_Instance = this;
    public static Player Instance => m_Instance;

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !gameStarted) gameStarted = true;
        if (!gameStarted) return;

        if (sharpening) return;
        Vector3 speed = m_Rigid.velocity;
        speed.x = playerForwardSpeed;
        m_Rigid.velocity = speed;
        //if (Input.GetMouseButtonDown(0)) RotateSharpener();
        //float forward = playerForwardSpeed * Time.deltaTime;
        //transform.Translate(1, 0, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //RotateSharpener();
        m_Rigid.AddTorque(Vector3.forward * rotationPerClick * -1);
        m_Rigid.AddForce(Vector3.up * upwardForceModifier, ForceMode.VelocityChange);

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        sharpening = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        SnapToObject();
    }

    private void OnTriggerExit(Collider other)
    {

    }

    private void RotateSharpener()
    {
        //Rotate the sharpener by adding torque and force upwards
        m_Rigid.AddTorque(Vector3.forward * rotationPerClick * -1);
    }

    private void SnapToObject()
    {
        sharpening = true;
        //Do the sharpening

    }

    private void RemoveChunkObjects()
    {

    }

}
