using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Pathfinding;

public class Bot : Character
{
    public Transform Target;
    [SerializeField] Transform Chest;
    public bool UseAI = false;

    private void Start()
    {
        // Target = FindObjectOfType<Player>().transform;
        // movement.GoToPosition(Target);
        animationController.Set8WayLayerWeight(false);
    }

    public void GotoTarget()
    {
        movement.GoToPosition(Target);
    }
    public void GotoPos(Vector3 pos)
    {
        movement.GoToPosition(pos);
    }
    public void GotoPath(List<Vector3> path)
    {
        // movement.GotoPath(path);
    }

    public override void Die()
    {
        base.Die();
        // rb.isKinematic = true;
        FindObjectOfType<Player>().IncreaseKillCount();
    }
}
