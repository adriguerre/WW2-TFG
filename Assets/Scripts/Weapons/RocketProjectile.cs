using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
public class RocketProjectile : MonoBehaviour
{

    [SerializeField] private Transform rocketExplodeVFXPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;
    [SerializeField] private GameObject rocket;

    public static event EventHandler onAnyRocketExploded; //Evento para el shake action

    private Vector3 targetPosition;
    private Action onRocketBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ; 

    private void Update()
    {
     
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        float moveSpeed = 25f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;


        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 6.5f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        Vector3 hitBoxSize = new Vector3(3, 3, 3);
        float reachedTargetDistance = 0.2f; 
        if(Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            //Checkeamos a los objetos/enemigos que hemos dado
            Collider[] colliderArray = Physics.OverlapBox(targetPosition, hitBoxSize);//Para saber que enemigos hay dentro del radio de la granada. 

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targerUnit))
                {
                    targerUnit.Damage(30);
                }
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage();

                }


            }
            onAnyRocketExploded?.Invoke(this, EventArgs.Empty);
            trailRenderer.transform.parent = null;
            Instantiate(rocketExplodeVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject);

            onRocketBehaviourComplete();
        }  
}

    public void Setup(GridPosition targetGridPosition, Action onRocketBehaviourComplete)
    {
        this.onRocketBehaviourComplete = onRocketBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(transform.position, targetPosition);
    }
}
