using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 10f, runningSpede = 30f;

    //gravity
    public Vector3 velocity;
    public float gravityModifier;

    public Transform cameraHead;
    public float mouseSensitivity = 100f;
    private float cameraVerticalRotation;

    //jumping
    public float jumpHeight = 1f;
    private bool canJump;
    public Transform ground;
    public LayerMask groundLayer;
    public float groundDistance = 0.5f;

    //crouching
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 bodyScale;
    public Transform body;
    private float initialControllerHeight;
    public float crouchSpeed = 6f;
    private bool isCrouching = false;


    //animation
    public Animator animator;

    //sliding
    private bool isRunning, startSlideTimer;
    public float slideSpeed = 10f;
    private float currentSlideTimer, maxSlideTime = 2f;




    // Start is called before the first frame update
    void Start()
    {
        bodyScale = body.localScale;
        initialControllerHeight = controller.height;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        CameraControl();

        Jump();

        Crouch();

        SlideTimer();
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
            StartCrouching();
        if (Input.GetKeyUp(KeyCode.C) || currentSlideTimer > maxSlideTime)
            StopCrouching();
    }

    private void StopCrouching()
    {
        body.localScale = bodyScale;
        cameraHead.position += new Vector3(0, 1, 0);
        controller.height = initialControllerHeight;
        isCrouching = false;

        currentSlideTimer = 0;
        velocity = new Vector3(0, 0, 0);
        startSlideTimer = false;
    }

    private void StartCrouching()
    {
        body.localScale = crouchScale;
        cameraHead.position -= new Vector3(0, 1, 0);
        controller.height /= 2;
        isCrouching = true;

        if (isRunning)
        {
            velocity = Vector3.ProjectOnPlane(cameraHead.transform.forward, Vector3.up).normalized * slideSpeed * Time.deltaTime;
            startSlideTimer = true;
        }
    }

    private void Jump()
    {
        canJump = Physics.OverlapSphere(ground.position, groundDistance, groundLayer).Length > 0;

        if (Input.GetButtonDown("Jump") && canJump )
            velocity.y = Mathf.Sqrt(-2f * jumpHeight * Physics.gravity.y) * Time.deltaTime;

        controller.Move(velocity);
    }

    void PlayerMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = x * transform.right + z * transform.forward;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            movement = movement * runningSpede * Time.deltaTime;
            isRunning = true;
        }
        else if (isCrouching)
            movement = movement * crouchSpeed * Time.deltaTime;
        else
        {
            movement = movement * speed * Time.deltaTime;
            isRunning = false;
        }

        animator.SetFloat("Speed", movement.magnitude);

        controller.Move(movement);

        velocity.y += Physics.gravity.y * Mathf.Pow(Time.deltaTime, 2) * gravityModifier;

        if (controller.isGrounded)
            velocity.y = Physics.gravity.y * Time.deltaTime;

        controller.Move(velocity);
    }

    void CameraControl()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        cameraHead.localRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);
    }

    void SlideTimer()
    {
        if (startSlideTimer)
            currentSlideTimer += Time.deltaTime;
    }
}
