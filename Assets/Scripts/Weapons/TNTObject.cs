using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TNTObject : MonoBehaviour
{

    [SerializeField] private Transform TNTExplodeVFXPrefab;


    private Action onTNTBehaviourComplete;
    private Vector3 tntPosition;

    public static event EventHandler onAnyTNTExploded; //Evento para el shake action
    public void Explode()
    {
        onAnyTNTExploded?.Invoke(this, EventArgs.Empty);
        Instantiate(TNTExplodeVFXPrefab, tntPosition + Vector3.up * 1f, Quaternion.identity);
        //Destroy(gameObject);
    }

    public void Setup(GridPosition gridPosition, Action onTNTBehaviourComplete)
    {
        this.onTNTBehaviourComplete = onTNTBehaviourComplete;
        tntPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

    }
}
