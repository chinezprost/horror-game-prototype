using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{

    [SerializeField] private bool isEnabled = true;
    
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.013f;
    [SerializeField, Range(0, 30)] private float frequency = 13.0f;

    

    private float toggleSpeed = 0.01f;
    private float currentSpeed = 0;
    private Vector3 startPos;
    private Rigidbody playerRigidbody;

    public Transform camera;
    public Transform cameraHolder;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        startPos = camera.localPosition;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;
        
        
        CheckMotion();
        ResetPosition();
        camera.LookAt(FocusTarget());
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
        Vector3 newPosition = Vector3.zero;
        newPosition.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        newPosition.y += Mathf.Sin(Time.time * frequency) * amplitude;
        return newPosition;
    }

    private Vector3 FocusTarget()
    {
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        newPosition += cameraHolder.forward * 15.0f;
        return newPosition;

    }
}
