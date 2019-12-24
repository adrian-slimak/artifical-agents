using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 20f;
    public float zoomSpeed = 10f;

    void Update()
    {
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.RightArrow))
            newPosition += new Vector3(speed * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow))
            newPosition += new Vector3(-speed * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.DownArrow))
            newPosition += new Vector3(0, -speed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.UpArrow))
            newPosition += new Vector3(0, speed * Time.deltaTime, 0);
        transform.position = newPosition;

        if (Input.GetKey(KeyCode.KeypadPlus))
            Camera.main.orthographicSize -= zoomSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.KeypadMinus))
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
    }
}
