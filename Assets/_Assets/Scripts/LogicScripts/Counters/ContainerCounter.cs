using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // player has an object

            if (player.GetKitchenObject().GetKitchenObjectSO() == kitchenObjectSO)
            {
                Debug.Log("putting item back");
                // player's object is same type as container counter's objects

                player.GetKitchenObject().GetKitchenObjectVisual().Hide();
                player.GetKitchenObject().SetKitchenObjectParent(this);
                OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // player's object is different from container counter's object type


            }
        }
        else
        {
            // player does not have object
            if (HasKitchenObject())
            {
                Debug.Log("Picking Item up");
                // the counter is holding an object
                GetKitchenObject().SetKitchenObjectParent(player);
                player.GetKitchenObject().GetKitchenObjectVisual().Show();
            }
            else
            {
                // the counter top is not holding an object
                Debug.Log("spawining new object");
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            }
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
