using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissShoot : MonoBehaviour
{

    private GridPosition targetPosition; 

    private void Update()
    {
        float counter = 0;

        //Wait for 4 seconds
        float waitTime = 10;
        while (counter < waitTime)
        {
            counter += Time.deltaTime;
        }
        Destroy(gameObject);
    }


    public void Setup(GridPosition gridPosition)
    {
        targetPosition = gridPosition;
    }
   
}
