using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;
    private GridPosition gridPosition;
    private Animator animator;
    private Action onInteractComplete;
    private bool isActive;
    private float timer; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if (isOpen)
            OpenDoor();
        else
            CloseDoor();
    }

    private void Update()
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

    //Cuando pasamos un Action(callback), es para que la acción no se termine instantanea, y por tanto se espera un poco
    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f; 
        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.setIsWalkableGridPosition(gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.setIsWalkableGridPosition(gridPosition, false);
    }
}
