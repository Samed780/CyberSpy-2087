using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform head;

    private float startFOV, targetFOV;
    public float FOVSpeed = 1f;
    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        cam = GetComponent<Camera>();
        startFOV = cam.fieldOfView;
        targetFOV = startFOV;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = head.position;
        transform.rotation = head.rotation;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, FOVSpeed * Time.deltaTime);
    }

    public void ZoomIn(float targetZoom)
    {
        targetFOV = targetZoom;
    }

    public void ZoomOut()
    {
        targetFOV = startFOV;
    }
}
