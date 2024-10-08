using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static GameInput;

public class BallObstacle : MonoBehaviour, IPropObjectParent, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //public event EventHandler<OnSelectedPropObjectChangedEventArgs> OnSelectedPropObjectChanged;
    //public class OnSelectedPropObjectChangedEventArgs : EventArgs
    //{
    //    public PropObject selectedProp;
    //}

    //public event EventHandler OnPlayerInteract;


    // ball needs to be changed to a user defined component class, like PropObject, but probably not
    [SerializeField] private BallObstacleBall ballObstacleBall;
    [SerializeField] private CameraManager cameraManager;

    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private Transform StartPropObjectHoldPoint;
    [SerializeField] private GameInput gameInput;


    private PropObject selectedProp;
    private bool isBeingInteractedWith;
    //private Vector3 movementVector;
    private Vector3 targetPosition;
    private Vector3 lastValidPosition;
    private PropObject targetedProp;
    private bool isCollidingWithPropObject;
    private Vector3 collisionBarrierPoint;


    private void Start()
    {
        isBeingInteractedWith = false;
        isCollidingWithPropObject = false;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (selectedProp != null)
        {
            //selectedProp.transform.localPosition += movementVector;
            selectedProp.GetComponent<Rigidbody>().MovePosition(targetPosition);
            Debug.Log($"targetPosition for selected prop: {targetPosition}");
            //selectedProp.transform.position = targetPosition;

        }

    }


    public void Interact(Player player)
    {
        Debug.Log($"Interacting with ballObstacle: {isBeingInteractedWith}");
        if (gameInput == null)
        {
            gameInput = player.GetGameInput();
        }
        ToggleInteraction(player);
        isBeingInteractedWith = !isBeingInteractedWith;

        if (isBeingInteractedWith)
        {
            // the ball obstacle is being interacted with
            if (player == null)
            {
                // player parameter is empty
                if (HasPropObject())
                {
                    // there is an object being held
                    GetPropObject().GetComponent<Rigidbody>().isKinematic = false;
                    GetPropObject().SetPropObjectParent(this);
                    float localZOffset = StartPropObjectHoldPoint.localPosition.z - 0.9f;
                    GetPropObject().transform.localPosition += new Vector3(0, 0, localZOffset);

                }
            }
            if (player.HasPropObject())
            {

                player.GetPropObject().SetPropObjectParent(this);

            }
        }
    }

    private void HandleInteractions()
    {

    }

    private void HandleMovement()
    {

    }

    public void UpdateTargetPosition()
    {
        //if (!isCollidingWithPropObject)
        //{
        // a prop object is colliding with the obstacle (should check specifically for player held object)
        Vector3 projectedMousePosition = GetProjectedMousePosition();
        //if (projectedMousePosition != lastValidPosition)
        //{
        //    // the mouse projection does not match the last position of the object (maybe useless to check)
        //    Ray attemptedDisplacementRay = new Ray(lastValidPosition, (projectedMousePosition - lastValidPosition).normalized);
        //    if (Physics.Raycast(attemptedDisplacementRay, out RaycastHit hitData, (projectedMousePosition - lastValidPosition).magnitude))
        //    {
        //        if (TryGetComponent<
        //    }
        //    Vector3 predictedTransformPosition = 

        //    lastValidPosition = projectedMousePosition;
        //}
        lastValidPosition = projectedMousePosition;
        if (collisionBarrierPoint != Vector3.zero)
        {
            Debug.Log($"lastValidPosition: {lastValidPosition}");
            Debug.Log($"collisionBarrierPoint: {collisionBarrierPoint}");
            Debug.Log($"targetPosition: {targetPosition}");
            // A collision point has been set
            if (Mathf.Abs(collisionBarrierPoint.x - lastValidPosition.x) > 0.2f)
            {
                // there is a horrizontal barrier
                targetPosition = new Vector3(collisionBarrierPoint.x, lastValidPosition.y, lastValidPosition.z);

            }
            else if (Mathf.Abs(collisionBarrierPoint.y - lastValidPosition.y) > 0.2f) 
            {
                // there is a vertical barrier
                targetPosition = new Vector3(lastValidPosition.x, collisionBarrierPoint.y, lastValidPosition.z);

            }
            else
            {
                // there is a barrier, but the lastValidPosition is close to it
                targetPosition = lastValidPosition;
            }
        }
        else
        {
            targetPosition = lastValidPosition;

        }

        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PropObject>(out PropObject propObject))
        {
            collisionBarrierPoint = new Vector3(other.transform.position.x, other.transform.position.y, targetPosition.z);

            isCollidingWithPropObject = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.TryGetComponent<PropObject>(out PropObject propObject))
        {
            collisionBarrierPoint = Vector3.zero;
            isCollidingWithPropObject = false;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PropObject>(out PropObject propObject))
        {
            RaycastHit hit;
            if (Physics.Raycast(propObject.transform.position, propObject.transform.forward, out hit))
            {
                Debug.Log("Point of contact: " + hit.point);
            }
            collisionBarrierPoint = new Vector3(other.transform.position.x, other.transform.position.y, targetPosition.z);
        }

    }
    private Vector3 GetMovementVector()
    {
        //Vector3 targetPosition = GetProjectedMousePosition();
        Vector3 normalizedTargetDirection = (targetPosition - transform.position).normalized;
        float movementSpeed = 1f;
        return normalizedTargetDirection * movementSpeed * Time.deltaTime;
    }

    private Vector3 GetProjectedMousePosition()
    {
        // set the targetPlane normal to be the z-axis, since it lines up with the normal for the backboard
        //Vector3 _targetPosition = gameInput.GetMousePositionOnPlane(new Plane(targetPlaneNormal, 0), layerMask);
        Vector3 result = lastValidPosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitData = Physics.RaycastAll(ray, Mathf.Infinity);
        float maxDistance = 1000f;
        //Debug.Log($"Ray: {ray}");
        foreach (RaycastHit hit in hitData)
        //if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
        {
            //Debug.Log($"hitData.point: {hit.point}");
            //Debug.Log($"hitData.transform.name: {hit.transform.name}");
            if (hit.transform.gameObject.TryGetComponent(out MouseProjectionLayer mouseProjectionLayer))
            {
                result = hit.point + transform.TransformVector(new Vector3(0, 0, 0.185f));
                //Debug.Log($"hitData.transform.gameObject.layer: {hit.transform.gameObject.layer}");
                //Debug.Log($"hitData.point: {hit.point}");
                //Debug.Log($"ray: {ray}");
                //Debug.Log($"result: {result}");

            }
        }

        // prop object. cast capsule to check if can move

        return result;
    }

    public Transform GetCameraFollowTransform()
    {
        return _camera.transform;
    }
    public BallObstacleBall GetBallObstacleBall()
    {
        return ballObstacleBall;
    }

    public void SetBallObstacleBall(BallObstacleBall ballObstacleBall)
    {
        this.ballObstacleBall = ballObstacleBall;
        this.ballObstacleBall.GetComponent<Rigidbody>().isKinematic = false;
    }
    public void ClearBallObstacleBall()
    {
        ballObstacleBall = null;
    }

    public bool HasBallObstacleBall()
    {
        return ballObstacleBall != null;
    }

    public bool IsBeingInteractedWith()
    {
        return isBeingInteractedWith;
    }

    private void ToggleInteraction(Player player)
    {
        Debug.Log("Toggleing interaction");
        cameraManager.SwitchCamera(_camera);
        if (isBeingInteractedWith)
        {
            // the ball obstacle is being interacted with

            Debug.Log("obstacle is being interacted with and is being deactivated");
            
            if (player == null)
            {
                gameInput.ChangeInputModeFromBallObstacleToPlayer(Player.Instance);
                Player.Instance.GetPlayerVisual().Show();
            }
            else
            {
                gameInput.ChangeInputModeFromBallObstacleToPlayer(player);
                player.GetPlayerVisual().Show();

            }

        } // the ball obstacle is not being interacted with
        else if (player != null)
        {
            Debug.Log("obstacle is deactivated and is yet to be interacted with");
            gameInput.ChangeInputModeFromPlayerToBallObstacle(this);
            player.GetPlayerVisual().Hide();

        }
        else
        {
            Debug.LogError("ERROR: Invalid interaction toggle, player is null and ballObject is not being interacted with");
        }
    }


    public Action<InputAction.CallbackContext> GetPickupReleasePerformedHandle()
    {
        return PickupRelease_performed;
    }

    private void PickupRelease_performed(InputAction.CallbackContext context)
    {

    }

    public Action<InputAction.CallbackContext> GetAltPickupReleasePerformedHandle()
    {
        return AltPickupRelease_performed;
    }

    private void AltPickupRelease_performed(InputAction.CallbackContext context)
    {

    }

    public Action<InputAction.CallbackContext> GetInteractPerformedHandle()
    {
        return Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext context)
    {
        Interact(null);
    }

    public Action<InputAction.CallbackContext> GetAltInteractHandle()
    {
        return AltInteract_performed;
    }

    private void AltInteract_performed(InputAction.CallbackContext context)
    {

    }

    public Transform GetPropObjectFollowTransform()
    {
        return StartPropObjectHoldPoint;
    }

    public PropObject GetPropObject()
    {
        return selectedProp;
    }

    public void SetPropObject(PropObject propObject)
    {
        selectedProp = propObject;
    }

    public void ClearPropObject()
    {
        selectedProp = null;
    }

    public bool HasPropObject()
    {
        return selectedProp != null;
    }

    public void Reset()
    {
        GetBallObstacleBall().Reset();
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Ball Obstacle pointer enter");
        if (gameInput.IsBallObstacleInControl())
        {
            gameInput.OnPointerEnterBallObstacleMouseProjectionLayer(this, new OnPointerHoverStatusChangeEventArgs()
            {
                isInside = true
            });
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Ball Obstacle pointer exit");
        if (gameInput.IsBallObstacleInControl())
        {
            gameInput.OnPointerExitBallObstacleMouseProjectionLayer(this, new OnPointerHoverStatusChangeEventArgs()
            {
                isInside = false
            });
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HasPropObject())
        {
            // the player is holding an object
            GetPropObject().SetPropObjectParent(null);
        } // the player is not holding an object
        else if (targetedProp)
        {
            // there is a prop under the players mouse
            targetedProp.SetPropObjectParent(this);
        }
    }

    public void SetTargetedPropObject(PropObject prop)
    {
        targetedProp = prop;
    }
}
