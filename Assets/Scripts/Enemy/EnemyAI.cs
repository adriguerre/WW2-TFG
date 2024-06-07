using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }

    public static event EventHandler<Unit> onAnyActionOfEnemy;
    public static event EventHandler onFinishedTurn;

    [SerializeField] private Difficult difficulty; 

    
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn, 
        Busy,
    }

    private State state;
    private float timer;


    private struct BestAction
    {
        public EnemyAIAction bestEnemyAIAction;
       public BaseAction bestBaseAction;
    }

    public void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one EnemyAI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        SetDifficulty(EnemyDifficulty.Instance.GetDifficult());
        
        
        if(TryGetComponent<EnemyDifficulty>(out EnemyDifficulty enemy))
        {
            Debug.Log("Eliminamos");
            Destroy(enemy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        //No hay mas enemigos que puedan usar puntos
                        TurnSystem.Instance.NextTurn();
                        onFinishedTurn?.Invoke(this, EventArgs.Empty);
                    }
                    
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = .5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)

    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }
    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
      
        foreach(Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) //Ejecutamos la mejor opción por todos los enemigos posibles 
        {
           // if (UnitManager.Instance.GetEnemyUnitList()[enemyUnit] == false)
           // {
                //Movemos la cámara al soldado 
                onAnyActionOfEnemy?.Invoke(this, enemyUnit);
                if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
                {
                    return true;
                }
           // }
            
            
        }
        return false;
    }
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        BestAction bestAction = new BestAction();
        foreach(BaseAction action in enemyUnit.GetBaseActionArray()) //Por cada acción que tenga el personaje
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(action)) //Si no puede gastar, no lo tenemos en cuenta
            {
                continue; //Enemigo no puede gastar tantos puntos
            }


            switch (GetDifficult())
            {
                case Difficult.Easy:
                    bestAction = GetBestAction(bestAction, action, Difficult.Easy);
                    break;
                case Difficult.Medium:
                    bestAction = GetBestAction(bestAction, action, Difficult.Medium);
                    break;
                case Difficult.Hard:
                    bestAction = GetBestAction(bestAction, action, Difficult.Hard);
                    break; 
            }
        }

        if(bestAction.bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestAction.bestBaseAction))
         {
            Debug.Log("HEMOS ELEGIDO " + bestAction.bestEnemyAIAction.gridPosition + " con " + bestAction.bestEnemyAIAction.actionValue);
            bestAction.bestBaseAction.TakeAction(bestAction.bestEnemyAIAction.gridPosition, onEnemyAIActionComplete); //Una vez ya visto todos los casos, hacemos la mejor opcion
            return true;
        }
        else
        {
            return false;
        }

    }


    private BestAction GetBestAction(BestAction bestAction, BaseAction action, Difficult diff)
    {
        if (bestAction.bestEnemyAIAction == null) //Primera acción, no hay mejor
        {

            bestAction.bestEnemyAIAction = action.GetBestEnemyAIAction(diff);
           
            bestAction.bestBaseAction = action;
        }
        else //Ya hay alguna
        {
            EnemyAIAction testEnemyAIAction = action.GetBestEnemyAIAction(diff);

            if (testEnemyAIAction != null && testEnemyAIAction.actionValue >= bestAction.bestEnemyAIAction.actionValue) //Checkeamos si hay mejor accion que disparar a John con 122 puntos
            {
                bestAction.bestEnemyAIAction = testEnemyAIAction;
                bestAction.bestBaseAction = action;
            }
        }
        return bestAction;
    }


    public void SetDifficulty(Difficult diff)
    {
        this.difficulty = diff; 
    }

    public Difficult GetDifficult()
    {
        return this.difficulty;
    }
}
