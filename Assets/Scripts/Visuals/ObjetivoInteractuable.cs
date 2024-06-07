using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetivoInteractuable : MonoBehaviour, IInteractable
{
    private GridPosition gridPosition;
    [SerializeField] private ObjetivoInteractuar objetivo;
    private bool isDone;
    private Action onInteractComplete;
    private bool isActive;
    private float timer;
    [SerializeField] private GameObject star;

    void Start()
    {
        isDone = false;
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
    }

    public bool getIsCompleted()
    {
        return this.objetivo.getIsCompleted();
    }

    public void SetObjeto(ObjetivoInteractuar aux)
    {
        this.objetivo = aux;
    }
    
    void Update()
    {
        timer -= Time.deltaTime;

        if (!isActive)
        {
            return;
        }
        if (timer <= 0)
        {
            
            isActive = false;
            onInteractComplete();
        }

    }

    public GameObject GetStar()
    {
        return star; 
    }

    public bool GetIsDone()
    {
        return isDone; 
    }

    public void SetIsDone(bool done)
    {
        this.isDone = done; 
    }

    public void Interact(Action onInteractionComplete)
    {

    
        isActive = true;
        this.onInteractComplete = onInteractionComplete;
        isDone = true;
        objetivo.objetivoCompletado();
    }

}
