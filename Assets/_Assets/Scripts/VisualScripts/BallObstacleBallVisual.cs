using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallObstacleBallVisual : MonoBehaviour, IShowHide
{
    [SerializeField] private GameObject visualGameObject;

    [SerializeField] private bool isShowing;

    public void Show()
    {
        Debug.Log("Showing");
        visualGameObject.SetActive(true);
        isShowing = true;
    }

    public void Hide()
    {
        Debug.Log("Hide");
        visualGameObject.SetActive(false);
        isShowing = false;
    }

    public bool IsVisible()
    {
        return isShowing;
    }

    //private void SetOnShowHide(BallObstacleBall ballObstacleBall)
    //{
    //    ballObstacleBall.OnShowHide += BallObstacleBall_OnShowHide;
    //}

    private void BallObstacle_OnShowHide(object sender, EventArgs e)
    {
        if (isShowing)
        {
            // the object is showing
            Hide();
        }
        else
        {
            // the object is hidden
            Show();
        }
    }
}
