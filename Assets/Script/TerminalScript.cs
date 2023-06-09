using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public enum Colors
{
    Red,
    Green,
    Default,
    LightWarning
};

public class TerminalScript : MonoBehaviour
{
    private string printedTerminalLines = "";
    private List<string> previousCommands = new List<string>();
    public TextMeshProUGUI terminalText;
    public TMP_InputField terminalInputfield;

    private int terminalCommandIndex = 0;
    public bool isTerminalOpen = true;
    
    void Start()
    {
        terminalInputfield.onEndEdit.AddListener(delegate{OnInputFieldEnd(terminalInputfield);});
    }

    void Update()
    {
        TerminalLogic();
    }
    
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), (1.0f / Time.smoothDeltaTime).ToString() );        
    }

    void PrintToTerminal(string value, Colors color = Colors.Default)
    {
        if (value == "")
            return;
        string lineToBePrinted = "";
        lineToBePrinted = color switch
        {
            Colors.Red => lineToBePrinted += "<color=\"red\">",
            Colors.Green => lineToBePrinted += "<color=\"green\">",
            Colors.LightWarning => lineToBePrinted += "<color=#BABABA>",
            _ => lineToBePrinted += "",
        };
        lineToBePrinted += value;
        lineToBePrinted += '\n';
        printedTerminalLines += lineToBePrinted;
        terminalText.text = printedTerminalLines;
    }

    void OnInputFieldEnd(TMP_InputField inputField)
    {
        ComputeCommand(inputField.text);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
        
    }

    void ComputeCommand(string command)
    {
        if (command == "")
            return;
        
        string auxString = "";
        List<string> args = new List<string>();
        int index = 0;
        foreach(var commandCharacter in command)
        {
            if (commandCharacter != ' ')
            {
                auxString += commandCharacter;
            }
            else
            {
                args.Add(auxString);
                auxString = "";
            }

            index++;
        }
        args.Add(auxString);
        auxString = args[0];
        args.RemoveAt(0);
        ProcessCommand(auxString, args);
        previousCommands.Add(command);
        auxString = "";
    }

    void TerminalLogic()
    {
        if (isTerminalOpen)
            return;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (terminalCommandIndex != previousCommands.Count-1)
            {
                terminalCommandIndex++;
                terminalInputfield.text = previousCommands[previousCommands.Count - 1 - terminalCommandIndex];
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (terminalCommandIndex == 0)
            {
                terminalInputfield.text = "";
            }
            else if (terminalCommandIndex != previousCommands.Count)
            {
                terminalCommandIndex--;
                terminalInputfield.text = previousCommands[previousCommands.Count - 1 - terminalCommandIndex];
            }
        }
    }

    void ProcessCommand(string command, List<string> args)
    {
        terminalCommandIndex = 0;
        if (command == "log")
        {
            if (args.Count != 1)
            {
                PrintToTerminal("Wrong args: Usage: [log (text)]", Colors.Red);
                return;
            }

            LogCommand(args[0]);
        }
        else if (command == "ping")
        {
            if (args.Count != 0)
            {
                PrintToTerminal("Wrong args: Usage: [ping]", Colors.Red);
                return;
            }

            PingCommand();
        }
        else if (command == "connect")
        {
            if (args.Count != 2)
            {
                PrintToTerminal("Wrong args: Usage: [connect (ip:port)]", Colors.Red);
                return;
            }

            ConnectCommand(args[0], args[1]);
        }
        else if (command == "clear")
        {
            if (args.Count != 0)
            {
                PrintToTerminal("Wrong args: Usage: [clear]", Colors.Red);
                return;
            }
            
            ClearCommand();
        }
        else if (command == "host")
        {
            if (args.Count != 2)
            {
                PrintToTerminal("Wrong args: Usage: [host (ip:port)]", Colors.Red);
                return;
            }
            
            PrintToTerminal("Trying to host...", Colors.Default);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                args[0],
                (ushort)Int32.Parse(args[1]));
            if (StartHostCommand())
            {
                PrintToTerminal($"Hosted started on: {NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address} and port: {NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port}", Colors.Green);
            }
            else
            {
                PrintToTerminal("Couldn't host server.", Colors.Red);
            }

        }
        else if (command == "server")
        {
            //StartServerCommand();
            PrintToTerminal("Server functionality is disabled until future development...", Colors.Red);

        }
        else if (command == "client")
        {
            StartClientCommand();
            PrintToTerminal("Trying to connect to server...", Colors.Default);

        }
        else
        {
            PrintToTerminal("Command not found.", Colors.LightWarning);
        }
    }
    void ClearCommand()
    {
        terminalText.text = "";
        printedTerminalLines = "";
    }
    void LogCommand(string value)
    {
        if (value.Length != 0) 
            PrintToTerminal(value, Colors.Default);
    }
    void PingCommand()
    {
        PrintToTerminal("Pong!", Colors.Default);
    }
    void ConnectCommand(string ip, string port)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)Int32.Parse(port));
        if (NetworkManager.Singleton.StartClient())
        {
            PrintToTerminal("Connected succesfully to the server!", Colors.Green);
        }
        else
        {
            PrintToTerminal("Couldn't connect to the server!",Colors.Red);
        }
    }
    public bool StartHostCommand()
    {
        return NetworkManager.Singleton.StartHost();
    }
    public void StartServerCommand()
    {
        NetworkManager.Singleton.StartServer();

    }
    public void StartClientCommand()
    {
        NetworkManager.Singleton.StartClient();

    }
}
