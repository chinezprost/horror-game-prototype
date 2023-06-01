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
        #region application settings
        Application.targetFrameRate = 144;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        #endregion
        
        #region clone manager
        if (ClonesManager.IsClone())
        {
            TerminalCommand.StartClientCommand();
        }
        else
        {
            TerminalCommand.StartHostCommand();
        }
        #endregion
    }

}
