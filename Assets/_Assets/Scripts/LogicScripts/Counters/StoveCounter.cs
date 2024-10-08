using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burnt
    };

    private State state;
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;


    private void Start()
    {
        state = State.Idle;
    }
    private void Update()
    {
        switch (state)
        {
            case State.Idle:

                break;
            case State.Frying:
                fryingTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = (float)fryingTimer / fryingRecipeSO.fryingTimeMax
                });
                if (fryingTimer > fryingRecipeSO.fryingTimeMax)
                {
                    // Fried
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                    state = State.Fried;
                    fryingTimer = 0f;
                    burningRecipeSO = GetBurningRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });
                }
                break;
            case State.Fried:

                burningTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = (float)burningTimer / burningRecipeSO.burningTimeMax
                });

                if (burningTimer > burningRecipeSO.burningTimeMax)
                {
                    // burning
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                    state = State.Burnt;
                    burningTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });


                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                }

                break;
            case State.Burnt:

                break;
        }
    }

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
                
                // Give the object to the player
                GetKitchenObject().SetKitchenObjectParent(player);

                // Set the state back to idle
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
                fryingTimer = 0f;
                burningTimer = 0f;
            }
        }
        else
        {
            // the counter is empty
            if (player.HasKitchenObject())
            {
                // player has object
                if (HasFryingRecipeForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // The player has an object that matches a recipe
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                }
            }
            else
            {
                // the player does not have an object
            }
        }
    }

    private bool HasFryingRecipeForInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOFromInput(kitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return true;
        }
        return false;
    }
    private FryingRecipeSO GetFryingRecipeSOFromInput(KitchenObjectSO kitchenObjectSO)
    {
        Debug.Log(fryingRecipeSOArray);
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == kitchenObjectSO)
            {
                Debug.Log(fryingRecipeSO.ToString());
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private bool HasBurningRecipeForInput(KitchenObjectSO kitchenObjectSO)
    {
        BurningRecipeSO burningRecipeSO = GetBurningRecipeSOFromInput(kitchenObjectSO);
        if (burningRecipeSO != null)
        {
            return true;
        }
        return false;
    }

    private BurningRecipeSO GetBurningRecipeSOFromInput(KitchenObjectSO kitchenObjectSO)
    {
        Debug.Log(burningRecipeSOArray);
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            Debug.Log($@"burningRecipeSO: {burningRecipeSO}
                        kitchenObjectSO: {kitchenObjectSO}");
            if (burningRecipeSO.input == kitchenObjectSO)
            {
                Debug.Log(burningRecipeSO.ToString());
                return burningRecipeSO;
            }
        }
        return null;
    }
}
