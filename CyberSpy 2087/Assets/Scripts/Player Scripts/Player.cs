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

    //hookshot
    public Transform hitPoint;
    private Vector3 hookShotPos;
    public float hookSpeed = 5f;
    private Vector3 flyingMomentum;
    public Transform grapplingHook;
    private float hookShotSize;
    public ParticleSystem warpEffect;

    //player states
    private enum State { Normal, Flying , HookShotThrow}
    private State state;



    // Start is called before the first frame update
    void Start()
    {
        state = State.Normal;

        bodyScale = body.localScale;

        initialControllerHeight = controller.height;
        
        grapplingHook.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                PlayerMovement();
                CameraControl();
                Jump();
                Crouch();
                SlideTimer();
                HanddleHookShotStart();
                break;

            case State.Flying:
                HandleHookShotMovement();
                CameraControl();
                break;
            case State.HookShotThrow:
                PlayerMovement();
                CameraControl();
                ThrowHook();
                break;

            default:
                break;
        }

        
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

        if (Input.GetButtonDown("Jump") && canJump)
        {
            velocity.y = Mathf.Sqrt(-2f * jumpHeight * Physics.gravity.y) * Time.deltaTime;
            AudioManager.instance.PlaySFX(2);
        }

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

        movement += flyingMomentum * Time.deltaTime;

        controller.Move(movement);

        velocity.y += Physics.gravity.y * Mathf.Pow(Time.deltaTime, 2) * gravityModifier;

        if (controller.isGrounded)
            velocity.y = Physics.gravity.y * Time.deltaTime;

        controller.Move(velocity);

        if(flyingMomentum.magnitude > 0f)
        {
            float reductionAmout = 4f;
            flyingMomentum -= flyingMomentum * reductionAmout * Time.deltaTime;

            if (flyingMomentum.magnitude < 5f)
                flyingMomentum = Vector3.zero;
        }
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

    void HanddleHookShotStart()
    {
        if (TestHookShotInput())
        {
            RaycastHit hit;

            if(Physics.Raycast(cameraHead.position, cameraHead.forward, out hit))
            {
                hitPoint.position = hit.point;
                hookShotPos = hit.point;

                hookShotSize = 0f;
                grapplingHook.gameObject.SetActive(true);
                state = State.HookShotThrow;
            }
        }
    }

    void ThrowHook()
    {
        grapplingHook.LookAt(hookShotPos);
        float hookThrowSpeed = 50f;
        hookShotSize += hookThrowSpeed * Time.deltaTime;
        grapplingHook.localScale = new Vector3(1, 1, hookShotSize);
        if (hookShotSize >= Vector3.Distance(transform.position, hookShotPos))
        {
            state = State.Flying;
            FindObjectOfType<CameraMovement>().ZoomIn(100f);
            warpEffect.Play();
        }
    }

    void HandleHookShotMovement()
    {
        grapplingHook.LookAt(hookShotPos);

        //movement direction
        Vector3 hookShotDirection = (hookShotPos - transform.position).normalized;

        float hookShotMinSpeed = 12f, hookShotMaxSpeed = 50f;

        float hookShotSpeedModifier = Mathf.Clamp(Vector3.Distance(transform.position, hookShotPos), hookShotMinSpeed, hookShotMaxSpeed);

        controller.Move(hookShotDirection * hookSpeed * hookShotSpeedModifier * Time.deltaTime);

        if (Vector3.Distance(transform.position, hookShotPos) < 2f)
            CancelHook();

        if (TestHookShotInput())
            CancelHook();

        if (TestJumpInput())
        {
            float extraMomentum = 40f, jumpSpeedUp = 40f;

            flyingMomentum += hookShotDirection * hookSpeed * extraMomentum;

            flyingMomentum += Vector3.up * jumpSpeedUp;

            CancelHook();
        }
    }

    bool TestJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    bool TestHookShotInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    void ResetGravity()
    {
        velocity.y = 0f;
    }

    void CancelHook()
    {
        state = State.Normal;
        ResetGravity();
        grapplingHook.gameObject.SetActive(false);
        FindObjectOfType<CameraMovement>().ZoomOut();
        warpEffect.Stop();
    }
}
