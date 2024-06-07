#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }
    private PlayerInputActions playerInputActions; 

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public bool IsEscapeKeyButtonDown()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }


    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM //Usamos el nuevo input manager 
        return Mouse.current.position.ReadValue();
#else // Si queremos usar el legacy input
        return Input.mousePosition;
#endif
    }

    public bool IsLeftMouseButtonDown()
    {
#if USE_NEW_INPUT_SYSTEM //Usamos el nuevo input manager 
        return playerInputActions.Player.LeftClick.WasPressedThisFrame();
#else // Si queremos usar el legacy input
     return Input.GetMouseButtonDown(0);
#endif

    }
    public bool IsRightMouseButtonDown()
    {
#if USE_NEW_INPUT_SYSTEM //Usamos el nuevo input manager 
        return playerInputActions.Player.RightClick.WasPressedThisFrame();
#else // Si queremos usar el legacy input
     return Input.GetMouseButtonDown(1);
#endif

    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputMoveDir = new Vector3(0, 0);
        if (Input.GetKey(KeyCode.W))
            inputMoveDir.y = +1f;
        if (Input.GetKey(KeyCode.S))
            inputMoveDir.y = -1f;
        if (Input.GetKey(KeyCode.A))
            inputMoveDir.x = -1f;
        if (Input.GetKey(KeyCode.D))
            inputMoveDir.x = +1f;

        return inputMoveDir;
#endif
    }

    public float GetCameraRotateAmount()
    {

#if USE_NEW_INPUT_SYSTEM
       return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
       float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
            rotateAmount = +1f;
        if (Input.GetKey(KeyCode.E))
            rotateAmount = -1f;

        return rotateAmount;
#endif

    }


    public Vector3 GetCameraZoomAmount(Vector3 targetFollowOffset)
    {

        float zoomAmountY = 1f;
        float zoomAmountZ = 1f;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmountY;
            targetFollowOffset.z += zoomAmountZ;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmountY;
            targetFollowOffset.z -= zoomAmountZ;
        }
        return targetFollowOffset;

    }

}

