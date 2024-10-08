using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallObstacleBall : PropObject
{

    [SerializeField] private BallObstacleBallVisual ballObstacleBallVisual;
    [SerializeField] private Transform StartPropObjectHoldPoint;
    private int spawnTimer;
    private int endTimer;
    private int currentTimer;

    private void Start()
    {
        spawnTimer = 1000;
        endTimer = 10000;
        Reset();
    }

    private void Update()
    {
        currentTimer++;
        //Debug.Log($"currentTime: {currentTimer}");
        if (currentTimer > endTimer)
        {
            Debug.Log("Boom");
        }
        else if (currentTimer > spawnTimer)
        {
            // the timer for spawning has elapsed
            if (!GetBallObstacleBallVisual().IsVisible())
            {
                // the ball is not visible
                GetBallObstacleBallVisual().Show();
                GetRigidbody().isKinematic = false;
                float relativeZTorque = 1f;
                GetConstantForce().relativeTorque = new Vector3(0, 0, relativeZTorque);
            }
        }
        else
        {
            // 

        }
    }

    public BallObstacleBallVisual GetBallObstacleBallVisual()
    {
        return ballObstacleBallVisual;
    }

    public Rigidbody GetRigidbody()
    {
        return GetComponent<Rigidbody>();
    }

    public ConstantForce GetConstantForce()
    {
        return GetComponent<ConstantForce>();
    }

    public void Reset()
    {
        currentTimer = 0;
        GetBallObstacleBallVisual().Hide();
        GetComponent<Rigidbody>().isKinematic = true;
        transform.localPosition = StartPropObjectHoldPoint.localPosition;
        transform.localRotation = Quaternion.identity;
    }

}
