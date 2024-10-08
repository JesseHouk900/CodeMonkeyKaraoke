using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inductee1BallObstacleTarget : MonoBehaviour
{
    public event EventHandler OnEventHappen;
    [SerializeField] private BallObstacleBall ballObstacleBall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BallObstacleBall>() == ballObstacleBall)
        {
            // the colliding object is the ball
            ballObstacleBall.GetConstantForce().relativeTorque = new Vector3(0, 0, 0);
            Debug.Log("Inductee's rejoice!");
            OnEventHappen?.Invoke(this, EventArgs.Empty);
        }
    }
}
