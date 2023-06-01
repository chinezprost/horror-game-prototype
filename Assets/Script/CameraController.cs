using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CameraController : MonoBehaviour
{

    [SerializeField] private bool isEnabled = true;
    
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.013f;
    [SerializeField, Range(0, 30)] private float frequency = 13.0f;
    [SerializeField, Range(10, 80)] private float fieldOfView = 75f;
    [SerializeField] private float fovChangeCoef = 6f;

    private PlayerNetwork localPlayerNetwork;
    

    private float toggleSpeed = 0.01f;
    private float currentSpeed = 0;
    private Vector3 startPos;
    private Rigidbody playerRigidbody;

    public Transform camera;
    public Camera cameraComponent;
    public Transform cameraHolder;


    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        cameraComponent = GetComponentInChildren<Camera>();
        localPlayerNetwork = GetComponent<PlayerNetwork>();
        startPos = camera.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;
        
        
        CheckMotion();
        FieldOfViewLogic();
        ResetPosition();
        camera.LookAt(FocusTarget());
    }

    private void FieldOfViewLogic()
    {
        if (!localPlayerNetwork.isMoving)
        {
            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, fieldOfView, Time.deltaTime * 2);
            return;
        }
        
        cameraComponent.fieldOfView = currentSpeed switch
        {
            >.1f => Mathf.Lerp(cameraComponent.fieldOfView, fieldOfView + currentSpeed * fovChangeCoef, Time.deltaTime),
            _ => Mathf.Lerp(cameraComponent.fieldOfView, fieldOfView, Time.deltaTime)
        };
    }

    private void ResetPosition()
    {
        if (camera.localPosition == startPos) return;
            camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 1 * Time.deltaTime);
    }

    private void CheckMotion()
    {
        currentSpeed = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z).magnitude;
        if (currentSpeed < toggleSpeed) return;

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        camera.localPosition += motion / 45 * currentSpeed;
    }

    private Vector3 FootStepMotion()
    {
        var newPosition = Vector3.zero;
        var cosOffset = Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        var sinOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        newPosition.x += cosOffset * Time.deltaTime * 500;
        newPosition.y += sinOffset * Time.deltaTime * 500;
        return newPosition;
    }

    private Vector3 FocusTarget()
    {
        var newPosition = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        newPosition += cameraHolder.forward * 15.0f;
        return newPosition;

    }
}
