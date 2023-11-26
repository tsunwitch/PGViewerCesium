using CesiumForUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.75f;
    public float cameraZoom = 80;
    private Vector3 _cameraOffset;
    public bool lookAtPlayer = true;
    public bool rotateAroundPlayer = true;
    public float rotationSpeed = 5.0f;
    public float zoomMultiplier = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _cameraOffset = (transform.position - target.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target)
        {
            if (rotateAroundPlayer && Input.GetMouseButton(1))
            {
                Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.down);
                Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotationSpeed, Vector3.right);

                _cameraOffset = camTurnAngleY * camTurnAngleX * _cameraOffset;
            }

            Vector3 newPos = target.position + _cameraOffset * zoomMultiplier;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);

            if (lookAtPlayer)
            {
                transform.LookAt(target.transform.position);
            }
        }
    }
}
