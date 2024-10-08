using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            // there is an item on the counter
            if (player.HasKitchenObject())
            {
                // the player has an item

            }
            else
            {
                // the player does not have an item
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        else
        {
            // the counter is empty
            if (player.HasKitchenObject())
            {
                // player has object
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // the player does not have an object
            }
        }
    }

}