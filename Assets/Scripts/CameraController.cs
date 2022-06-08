using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform rotationAt;
    public Transform lookAt;
    public Transform mainCameraPoint;

    public float rotationCameraSpeed;

    void Start()
    {
        Camera.main.transform.parent = mainCameraPoint;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void Update()
    {
        mainCameraPoint.transform.LookAt(lookAt);

        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * rotationCameraSpeed);
    }
}