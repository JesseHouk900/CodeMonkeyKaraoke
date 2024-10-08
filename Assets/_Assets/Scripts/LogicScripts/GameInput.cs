using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    public event EventHandler<OnPointerHoverStatusChangeEventArgs> _OnPointerEnterBallObstacleMouseProjectionLayer;
    public event EventHandler<OnPointerHoverStatusChangeEventArgs> _OnPointerExitBallObstacleMouseProjectionLayer;
    public class OnPointerHoverStatusChangeEventArgs : EventArgs
    {
        public bool isInside;
    }

    public event EventHandler OnInteractAction;
    public event EventHandler OnJumpAction;
    public event EventHandler OnAltInteractAction;
    //public event EventHandler OnPickupRelease;
    //public event EventHandler OnAltPickupRelease;

    private Vector3 movementVector;
    private PlayerInputActions playerInputActions;
    private Vector3 lastInteractDirection;

    private BallObstacle activeBallObstacle;
    private bool isPointerHoveringOverMouseProjectionLayer;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        EnablePlayer();
        isPointerHoveringOverMouseProjectionLayer = false;
        
    }

    private void Update()
    {
        
        if (IsBallObstacleInControl())
        {
            HandleBallObstacleMovement();
            HandleBallObstacleInteractions();
        }
    }

    private void HandleBallObstacleMovement()
    {
        //Debug.Log($"ball obstacls has prop: {activeBallObstacle.HasPropObject()}");
        //Debug.Log($"isPointerHoveringOverMouseProjectionLayer: {isPointerHoveringOverMouseProjectionLayer}");
        if (activeBallObstacle.HasPropObject() && isPointerHoveringOverMouseProjectionLayer)
        {
            //Debug.Log("Update position");
            activeBallObstacle.UpdateTargetPosition();
        }
    }

    private void HandleBallObstacleInteractions()
    {
        if (!activeBallObstacle.HasPropObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDistance = 1000f;
            if (Physics.Raycast(ray, out RaycastHit hitData, maxDistance))
            {
                // the mouse is over something
                activeBallObstacle.SetTargetedPropObject(hitData.transform.gameObject.GetComponent<PropObject>());
            }
        }
    }
    private void Debug_ModifyMovementSpeed_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log(obj.ToString());
        if (obj.control.ToString().Contains("Plus"))
        {
            PhysicsVisualizationSettings.showAllContacts = true;
        }
        else if (obj.control.ToString().Contains("minus"))
        {
            PhysicsVisualizationSettings.showAllContacts = false;
        }
    }


    private void AltInteract_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAltInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetNormalizedInputVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector3>();

        inputVector = inputVector.normalized;
        movementVector = new Vector3(inputVector.x, 0, inputVector.y);
        //Debug.Log($"inputVector: {inputVector}");
        //Debug.Log($"movementVector: {movementVector}");
        return movementVector;
    }

    public Vector3 GetMousePositionOnPlane(Plane plane, LayerMask layerMask)
    {
        Vector3 result = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        Debug.Log($"layerMask: {layerMask.value}");
        float maxDistance = 1000f;
        if (Physics.Raycast(ray, out hitData, Mathf.Infinity, layerMask))
        {
            result = hitData.point + transform.TransformVector(new Vector3(0, 0, 0.185f));
            Debug.Log($"hitData.transform.gameObject.layer: {hitData.transform.gameObject.layer}");
            Debug.Log($"hitData.point: {hitData.point}");
            Debug.Log($"ray: {ray}");
            Debug.Log($"result: {result}");
        }
        return result;
    }


    
    public void ChangeInputModeFromPlayerToBallObstacle(BallObstacle ballObstacle)
    {
        playerInputActions.Player.Disable();
        EnableBallObstacle(ballObstacle);
        activeBallObstacle = ballObstacle;
        if (playerInputActions.BallObstacle.PickupRelease == null)
        {
            // there are no listeners for the PickupRelease action
            EnableBallObstacle(ballObstacle);
        }

    }

    public void ChangeInputModeFromBallObstacleToPlayer(Player player)
    {
        //Debug.Log("Changing from ball obstacle to player");
        playerInputActions.BallObstacle.Disable();
        EnablePlayer();
        if (playerInputActions.Player.Interact == null)
        {
            // there is no listener for the interact Action
            Debug.LogError("Error: No listeners set for player");
        }
    }

    public bool IsPlayerInControl()
    {
        return playerInputActions.Player.enabled;
    }

    public bool IsBallObstacleInControl()
    {
        return playerInputActions.BallObstacle.enabled;
    }

    private void EnableBallObstacle(BallObstacle ballObstacle)
    {
        playerInputActions.BallObstacle.Enable();

        playerInputActions.BallObstacle.PickupRelease.performed += ballObstacle.GetPickupReleasePerformedHandle();
        playerInputActions.BallObstacle.AltPickupRelease.performed += ballObstacle.GetAltPickupReleasePerformedHandle();
        playerInputActions.BallObstacle.Interact.performed += ballObstacle.GetInteractPerformedHandle();
    }

    private void EnablePlayer()
    {
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.AltInteract.performed += AltInteract_performed;
        playerInputActions.Player.Debug_ModifyMovementSpeed.performed += Debug_ModifyMovementSpeed_performed;

    }

    public void OnPointerEnterBallObstacleMouseProjectionLayer(object sender, OnPointerHoverStatusChangeEventArgs args)
    {
        isPointerHoveringOverMouseProjectionLayer = args.isInside;
        //_OnPointerEnterBallObstacleMouseProjectionLayer?.Invoke(sender, new OnPointerHoverStatusChangeEventArgs()
        //{
        //    isInside = isInside

        //});
    }

    public void OnPointerExitBallObstacleMouseProjectionLayer(object sender, OnPointerHoverStatusChangeEventArgs args)
    {
        isPointerHoveringOverMouseProjectionLayer = args.isInside;
        //_OnPointerExitBallObstacleMouseProjectionLayer?.Invoke(sender, new OnPointerHoverStatusChangeEventArgs()
        //{
        //    isInside = isInside
        //});
    }

    private Vector3 GetProjectedMousePosition()
    {
        // set the targetPlane normal to be the z-axis, since it lines up with the normal for the backboard
        //Vector3 _targetPosition = gameInput.GetMousePositionOnPlane(new Plane(targetPlaneNormal, 0), layerMask);
        Vector3 result = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        float maxDistance = 1000f;
        if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
        {
            Debug.Log($"hitData.point: {hitData.point}");
            Debug.Log($"hitData.transform.name: {hitData.transform.name}");
            if (hitData.transform.gameObject.TryGetComponent(out MouseProjectionLayer mouseProjectionLayer))
            {
                result = hitData.point + transform.TransformVector(new Vector3(0, 0, 0.185f));
                Debug.Log($"hitData.transform.gameObject.layer: {hitData.transform.gameObject.layer}");
                Debug.Log($"hitData.point: {hitData.point}");
                Debug.Log($"ray: {ray}");
                Debug.Log($"result: {result}");

            }
        }
        Vector3 _targetPosition = result;

        // prop object. cast capsule to check if can move

        return _targetPosition;
    }
}
