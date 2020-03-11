using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 20f;
    public float zoomSpeed = 10f;

    public float dragSpeed = 3f;
    public float zoomSpeedScroll = 50f;
    private Vector3 dragOrigin;

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


        Camera.main.orthographicSize *= 1f - Input.GetAxis("Mouse ScrollWheel") * zoomSpeedScroll * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);
    }
}
