using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedKitchenObjectVisual : MonoBehaviour
{

    [SerializeField] private KitchenObject kitchenObject;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private void Start()
    {
        Player.Instance.OnSelectedKitchenObjectChanged += Player_OnSelectedKitchenObjectChanged;
    }

    private void Player_OnSelectedKitchenObjectChanged(object sender, Player.OnSelectedKitchenObjectChangedEventArgs e)
    {
        if (kitchenObject == e.selectedKitchenObject)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            if (visualGameObject != null)
            {
                visualGameObject.SetActive(true);
            }
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            if (visualGameObject != null)
            {
                visualGameObject.SetActive(false);
            }
        }
    }
}
