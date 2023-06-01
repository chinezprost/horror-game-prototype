using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] 
    public Slider staminaSlider; //placeholder

    private void Start()
    {
        staminaSlider = GameObject.FindWithTag("staminaSlider").GetComponent<Slider>();
    }
}
