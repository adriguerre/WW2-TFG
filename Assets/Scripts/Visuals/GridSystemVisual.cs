using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialsList;
    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;
    Unit selectedUnit;
    BaseAction selectedAction;

    private GridPosition actualGridPosition;
     //Singleton

    public static GridSystemVisual Instance { get; private set; }



    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White, 
        Blue, 
        Red, 
        Yellow, 
        RedSoft, 
        Green
    }


    public GridSystemVisualSingle GetGridSingle(int x, int z)
    {
        return gridSystemVisualSingleArray[x, z];
    }
    


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }



    void Start()
    {
        selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridPosition unitPosition = selectedUnit.GetGridPosition();
        actualGridPosition = new GridPosition(unitPosition.x, unitPosition.z);

        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

        for(int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {

                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);              
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        //UpdateGridVisual();
    }

    // Update is called once per frame
    void Update()
    {
        selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if(selectedAction != null &&  (selectedAction.GetActionName() == "Granada" || selectedAction.GetActionName() == "Cohete"))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            if(mouseGridPosition != actualGridPosition)
            {
                if (selectedAction.GetValidActionGridPositionList().Contains(mouseGridPosition))
                {
                   
                    HideGridVisualGrenade(actualGridPosition);
                    UpdateGridVisualGrenade(mouseGridPosition);
                    actualGridPosition = mouseGridPosition;
                }
         
            }
           
        }
    }

 

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        foreach(GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleArray)
        {
            gridSystemVisualSingle.Hide();
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for(int x = -range; x <= range; x++)
        {
            for(int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }             
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range) 
                {                                                                        
                    continue;
                }
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, gridVisualType);
    }    
    
    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for(int x = -range; x <= range; x++)
        {
            for(int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }             
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public GridSystemVisualSingle GetGridSystemVisualSingle(int x, int z) 
    {
        return gridSystemVisualSingleArray[x, z];
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();
         selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
         selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default: 
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;   
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case RocketAction rocketAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case SupressionAction supressionAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), supressionAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case SniperShootAction sniperShootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), sniperShootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case MoveBonusAction moveBonusAction:
                gridVisualType = GridVisualType.Green;
                //Pondriamos green soft
                //ShowGridPositionRange(selectedUnit.GetGridPosition(), moveBonusAction.GetMaxBonusDistance(), GridVisualType.Green);
                break;
            case HealAction healAction:
                gridVisualType = GridVisualType.Green;
                break;
            case ReviveAction reviveAction:
                gridVisualType = GridVisualType.Green;
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    public void UpdateGridVisualGrenade(GridPosition gridPosition)
    {
        for (int x = gridPosition.x - 1; x <= gridPosition.x + 1; x++)
        {
            for(int z = gridPosition.z - 1; z <= gridPosition.z + 1; z++)
            {
                GridPosition testGridPosition = new GridPosition(x, z);
                if(IsValidGridPosition(testGridPosition))
                {        
                    gridSystemVisualSingleArray[x, z].Show(GetGridVisualTypeMaterial(GridVisualType.RedSoft));
                }   
            }
        }
    }
    private void HideGridVisualGrenade(GridPosition actualGridPosition)
    { 
        for (int x = actualGridPosition.x - 1; x <= actualGridPosition.x + 1; x++)
        {
            for (int z = actualGridPosition.z - 1; z <= actualGridPosition.z + 1; z++)
            {
                GridPosition testGridPosition = new GridPosition(x, z);
                if (IsValidGridPosition(testGridPosition)){
                    gridSystemVisualSingleArray[x, z].Hide();

                    if (selectedAction.GetValidActionGridPositionList().Contains(testGridPosition))
                    {
                        gridSystemVisualSingleArray[x, z].Show(GetGridVisualTypeMaterial(GridVisualType.Yellow));
                    }
                }         
            }
        }
    }

    private bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0
            && gridPosition.z >= 0
            && gridPosition.x < LevelGrid.Instance.GetWidth()
            && gridPosition.z < LevelGrid.Instance.GetHeight();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialsList)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        return null;
    }

}
