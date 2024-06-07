using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GrenadeProjectile : MonoBehaviour
{

    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    public static event EventHandler OnAnyGrenadeExploded;

    private Vector3 targetPosition;
    private Action OnGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        float moveSpeed = 15f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;


        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;


        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        Vector3 hitBoxSize = new Vector3(3,3,3);
        float reachedTargetDistance = .2f;
        if(Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {

            //Checkeamos a los objetos/enemigos que hemos dado
            Collider[] colliderArray = Physics.OverlapBox(targetPosition, hitBoxSize);//Para saber que enemigos hay dentro del radio de la granada. 

            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent<Unit>(out Unit targerUnit))
                {
                    targerUnit.Damage(30);
                }


            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            trailRenderer.transform.parent = null;
            Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            Destroy(gameObject);

            OnGrenadeBehaviourComplete();
        }
    }


    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.OnGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(transform.position, targetPosition);
    }

}
