using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObjectVisual : MonoBehaviour
{
    [SerializeField] private IDesiredObjectParent desireObjectParent;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private bool isShowing;

    private void Start()
    {
        isShowing = false;
    }
    public void Show()
    {
        Debug.Log("Showing");
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        Debug.Log("Hiding");
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }

    public void SetOnDesiredObjectShowHide(TraderNPC traderNPC)
    {
        traderNPC.OnDesiredObjectShowHide += TraderNPC_OnDesiredObjectShowHide;
    }

    private void TraderNPC_OnDesiredObjectShowHide(object sender, TraderNPC.OnDesiredObjectShowHideEventArgs e)
    {
        if (isShowing)
        {
            // the object is showing
            Hide();
            isShowing = false;
        }
        else
        {
            // the object is hidden
            Show();
            isShowing = true;
        }
    }
}
