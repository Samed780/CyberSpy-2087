using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = FindObjectOfType<CameraMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + m_Camera.forward);
    }
}
