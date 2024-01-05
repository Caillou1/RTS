using System.Collections;
using System.Collections.Generic;
using Core.Source;
using UnityEngine;

public class UICommand : MonoBehaviour
{
    public ACommand Command;

    public void Select()
    {
        if(Command.IsAvailable())
        {
            Command.Select();
        }
    }
}
