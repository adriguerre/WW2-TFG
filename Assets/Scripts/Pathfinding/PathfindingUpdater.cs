using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingUpdater : MonoBehaviour
{

    public void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
    }

    private void DestructibleCrate_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        Pathfinding.Instance.setIsWalkableGridPosition(destructibleCrate.GetGridPosition(), true);
    }
}
