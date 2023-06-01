using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Singleton { get; set; }
    private void Awake() //signleton
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    public List<AudioClip> Footstep_SFX_Left;
    public List<AudioClip> Footstep_SFX_Right;
}
