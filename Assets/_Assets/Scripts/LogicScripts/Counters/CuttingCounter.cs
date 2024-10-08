using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;
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
                if (HasKitchenRecipeForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                    
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingInteractionsMax
                    }
                    );
                }
            }
            else
            {
                // the player does not have an object
            }
        }
    }

    public override void AltInteract(Player player)
    {
        if (HasKitchenObject() && HasKitchenRecipeForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingInteractionsMax
            }
            );

            if (cuttingProgress >= cuttingRecipeSO.cuttingInteractionsMax)
            {
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(cuttingRecipeSO.output, this);
            }
            else
            {

            }

        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        return null;

    }

    private bool HasKitchenRecipeForInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(kitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return true;
        }
        return false;
    }
    private CuttingRecipeSO GetCuttingRecipeSOFromInput(KitchenObjectSO kitchenObjectSO)
    {
        Debug.Log(cuttingRecipeSOArray);
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            Debug.Log(cuttingRecipeSO.ToString());
            if (cuttingRecipeSO.input == kitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
