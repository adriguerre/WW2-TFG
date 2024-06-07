using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{

    //Singleton
    public static UnitActionSystem Instance { get; private set; }


   public event EventHandler OnSelectedUnitChanged; 
   public event EventHandler OnSelectedActionChanged;
   public event EventHandler<bool> OnBusyChanged;
   public event EventHandler onActionStarted;
   public static event EventHandler onAnySelectedChangedSoHidePercent;


   [SerializeField] private Unit selectedUnit;
   [SerializeField] private LayerMask unitLayerMask;
    private BaseAction selectedAction;

    private bool isBusy;

    private void DataPersistence_OnLoad(object sender, EventArgs e)
    {
        //Debug.Log(UnitManager.Instance.GetFriendlyUnitList()[0]);

    }

    public void LoadSelectedUnit(Unit unit)
    {
        SetSelectedUnit(unit);
        SetSelectedAction(selectedUnit.GetAction<MoveAction>());
    }


    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    private void Start()
    {
        DataPersistenceManager.Instance.onLoad += DataPersistence_OnLoad;
        SetSelectedUnit(selectedUnit);
        SetSelectedAction(selectedAction);
    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(GameManager.Instance.GetGameState() != GameState.gamePaused)
        {
            TryHandleUnitSelection();
            HandleSelectedAction();
        }


    }

   

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void TryHandleUnitSelection()
    {

        if (InputManager.Instance.IsRightMouseButtonDown())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit) 
                    {
                        return;
                    }
                    if (unit.IsEnemy())
                    {
                        return;
                    }
                    SetSelectedUnit(unit);

                }
            }
        }
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsLeftMouseButtonDown())
        {

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition) && selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);
                onActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void SetSelectedUnit(Unit unit)
    {
        if (unit.GetIsWounded()) {
            return;
        }
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        //Lanzamos el evento para cambiar el SelectedVIsual
        //EL ? lo que hace es comprobar si es null y sigue si no lo es
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        onAnySelectedChangedSoHidePercent?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        this.selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        if (baseAction.GetActionName() != "Shoot")
            onAnySelectedChangedSoHidePercent?.Invoke(this, EventArgs.Empty);
            
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    public bool GetIsBusy()
    {
        return isBusy;
    }
}

