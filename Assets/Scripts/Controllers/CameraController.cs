using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameObject player;
    private float xMove = -90f;
    private float yMove = 0;
    private float distance = 10;
    private float wheelSpeed = 10.0f;

    private void Start()
    {
        if (GameObject.FindWithTag("Player") != null)
            player = GameObject.FindWithTag("Player");
        else
            Debug.Log("Player 오브젝트 없음");
    }

    private void LateUpdate()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        if (Input.GetMouseButton(1))
        {
            xMove += Input.GetAxis("Mouse X");
            yMove -= Input.GetAxis("Mouse Y");
        }

        transform.rotation = Quaternion.Euler(yMove, xMove, 0);

        distance -= Input.GetAxis("Mouse ScrollWheel") * wheelSpeed;
        distance = Mathf.Clamp(distance, 1.0f, 10.0f);

        Vector3 reverseDistance = new Vector3(0.0f, -2.0f, distance);

        transform.position = player.transform.position - transform.rotation * reverseDistance;
    }
}
