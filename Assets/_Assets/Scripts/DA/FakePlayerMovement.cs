using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private GameInput gameInput;
    private Rigidbody rb;
    private bool isWalking;
    private Vector3 movementVector;

    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        Debug.Log(rb);
        
        //rb = GetComponent<Rigidbody>();
    }

    public float collisionRadius = 0.8f;     //Adjust as needed
    private void Update()
    {
        movementVector = gameInput.GetNormalizedInputVector();
        // For animations
        isWalking = movementVector != Vector3.zero;


        float playerHeight = 2f;
        float playerRadius = 1f;
        float moveDistance = movementSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + transform.up * playerHeight, playerRadius, movementVector, moveDistance);
        // Collision Detection
        if (canMove)
        {
            transform.position += (movementVector * movementSpeed * Time.deltaTime);
            
            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, movementVector, rotateSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("Collision detected!");
            //Handle collision response here (e.g., stop movement, play a sound, etc.)
            movementVector = Vector3.zero;

        }

    }
    // All physics updates should be made in the FixedUpdate
    private void FixedUpdate()
    {
        rb.velocity = movementVector;
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
