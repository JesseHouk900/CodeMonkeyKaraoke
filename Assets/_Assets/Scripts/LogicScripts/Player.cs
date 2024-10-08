using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour, IKitchenObjectParent, IPropObjectParent, IShowHide
{
    public static Player Instance { get; private set; }

    // Event handler for when selected counter is changed
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    // Event handler for when selected kitchen object is changed
    public event EventHandler<OnSelectedKitchenObjectChangedEventArgs> OnSelectedKitchenObjectChanged;
    public class OnSelectedKitchenObjectChangedEventArgs : EventArgs
    {
        public KitchenObject selectedKitchenObject;
    }

    // Event handler for when selected TraderNPC is changed
    public event EventHandler<OnSelectedTraderNPCChangedEventArgs> OnSelectedTraderNPCChanged;
    public class OnSelectedTraderNPCChangedEventArgs : EventArgs 
    {
        public TraderNPC selectedTraderNPC;
    }

    [SerializeField] private float movementSpeed;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private Transform[] propObjectHoldPointArray;
    [SerializeField] private int propObjectHoldPointIndex;
    [SerializeField] private float jumpPower;
    [SerializeField] private PlayerVisual playerVisual;

    private Rigidbody rb;
    private bool isWalking;
    private bool isJumpPressed;
    private bool isOnGround;
    private Vector3 movementVector;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private KitchenObject selectedKitchenObject;
    private TraderNPC selectedTraderNPC;
    private float interactCooldownTime;
    private float lastInteractTime;
    private PropObject propObject;
    private PropObject selectedPropObject;
    private BallObstacle selectedBallObstacle;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: More than 1 player detected");
        }
        Instance = this;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnJumpAction += GameInput_OnJumpAction;
        gameInput.OnAltInteractAction += GameInput_OnAltInteractAction;
        isJumpPressed = false;
        isOnGround = true;
        interactCooldownTime = 0.3f;
        lastInteractTime = Time.time;
    }


    private void GameInput_OnJumpAction(object sender, System.EventArgs e)
    {
        if (isOnGround && !isJumpPressed)
        {
            isJumpPressed = true;
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (GetTimeSinceLastInteraction() > interactCooldownTime)
        {
            Debug.Log("Interacting");
            // interact cooldown expired
            if (selectedCounter != null)
            {
                // there is a counter to interact with
                Debug.Log("interacting with counter");
                selectedCounter.Interact(this);
            }
            else if (HasSelectedKitchenObject())
            {
                // there is a kitchen object selected by the player
                Debug.Log("Interacting with selected kitchen object");
                GetSelectedKitchenObject().Interact(this);
            }
            else if (HasSelectedTraderNPC())
            {
                // there is a TraderNPC selected by the player
                Debug.Log("Selected Trader interacted with");
                GetSelectedTraderNPC().Interact(this);
            }
            else if (HasSelectedBallObstacle())
            {
                // there is a ball obstacle selected by the player
                Debug.Log("Interacting with ball obstacle puzzle");
                GetSelectedBallObstacle().Interact(this);
                
            }
            else if (HasSelectedPropObject())
            {
                // there is a prop object selected by the player
                Debug.Log("Interacting with selected prop");
                GetSelectedPropObject().Interact(this);
            }
            else
            {
                // there is nothing to interact with
                if (HasKitchenObject())
                {
                    // the player is holding a kitchen object 
                    Debug.Log("interacting with held kitchen object");
                    GetKitchenObject().Interact(this);
                }
                if (HasPropObject())
                {
                    // the player is holding a prop object 
                    Debug.Log("Interacting with the held prop object");
                    GetPropObject().Interact(this);

                }
                else
                {
                    // the player is not holding an object
                }
            }
        }
        lastInteractTime = Time.time;
    }
    private void GameInput_OnAltInteractAction(object sender, EventArgs e)
    {
        if (GetTimeSinceLastInteraction() > interactCooldownTime)
        {
            // the interaction cooldown timer has expired
            if (selectedCounter != null)
            {
                selectedCounter.AltInteract(this);
            }
            else if (selectedTraderNPC != null)
            {
                Debug.Log("Selected Trader alt interacted with");
                StartCoroutine(selectedTraderNPC.AltInteract(this));
            }
            
            lastInteractTime = Time.time;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameInput.IsPlayerInControl())
        {
            HandleMovement();
            HandleInteractions();
        }

    }
    // All physics updates should be made in the FixedUpdate
    private void FixedUpdate()
    {
        transform.position += (movementVector);
        //if (Time.deltaTime > 0)
        //{

            if (isOnGround)
            {
                if (isJumpPressed)
                {
                    isOnGround = false;
                    rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                    isJumpPressed = false;
                }
            }
        //}
        //rb.MovePosition(rb.position + movementVector * Time.deltaTime);
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovement()
    {
        movementVector = GetMovementVector();
        if (!isOnGround && Physics.Raycast(transform.position, Vector3.down, out var hit))
        {
            if (hit.distance > 50.0f)
            {
                // You are falling or at least 50 units above something.
            }
            else if (hit.distance > 0.5f)
            {
                // You are falling or at least 0.5 units above something.
            }
            else
            {
                // You are not above 0.5 units above something.

                isOnGround = true;
                Debug.Log("on ground");
            }
        }
        else
        {
            // The player is jumping

        }

        float rotateSpeed = 10f;

        transform.forward = Vector3.Slerp(transform.forward, movementVector, rotateSpeed * Time.deltaTime);
    }

    private Vector3 GetMovementVector()
    {
        Vector3 inputVector = gameInput.GetNormalizedInputVector();
        float playerHeight = 2f;
        float playerRadius = .7f;
        float moveDistance = Time.deltaTime * movementSpeed;
        movementVector = inputVector;
        isWalking = movementVector != Vector3.zero;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, movementVector, moveDistance, countersLayerMask);
        if (!canMove)
        {
            // Cannot move in given direction

            // Try moving on the x axis
            Vector3 movementVectorX = new Vector3(movementVector.x, 0, 0).normalized;
            canMove = movementVector.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, movementVectorX, moveDistance, countersLayerMask);
            if (canMove)
            {
                // Can move on X axis, so set movement vector to only have x component
                movementVector = movementVectorX;
            }
            else
            {
                // Cannot move on x axis

                // Attempt to move on z axis
                Vector3 movementVectorZ = new Vector3(0, 0, movementVector.z).normalized;
                canMove = movementVector.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, movementVectorZ, moveDistance, countersLayerMask);
                if (canMove)
                {
                    // Can move on Z axis
                    movementVector = movementVectorZ;
                }
                else
                {
                    // Cannot move in X-Z plane

                    // Set movement vector to 0
                    movementVector = Vector3.zero;
                }
            }
        }
        return movementVector * moveDistance;
    }

    private void HandleInteractions()
    {

        Vector3 inputVector = gameInput.GetNormalizedInputVector();

        if (inputVector != Vector3.zero)
        {
            lastInteractDirection = inputVector;
        }
        float interactDistance = 2f;

        float playerHeight = 2f;
        float playerRadius = .7f;
        float moveDistance = Time.deltaTime * movementSpeed;
        if (Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, lastInteractDirection, out RaycastHit raycastHit, interactDistance))
        {
            //Debug.Log($"raycastHit: {raycastHit.transform}");
            // raycastHit is object in interactDistance of player
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // raycastHit has a counter component
                if (selectedCounter != baseCounter)
                {
                    SetSelectedCounter(baseCounter);
                    SetSelectedKitchenObject(null);
                    SetSelectedTraderNPC(null);
                    SetSelectedPropObject(null);
                    SetSelectedBallObstacle(null);

                }
            } // raycastHit has no counter component
            else if (raycastHit.transform.TryGetComponent(out KitchenObject kitchenObject))
            {
                // raycastHit has KitchenObject component
                if (!HasKitchenObject())
                {
                    SetSelectedKitchenObject(kitchenObject);
                    SetSelectedCounter(null);
                    SetSelectedTraderNPC(null);
                    SetSelectedPropObject(null);
                    SetSelectedBallObstacle(null);

                }
            } // raycastHit has no kitchen object component
            else if (raycastHit.transform.TryGetComponent(out TraderNPC traderNPC))
            {
                // raycastHit has TraderNPC component
                if (!HasSelectedTraderNPC())
                {
                    SetSelectedTraderNPC(traderNPC);
                    SetSelectedCounter(null);
                    SetSelectedKitchenObject(null);
                    SetSelectedPropObject(null);
                    SetSelectedBallObstacle(null);

                }

            } // raycastHit has no Trader npc component
            else if (raycastHit.transform.TryGetComponent(out PropObject propObject))
            {
                // raycastHit has PropObject component
                if (!HasSelectedPropObject())
                {
                    SetSelectedPropObject(propObject);
                    SetSelectedCounter(null);
                    SetSelectedKitchenObject(null);
                    SetSelectedTraderNPC(null);
                    SetSelectedBallObstacle(null);
                }
            } // raycastHit has no prop object component
            else if (raycastHit.transform.TryGetComponent(out BallObstacle ballObstacle))
            {
                if (!HasSelectedBallObstacle())
                {
                    Debug.Log("Ball obstacle hit");
                    // raycastHit has BallObstacle component
                    SetSelectedBallObstacle(ballObstacle);
                    SetSelectedPropObject(null);
                    SetSelectedCounter(null);
                    SetSelectedKitchenObject(null);
                    SetSelectedTraderNPC(null);

                }
            } // raycastHit does not have Ball obstacle component
            else
            {
                // default for not recognizing/having interaction with raycastHit
                SetSelectedCounter(null);
                SetSelectedKitchenObject(null);
                SetSelectedTraderNPC(null);
                SetSelectedPropObject(null);
                SetSelectedBallObstacle(null);

            }
        }
        else
        {
            SetSelectedCounter(null);
            SetSelectedKitchenObject(null);
            SetSelectedTraderNPC(null);
            SetSelectedPropObject(null);
            SetSelectedBallObstacle(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        }
        );
    }

    private void SetSelectedPropObject(PropObject selectedPropObject)
    {
        this.selectedPropObject = selectedPropObject;
    }

    private void SetSelectedBallObstacle(BallObstacle selectedBallObstacle)
    {
        this.selectedBallObstacle = selectedBallObstacle;
    }


    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        this.kitchenObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public KitchenObject GetSelectedKitchenObject()
    {
        return selectedKitchenObject;
    }

    public void SetSelectedKitchenObject(KitchenObject kitchenObject)
    {
        selectedKitchenObject = kitchenObject;

        OnSelectedKitchenObjectChanged?.Invoke(this, new OnSelectedKitchenObjectChangedEventArgs
        {
            selectedKitchenObject = selectedKitchenObject
        }
        );
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public bool HasSelectedKitchenObject()
    {
        return selectedKitchenObject != null;
    }

    public TraderNPC GetSelectedTraderNPC()
    {
        return selectedTraderNPC;
    }

    public void SetSelectedTraderNPC(TraderNPC selectedTraderNPC)
    {
        this.selectedTraderNPC = selectedTraderNPC;

        OnSelectedTraderNPCChanged?.Invoke(this, new OnSelectedTraderNPCChangedEventArgs
            {
                selectedTraderNPC = selectedTraderNPC
            }
        );
    }
    public bool HasSelectedTraderNPC()
    {
        return selectedTraderNPC != null;
    }

    public bool HasPropObject()
    {
        return propObject != null;
    }

    public void ModifyMovementSpeed(float speedModifier)
    {
        movementSpeed += speedModifier;
    }

    private float GetTimeSinceLastInteraction()
    {
        return Time.time - lastInteractTime;
    }

    public Transform GetPropObjectFollowTransform()
    {
        return propObjectHoldPointArray[propObjectHoldPointIndex];
    }

    public PropObject GetPropObject()
    {
        return propObject;
    }

    public void SetPropObject(PropObject propObject)
    {
        this.propObject = propObject;
        this.propObject.GetComponent<Rigidbody>().isKinematic = true;
        // OnSelectedPropObjectChanged?.Invoke({ });

    }

    public void ClearPropObject()
    {
        propObject = null;
    }

    private bool HasSelectedPropObject()
    {
        return selectedPropObject != null;
    }

    private PropObject GetSelectedPropObject()
    {
        return selectedPropObject;
    }

    private bool HasSelectedBallObstacle()
    {
        return selectedBallObstacle != null;
    }

    private BallObstacle GetSelectedBallObstacle()
    {
        return selectedBallObstacle;
    }

    public GameInput GetGameInput()
    {
        return gameInput;
    }

    public PlayerVisual GetPlayerVisual()
    {
        return playerVisual;
    }
}
