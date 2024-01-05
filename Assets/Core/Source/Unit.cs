using System.Collections;
using System.Collections.Generic;
using Core.Source;
using UnityEngine;
using UnityEngine.AI;

public class Unit : AEntity
{
    public NavMeshAgent Agent;

    public override void Move(Vector3 position)
    {
        Agent.SetDestination(position);
    }
}
