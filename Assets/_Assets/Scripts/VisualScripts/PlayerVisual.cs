using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour, IShowHide
{
    [SerializeField] private GameObject[] visualGameObjectArray;
    [SerializeField] private bool isShowing;

    public void Show()
    {
        Debug.Log("Showing");
        foreach (GameObject visualGameObject in visualGameObjectArray)

        {
            visualGameObject.SetActive(true);
        }
        isShowing = true;

    }

    public void Hide()
    {
        Debug.Log("Hide");
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
        isShowing = false;

    }

    public bool IsVisible()
    {
        return isShowing;
    }

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
