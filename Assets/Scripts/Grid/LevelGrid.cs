using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LevelGrid : MonoBehaviour
{

    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMovedGridPosition;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Transform gridDebugObjectPrefab;

    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;


        gridSystem = new GridSystem<GridObject>(width, height, cellSize,
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(gridPosition, g));
       //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);

    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnitInGridObject(unit);
    }


    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitListInGridObject();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnitFromUnitList(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public CoverType GetCoverTypeAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        if (gridObject.GetCoverObject() == null)
            return CoverType.None;
        else
            return gridObject.GetCoverObject().GetCoverType();
    }

    public void SetCoverObjectAtGridPosition(GridPosition gridPosition, CoverObject coverObject)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetCoverObject(coverObject);
    }


    //Hay que hacer un método para obtener si tiene cobertura en una de los 4 lados. 
    public CoverType GetUnitCoverType(GridPosition gridPosition)
    {
        int x = gridPosition.x;
        int z = gridPosition.z;

        GridPosition leftPosition = new GridPosition(x-1, z);
        GridPosition rightPosition = new GridPosition(x+1, z);
        GridPosition upPosition = new GridPosition(x, z+1);
        GridPosition downPosition = new GridPosition(x, z-1);

        bool validLeft = gridSystem.IsValidGridPosition(leftPosition);
        bool validRight = gridSystem.IsValidGridPosition(rightPosition);
        bool validUp = gridSystem.IsValidGridPosition(upPosition);
        bool validDown = gridSystem.IsValidGridPosition(downPosition);

        CoverType leftCover = CoverType.None;
        CoverType rightCover = CoverType.None;
        CoverType upCover = CoverType.None;
        CoverType downCover = CoverType.None;

        if (validLeft)
        {
            
            if (gridSystem.GetGridObject(leftPosition).GetCoverObject() != null)
                leftCover = gridSystem.GetGridObject(leftPosition).GetCoverObject().GetCoverType();
        }
        if(validRight)
        if (gridSystem.GetGridObject(rightPosition).GetCoverObject() != null)
                rightCover = gridSystem.GetGridObject(rightPosition).GetCoverObject().GetCoverType(); 
        if (validUp)
        if (gridSystem.GetGridObject(upPosition).GetCoverObject() != null)
                upCover = gridSystem.GetGridObject(upPosition).GetCoverObject().GetCoverType();
        if (validDown)
            if (gridSystem.GetGridObject(downPosition).GetCoverObject() != null)
                downCover = gridSystem.GetGridObject(downPosition).GetCoverObject().GetCoverType();

        if(leftCover == CoverType.Full || rightCover == CoverType.Full 
            || upCover == CoverType.Full || downCover == CoverType.Full) 
            
            return CoverType.Full;

        if (leftCover == CoverType.Half || rightCover == CoverType.Half
            || upCover == CoverType.Half || downCover == CoverType.Half)

            return CoverType.Half;



        return CoverType.None;

    }


    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }
    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }

    
}

