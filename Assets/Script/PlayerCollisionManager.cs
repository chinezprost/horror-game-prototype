using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    [SerializeField] public bool isInJumpingTrigger = false;
    [SerializeField] public bool isInwardsTrigger = false;
    [SerializeField] private Transform jumpingPrefab;
    [SerializeField] public Transform jump_trigger_in, jump_trigger_out;
    
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "JumpOver_Collider_In")
        {
            isInJumpingTrigger = true;
            jumpingPrefab = other.transform.parent;
            jump_trigger_in = other.transform;
            isInwardsTrigger = true;
            jump_trigger_out = jumpingPrefab.Find("Jump_Trigger_Back");
        }

        if (other.tag == "JumpOver_Collider_Out")
        {
            isInJumpingTrigger = true;
            jumpingPrefab = other.transform.parent;
            jump_trigger_out = other.transform;
            isInwardsTrigger = false;
            jump_trigger_in = jumpingPrefab.Find("Jump_Trigger_Front");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isInJumpingTrigger = false;
        jumpingPrefab = jump_trigger_in = jump_trigger_out = null;
        isInwardsTrigger = false;
    }
}
