using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private GridSystemVisualSingle gridSystemVisualSingle;

    private GridSystemVisualSingle actualGridSystemVisualSingle;

    private void Awake()
    {
            instance = this;
  
    }

    void Update()
    {

        transform.position = MouseWorld.GetPosition();

        GridPosition gridPositon = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
     

        if (LevelGrid.Instance.IsValidGridPosition(gridPositon))
        {
            gridSystemVisualSingle = GridSystemVisual.Instance.GetGridSystemVisualSingle(
             gridPositon.x, gridPositon.z);
            if(actualGridSystemVisualSingle == null)
            {
                gridSystemVisualSingle.StartAnimation();
                actualGridSystemVisualSingle = gridSystemVisualSingle;
            }

            if (actualGridSystemVisualSingle != gridSystemVisualSingle)
            {
                actualGridSystemVisualSingle.FinishAnimation();
                gridSystemVisualSingle.StartAnimation();
                actualGridSystemVisualSingle = gridSystemVisualSingle;
            }
        
        }
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        //Usamos el ray, para detectar donde esta el raton, y le ponemos que solo detecte el suelo con el layer elegido
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);

        return raycastHit.point;
    }

}
