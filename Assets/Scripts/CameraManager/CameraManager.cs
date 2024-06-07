using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CameraManager : MonoBehaviour
{
     private GameObject actionCameraGameObject;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera targetCameraGameObject;
    [SerializeField] private Transform actionCameraPrefab;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        EnemyAI.onAnyActionOfEnemy += EnemyAI_OnAnyActionOfEnemy;
        EnemyAI.onFinishedTurn += EnemyAI_onFinishedTurn;
        Transform actionCamera = Instantiate(actionCameraPrefab, transform);
        actionCameraGameObject = actionCamera.gameObject;
        
        HideActionCamera();

    }

    

    private void EnemyAI_onFinishedTurn(object sender, EventArgs e)
    {
        HideActionCamera();
    }

    private void EnemyAI_OnAnyActionOfEnemy(object sender, Unit e)
    {
        
        //Movemos la camara a donde se situe la unidad
        Vector3 cameraCharacterHeight = Vector3.up * 10f + Vector3.right * 10f; //La altura a la que hay que poner la camara
        Vector3 actionCameraPosition = e.GetWorldPosition() + cameraCharacterHeight;
        if(actionCameraGameObject == null)
        {
            actionCameraGameObject = GameObject.FindGameObjectWithTag("ActionVirtualCamera");
        }
        if(actionCameraGameObject != null)
        {
            actionCameraGameObject.transform.position = actionCameraPosition;
            actionCameraGameObject.transform.LookAt(e.GetWorldPosition());
        }
      
        ShowActionCamera();
    }

    private void ShowActionCamera()
    {
        if(actionCameraGameObject != null)
            actionCameraGameObject.SetActive(true);

    }

    private void HideActionCamera()
    {
        if (actionCameraGameObject != null)
            actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            
            case ShootAction shootAction:
                int random = UnityEngine.Random.Range(0, 100);
                if (random >= 70) //Solo el 30% de las veces sale la camara
                {
                    Unit shooterUnit = shootAction.GetUnit(); //El unit que dispara
                    Unit targetUnit = shootAction.GetTargetUnit(); //A quien dispara

                    Vector3 cameraCharacterHeight = Vector3.up * 2f; //La altura a la que hay que poner la camara
                    Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized; //El vector direccion de donde apunta la camara

                    float shoulderOffsetAmount = 0.5f; //La distancia del hombro, que lo hacemos rotando 90 grados el vector a la derecha
                    Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

                    Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);  //Donde se posiciona la camara
                    
                    if(actionCameraGameObject != null)
                    {
                        actionCameraGameObject.transform.position = actionCameraPosition;
                        actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                    }
                   
                    ShowActionCamera();
                }
                break;


        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
         
        }
    }
}
