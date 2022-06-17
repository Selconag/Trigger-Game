using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrpExampleSceneCameraRotation : MonoBehaviour
{
    public Transform cam = null;
    public float camRotateSpeed = 1;

    public Text text = null;

    int last_time;
    int frame_count;
    int mem_frame;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    Vector2? oldMousePos = null;
    void Update()
    {
        var c_t = (int)Time.time;
        if (c_t != last_time)
        {
            last_time = c_t;
            mem_frame = frame_count;
            frame_count = 0;
        }
        frame_count++;

        text.text = mem_frame.ToString();

        var r = cam.eulerAngles;
        r.y += Input.GetAxis("Horizontal") * camRotateSpeed;
        r.x += -Input.GetAxis("Vertical") * camRotateSpeed;
        if (!oldMousePos.HasValue) oldMousePos = Input.mousePosition;
        var dm = (Vector2)Input.mousePosition - oldMousePos.Value;
        r.y += dm.x * camRotateSpeed;
        r.x += -dm.y * camRotateSpeed;
        r.z = 0;
        oldMousePos = Input.mousePosition;

        cam.eulerAngles = r;


    }
}
