using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AUnit : MonoBehaviour
{
    public Renderer Selectable;
    public Renderer Selected;
    public NavMeshAgent Agent;

    public void Move(Vector3 position)
    {
        Agent.SetDestination(position);
    }
}
