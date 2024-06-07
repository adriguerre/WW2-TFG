using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro fCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private SpriteRenderer isWalkableSpriteRenderer;
    private PathNode pathNode;
    public override void SetGridObject(object gridObject)
    {
        pathNode = (PathNode)gridObject;
        base.SetGridObject(gridObject);
    }

    protected override void Update()
    {
        base.Update();
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
        isWalkableSpriteRenderer.color = pathNode.IsWalkable() ? Color.green : Color.red;
        
    }
}
