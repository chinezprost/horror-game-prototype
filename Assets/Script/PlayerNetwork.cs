using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework.Constraints;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Diagnostics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerNetwork : NetworkBehaviour
{

    public PlayerInventory PlayerInventory;
    public HeadBobController PlayerHeadbobController;
    public Rigidbody PlayerRigidbody;
    
    public GameObject PlayerCamera;
    public GameObject CameraHolder;

    [Tooltip("PlayerSpeed")] 
    public float speedModifier = 4;
    public float runningSpeed = 2;
    public float sensitivityModifier = 1f;

    private float maxLookAngle = 87f;

    private float cameraSmoothenessLerpTimer = 0;
    private float accelerationCoeficient = 0;
    private bool isMoving = false;
    private bool isRunning = false;

    public float deaccelerationCoef = 50;
    public float accelerationCoef = 10;
    public float maximumAccelerationCoef = 10;

    private Vector3 currentPlayerVelocity = Vector3.zero;
    private Vector3 previousPlayerVelocity = Vector3.zero;
    
    void Start()
    {
        PlayerInitialization();
    }

    void Update()
    {
        PlayerMovementLogic();
        PlayerCameraLogic();
    }

    

    

    public override void OnNetworkSpawn()
    {
        
    }

    void PlayerInitialization()
    {
        //if it is owner of player
        if (IsOwner)
        {
            Debug.Log("Spawned");
            
            PlayerRigidbody = this.gameObject.AddComponent<Rigidbody>();
            PlayerHeadbobController = this.gameObject.AddComponent<HeadBobController>();
            PlayerInventory = this.gameObject.AddComponent<PlayerInventory>();
            
            PlayerHeadbobController.enabled = true;
            
            PlayerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            PlayerRigidbody.automaticInertiaTensor = false;
            PlayerRigidbody.inertiaTensor = new Vector3(0, 0, 0);
            
            PlayerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            PlayerCamera = new GameObject("Camera Holder");
            PlayerCamera.transform.localRotation = Quaternion.Euler(0, 90, 0);
            if (CameraHolder == null)
            {
                Debug.Log("Error while creating player camera holder.");
            }
            if (PlayerCamera == null)
            {
                Debug.Log("Error while creating player camera.");
            }

            PlayerCamera.transform.parent = this.transform;
            Camera.main.transform.parent = PlayerCamera.transform;
            PlayerCamera.transform.parent = Camera.main.transform;
            PlayerCamera.transform.position = new Vector3(0, 2, 0);

            PlayerHeadbobController.camera = Camera.main.transform;
            PlayerHeadbobController.cameraHolder = PlayerCamera.transform;
        }
        else
        {
            
        }
        
        //if it isn't owner
    }

    void PlayerMovementLogic()
    {
        if (!IsOwner || !IsSpawned)
        {
            return;
        }
        
        currentPlayerVelocity = Vector3.zero;
        
        isRunning = false;
        isMoving = false;

        
        if (Input.GetKey(KeyCode.W))
        {
            currentPlayerVelocity += Time.deltaTime * 1000 * transform.right;
            if(Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            currentPlayerVelocity += Time.deltaTime * 1000 * -transform.right;
            isRunning = false;
        }
        if (Input.GetKey(KeyCode.A))
        {
            currentPlayerVelocity += Time.deltaTime * 1000 * transform.forward;
            isRunning = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentPlayerVelocity += Time.deltaTime * 1000 * -transform.forward;
            isRunning = false;
        }
        currentPlayerVelocity.Normalize();
        isMoving = currentPlayerVelocity != Vector3.zero;

        if (currentPlayerVelocity != Vector3.zero)
            previousPlayerVelocity = currentPlayerVelocity;
        
        if (isMoving)
        {
            if (isRunning && accelerationCoeficient < (maximumAccelerationCoef * runningSpeed))
            {
                accelerationCoeficient += Time.deltaTime * accelerationCoef;
            }
            else if(accelerationCoeficient > maximumAccelerationCoef)
            {
                accelerationCoeficient -= Time.deltaTime * deaccelerationCoef;
            }
            else if(accelerationCoeficient < maximumAccelerationCoef)
            {
                accelerationCoeficient += Time.deltaTime * accelerationCoef;
            }
        }
        else
        {
            if (accelerationCoeficient > 1.0f)
            {
                accelerationCoeficient -= Time.deltaTime * deaccelerationCoef;
            }
        }

        accelerationCoeficient = Mathf.Clamp(accelerationCoeficient, 1, maximumAccelerationCoef * runningSpeed);
        if (currentPlayerVelocity != Vector3.zero)
        {
            PlayerRigidbody.velocity = currentPlayerVelocity * accelerationCoeficient;
        }
        else
        {
            PlayerRigidbody.velocity = previousPlayerVelocity * accelerationCoeficient;
        }

        if (accelerationCoeficient == 1)
        {
            previousPlayerVelocity = Vector3.zero;
        }

        PlayerRigidbody.velocity *= speedModifier;


    }
    
    void PlayerCameraLogic()
    {
        if (!IsOwner || !IsSpawned)
        {
            return;
        }
        
        var objectRotation = this.transform.rotation;
        PlayerRigidbody.MoveRotation(Quaternion.Euler(objectRotation.eulerAngles + new Vector3(0, Input.GetAxisRaw("Mouse X") * sensitivityModifier)));
        Vector3 RotationAddition = new Vector3(0, 0, 0);
        RotationAddition.x -= Input.GetAxisRaw("Mouse Y") * sensitivityModifier;
        var cameraRotation = PlayerCamera.transform.rotation;
        cameraSmoothenessLerpTimer += Time.deltaTime;
        PlayerCamera.transform.rotation = 
            Quaternion.Euler(new Vector3(ClampAngle(Mathf.Lerp(cameraRotation.eulerAngles.x, cameraRotation.eulerAngles.x + RotationAddition.x, cameraSmoothenessLerpTimer), -maxLookAngle, maxLookAngle), cameraRotation.eulerAngles.y, cameraRotation.eulerAngles.z));
            //Quaternion.Euler(new Vector3(cameraRotation.eulerAngles.x, cameraRotation.eulerAngles.y, ClampAngle(Mathf.Lerp(cameraRotation.eulerAngles.z, cameraRotation.eulerAngles.z + RotationAddition.z, cameraSmoothenessLerpTimer), -90, 90)));
        
    }
    
    
    public float ClampAngle(float current, float min, float max)
    {
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;
 
        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        return current;
    }
    
    
    
}


