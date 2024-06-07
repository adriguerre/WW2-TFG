using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DestructibleCrate : MonoBehaviour
{

    [SerializeField] private Transform crateDestroyedPrefab; 

   

    public static event EventHandler OnAnyDestroyed;

    private GridPosition gridPosition;

    public void Start()
    {
       gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

    }

    public void Damage()
    {
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position, 10f);
        LevelGrid.Instance.SetCoverObjectAtGridPosition(gridPosition, null);
        UpdateCoversAfterDestroy(gridPosition);
        //Y ahora tengo que actualizar las unidades, que estaban a cubierto
        Destroy(gameObject);
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateCoversAfterDestroy(GridPosition gridPosition)
    {
        for (int x = gridPosition.x - 1; x <= gridPosition.x + 1; x++)
        {
            for (int z = gridPosition.z - 1; z <= gridPosition.z + 1; z++)
            {
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(new GridPosition(x, z));

                if (unit != null)
                {
                    unit.UpdateCoverType();
                }
                
            }
        }
    }



    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }

}
