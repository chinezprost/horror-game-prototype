using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public GameObject TerminalWindow;
    public bool IsTerminalOpen = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
            }
        }
        
    }
    
    
}
