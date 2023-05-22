using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public GameObject TerminalWindow;
    private TMP_InputField TerminalWindow_InputField;
    public bool IsTerminalOpen = false;
    
    void Start()
    {
        TerminalWindow_InputField = TerminalWindow.GetComponentInChildren<TMP_InputField>();
    }
    
    void Update()
    {
        TerminalEventListener();
    }

    void TerminalEventListener()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (IsTerminalOpen)
            {
                IsTerminalOpen = false;
                TerminalWindow.SetActive(false);
            }
            else
            {
                IsTerminalOpen = true;
                TerminalWindow.SetActive(true);
                TerminalWindow_InputField.ActivateInputField();
            }
        }
        
    }
    
    
}
