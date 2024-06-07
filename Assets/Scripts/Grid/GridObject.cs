using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    private List<Unit> unitList;
    private IInteractable interactable;
    private CoverObject coverObject;

    public GridObject(GridPosition gridPosition, GridSystem<GridObject> gridSystem)
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        unitList = new List<Unit>();
    } 

    public void AddUnitInGridObject(Unit unit)
    {
        this.unitList.Add(unit);
    }

    public List<Unit> GetUnitListInGridObject()
    {
        return this.unitList;
    }
    
    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void RemoveUnitFromUnitList(Unit unit)
    {
        this.unitList.Remove(unit);
    }


    public override string ToString()
    {
        string unitString = "";
        foreach(Unit unit in unitList)
        {
            unitString += unit + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
            return unitList[0];
        else
            return null;
    }

    public CoverObject GetCoverObject()
    {
        return coverObject; 
    }

    public void SetCoverObject(CoverObject cover)
    {
        this.coverObject = cover; 
    }
    public IInteractable GetInteractable()
    {
        return interactable;
    }

    public void SetInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }

}
