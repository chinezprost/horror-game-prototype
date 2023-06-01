using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    #region scripts/componenets
    public PlayerUIController PlayerUI;
    public PlayerInventory PlayerInventory;
    public CameraController PlayerCameraController;
    public SoundManager PlayerSoundManager;
    public Rigidbody PlayerRigidbody;
    #endregion
    
    #region gameobjects
    public GameObject PlayerCamera;
    public GameObject CameraHolder;
    public GameObject PlayerBody;
    #endregion
    
    [Tooltip("PlayerSpeed")][SerializeField] private float speedModifier = 4;
    [SerializeField] private float runningSpeed = 4;
    [SerializeField] private float sensitivityModifier = 1f;

    [SerializeField] private float maxLookAngle = 87f;
    [SerializeField] private float cameraSmoothenessLerpTimer = 0;
    [SerializeField] private float accelerationCoeficient = 0;

    [SerializeField] public bool isMoving = false;
    [SerializeField] public bool isRunning = false;
    
    [SerializeField] private float staminaIncreaseCoef = 1.7f;
    [SerializeField] private float staminaUsageCoef = 0.8f;

    [SerializeField] private bool canRun = true;
    
    [SerializeField] private float playerStamina = 10f;
    
    [SerializeField] private bool isGrounded = false;

    [SerializeField] private float lastRunTime = -1f;
    
    [SerializeField] public float stepTime = 3f;
    [SerializeField] public float minStepTime = 1f;
    [SerializeField] public float maxStepTime = 1f;
    
    [SerializeField] private float lastStepTime = -1f;
    [SerializeField] private int leftStep = 0;
    
    [SerializeField] public float deaccelerationCoef = 50;
    [SerializeField] public float accelerationCoef = 10;
    [SerializeField] public float maximumAccelerationCoef = 10;

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
    
    #region collision check
    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.otherCollider.tag.Contains("Ground"))
            {
                isGrounded = true;
            }
        }
        
    }
    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }
    #endregion
    
    void PlayerInitialization()
    {
        if (!IsOwner || !IsSpawned)
            return;
            
            
        PlayerRigidbody = this.gameObject.AddComponent<Rigidbody>();
        PlayerCameraController = this.gameObject.AddComponent<CameraController>();
        PlayerSoundManager = this.gameObject.AddComponent<SoundManager>();
        PlayerInventory = this.gameObject.AddComponent<PlayerInventory>();
        PlayerUI = this.gameObject.AddComponent<PlayerUIController>();
        PlayerBody = GameObject.Find("Player_Body");
            
        PlayerCameraController.enabled = true;
        PlayerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        PlayerRigidbody.automaticInertiaTensor = false;
        PlayerRigidbody.inertiaTensor = new Vector3(0, 0, 0);
            
        PlayerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        PlayerCamera = new GameObject("Camera Holder");
        PlayerCamera.transform.localRotation = Quaternion.Euler(0, 90, 0);
        if (CameraHolder == null)
            Debug.Log("Error while creating player camera holder.");
            
        if (PlayerCamera == null)
            Debug.Log("Error while creating player camera.");
        

        PlayerCamera.transform.parent = this.transform;
        Camera.main.transform.parent = PlayerCamera.transform;
        PlayerCamera.transform.parent = Camera.main.transform;
        PlayerCamera.transform.position = new Vector3(0, 2, 0);
        PlayerCameraController.camera = Camera.main.transform;
        PlayerCameraController.cameraHolder = PlayerCamera.transform;
    }
    void PlayerMovementLogic()
    {
        if (!IsOwner || !IsSpawned)
            return;
        
        PlayerFootstepSFXLogic();
        
        if (!isRunning && playerStamina < 10)
        {
            canRun = false;
            if(Time.time - lastRunTime > 4.5f)
                playerStamina += Time.deltaTime * staminaIncreaseCoef;

        }
        
        if (playerStamina >= 3)
            canRun = true;
        
        PlayerUI.staminaSlider.value = playerStamina;
        currentPlayerVelocity = Vector3.zero;
        isRunning = false;
        isMoving = false;
        
        if (Input.GetKey(KeyCode.W))
        {
            currentPlayerVelocity += Time.deltaTime * 1000 * transform.right;
            if(Input.GetKey(KeyCode.LeftShift) && canRun && playerStamina > 0)
            {
                isRunning = true;
                lastRunTime = Time.time;
                playerStamina -= Time.deltaTime * staminaUsageCoef;
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
                accelerationCoeficient += Time.deltaTime * accelerationCoef;
            else if(accelerationCoeficient > maximumAccelerationCoef)
                accelerationCoeficient -= Time.deltaTime * deaccelerationCoef;
            else if(accelerationCoeficient < maximumAccelerationCoef)
                accelerationCoeficient += Time.deltaTime * accelerationCoef;
            
        }
        else if (accelerationCoeficient > 1.0f)
            accelerationCoeficient -= Time.deltaTime * deaccelerationCoef;
        

        accelerationCoeficient = Mathf.Clamp(accelerationCoeficient, 1, maximumAccelerationCoef * runningSpeed);
        if (currentPlayerVelocity != Vector3.zero)
            PlayerRigidbody.velocity = currentPlayerVelocity * accelerationCoeficient;
        else
            PlayerRigidbody.velocity = previousPlayerVelocity * accelerationCoeficient;
        
        if (accelerationCoeficient == 1)
            previousPlayerVelocity = Vector3.zero;
        PlayerRigidbody.velocity *= speedModifier;
    }

    void PlayerFootstepSFXLogic()
    {
        if (isMoving)
        {
            if (Time.time - lastStepTime >= Mathf.Clamp(stepTime - PlayerRigidbody.velocity.magnitude, minStepTime, maxStepTime))
            {
                RpcManager.Singleton.CreateFootstepServerRpc(transform.position, leftStep, isRunning);
                if (leftStep == 0)
                {
                    leftStep = 1;
                }
                else
                {
                    leftStep = 0;
                }
                lastStepTime = Time.time;
            }
            
        }
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
        PlayerCamera.transform.rotation = Quaternion.Euler(new Vector3(ClampAngle(Mathf.Lerp(cameraRotation.eulerAngles.x, cameraRotation.eulerAngles.x + RotationAddition.x, cameraSmoothenessLerpTimer), -maxLookAngle, maxLookAngle), cameraRotation.eulerAngles.y, cameraRotation.eulerAngles.z));
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


