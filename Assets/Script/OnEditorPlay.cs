using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParrelSync;

public class OnEditorPlay : MonoBehaviour
{
    public TerminalScript TerminalCommand;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (ClonesManager.IsClone())
        {
            TerminalCommand.StartClientCommand();
        }
        else
        {
            TerminalCommand.StartHostCommand();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
