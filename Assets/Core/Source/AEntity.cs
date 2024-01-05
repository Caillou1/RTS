using System.Collections;
using System.Collections.Generic;
using Core.Source;
using UnityEngine;
using UnityEngine.AI;

public class AEntity : MonoBehaviour
{
    public Renderer Selectable;
    public Renderer Selected;

    public List<ACommand> Commands;

    
    public virtual void Move(Vector3 position)
    {
    }
}
