using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private GameObject cinemachineVirtualCameraGameObject;

    [SerializeField] private GameObject CM_Cinematic;

    private const float MIN_FOLLOW_Y_OFFSET = 3f;
    private const float MAX_FOLLOW_Y_OFFSET = 10f;
    private const float MAX_FOLLOW_Z_OFFSET = -2f;
    private const float MIN_FOLLOW_Z_OFFSET = -11f;


    private Vector3 targetFollowOffset; 
    private CinemachineTransposer cinemachineTransposer;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    // Update is called once per frame
    void Update()
    {

        StartCoroutine(WaitForSeconds());


        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    IEnumerator WaitForSeconds()
    {
        //Poner 18
        yield return new WaitForSeconds(17);
        cinemachineVirtualCameraGameObject.SetActive(true);
        CM_Cinematic.SetActive(false);
    }

    private void HandleMovement()
    {

        Vector2 inputMoveDir = GetCameraMoveVector();
            float moveSpeed = 10f;
            Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
            transform.position += moveVector * moveSpeed * Time.deltaTime;
   

    }

    public Vector2 GetCameraMoveVector()
    {
        Vector2 inputMoveDir = new Vector3(0, 0);
        if (Input.GetKey(KeyCode.W) && CanPressWLimits() && (transform.position.z <= 60 || transform.rotation.y >= 0.8 && transform.rotation.y <= 1))
            inputMoveDir.y = +1f;
        if (Input.GetKey(KeyCode.S) && CanPressSLimits() && (transform.position.z >= 3.7 || transform.rotation.y >= 0.8 && transform.rotation.y <= 1))
            inputMoveDir.y = -1f;
        if (Input.GetKey(KeyCode.A) && CanPressALimits() && (transform.position.x >= 0 || transform.rotation.y >= 0.2 && transform.rotation.y <= 0.8))
            inputMoveDir.x = -1f;
        if (Input.GetKey(KeyCode.D) && CanPressDLimits() && (transform.position.x <= 60 || transform.rotation.y >= 0.2 && transform.rotation.y <= 0.8))
            inputMoveDir.x = +1f;

        return inputMoveDir;

    }


    private bool CanPressSLimits()
    {
        if(transform.rotation.y >= 0.8)
        {
            if(transform.position.z >= 60)
            {
                return false;
            }     
        }
            if(transform.position.x <= 0 || transform.position.x >= 60)
            {
                return false; 
            }
        return true; 
    }
    private bool CanPressWLimits()
    {
        if (transform.rotation.y >= 0.6)
        {
            if (transform.position.z <= 3.7)
            {
                return false;
            }
        }
        if (transform.rotation.y >= 0.4)
        {
            if (transform.position.x <= -5 || transform.position.x >= 65)
            {
                return false;
            }
        }
        return true;
    }
    private bool CanPressALimits()
    {
  
        if (transform.position.z <= 3.7 || transform.position.z >= 60)
        {
            return false;
        }
        if (transform.rotation.y >= 0.4)
        {
            if (transform.position.x <= 0 || transform.position.x >= 60)
            {
                return false;
            }
        }
        return true;
    }
    private bool CanPressDLimits()
    {

        if (transform.position.z <= 3.7 || transform.position.z >= 60)
        {
            return false;
        }
        if (transform.rotation.y >= 0.4)
        {
            if (transform.position.x <= 0 || transform.position.x >= 60)
            {
                return false;
            }
        }
        return true;
    }



    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();
        
        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }
    private void HandleZoom()
    {
        targetFollowOffset = InputManager.Instance.GetCameraZoomAmount(targetFollowOffset);


        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        targetFollowOffset.z = Mathf.Clamp(targetFollowOffset.z, MIN_FOLLOW_Z_OFFSET, MAX_FOLLOW_Z_OFFSET);
        float zoomSpeed = 5f;
        //EL lerp lo usamos para hacerlo un poco mas suave el cambio (Posicion inicial, posicion final, tiempo)
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
