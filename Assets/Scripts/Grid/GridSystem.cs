using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GridSystem<TGridObject>
{

    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = createGridObject(this, gridPosition);
            }
        }
        
    }



    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

   public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
             Mathf.RoundToInt(worldPosition.z / cellSize));
   }

    public void CreateDebugObjects(Transform debugPrefab) //EL debugPrefab es el prefab del texto que vamos a poner
    {

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform =  GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity); //Inicializamos el grid en la posicion x z
                
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>(); //Recogemos el cuadrado del grid con el transform y el get component
                gridDebugObject.SetGridObject(GetGridObject(gridPosition)); // Y hacemos el set del cuadrado del grid
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0
            && gridPosition.z >= 0
            && gridPosition.x < width
            && gridPosition.z < height;
    }


    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

}
