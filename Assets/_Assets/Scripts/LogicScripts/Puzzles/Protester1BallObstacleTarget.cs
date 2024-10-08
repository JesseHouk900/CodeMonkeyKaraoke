using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protester1BallObstacleTarget : MonoBehaviour
{
    [SerializeField] private BallObstacleBall ballObstacleBall;

    public void OnMouseDown()
    {
        // do a raycast to find collider
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BallObstacleBall>() == ballObstacleBall)
        {
            // the colliding object is the ball
            ballObstacleBall.GetConstantForce().relativeTorque = new Vector3(0, 0, 0);
            Debug.Log("Protester's rejoice!");
        }
    }
}
