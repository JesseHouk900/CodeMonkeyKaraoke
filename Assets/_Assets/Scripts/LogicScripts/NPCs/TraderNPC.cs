using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderNPC : MonoBehaviour, IKitchenObjectParent, IDesiredObjectParent
{

    // Event handeler for when TraderNPC is interacted with
    public event EventHandler<OnDesiredObjectShowHideEventArgs> OnDesiredObjectShowHide;

    public class OnDesiredObjectShowHideEventArgs : EventArgs
    {
        public KitchenObject kitchenObject;
        public bool? isVisible;
    }

    [SerializeField] private Transform traderNPC;
    [SerializeField] private KitchenObject kitchenObject;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private Transform desiredKitchenObjectRequestPoint;
    [SerializeField] private KitchenObject desiredKitchenObject;
    [SerializeField] private KitchenObjectVisual desiredObjectVisual;
    [SerializeField] private KitchenObject rewardKitchenObject;

    private void Start()
    {
        SetDesiredObject(desiredKitchenObject);

        //OnDesiredObjectShowHide?.Invoke(this, new OnDesiredObjectShowHideEventArgs
        //{
        //    kitchenObject = desiredKitchenObject
        //}
        //);
    }

    public void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            // npc is holding object
            if (!player.HasKitchenObject())
            {
                // player is not holding object
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        else
        {
            // npc is not holding object
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
    }

    public IEnumerator AltInteract(Player player) 
    {
        if (!HasDesireSatisfied())
        {
            Debug.Log("npc desire unsatisfied");
            // npc has not had its desire satisfied
            if (!player.HasKitchenObject())
            {
                Debug.Log("player does not have object");
                // player is not holding object
                //SetDesiredObject(desiredKitchenObject);

                OnDesiredObjectShowHide?.Invoke(this, new OnDesiredObjectShowHideEventArgs
                {
                    kitchenObject = desiredKitchenObject
                }
                );
                yield return new WaitForSeconds(3);

                OnDesiredObjectShowHide?.Invoke(this, new OnDesiredObjectShowHideEventArgs
                {
                    kitchenObject = desiredKitchenObject
                });

            }
            else
            {

                Debug.Log("player is holding object");
                // player is holding object
                Debug.Log($"{player.GetKitchenObject().GetKitchenObjectSO()} == {desiredKitchenObject.GetKitchenObjectSO()}");
                if (player.GetKitchenObject().GetKitchenObjectSO() == desiredKitchenObject.GetKitchenObjectSO())
                {
                    // player has same object as desiredObject
                    Debug.Log("player has same object as npc desires");
                    AcceptDesiredObjectFromPlayer(player);
                }
            }
        }
        else
        {
            // npc has desired object
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        yield return 0;
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        this.kitchenObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    //public KitchenObject GetDesiredObject();
    public KitchenObject GetDesiredObject()
    {
        return desiredKitchenObject; 
    }

    //public void SetDesiredObject(KitchenObject kitchenObject);
    public void SetDesiredObject(KitchenObject kitchenObject)
    {
        this.desiredKitchenObject = kitchenObject;
        Debug.Log("Setting desired object show hide event handler");
        desiredObjectVisual.SetOnDesiredObjectShowHide(this);
    }

    //public bool HasDesiredObject();
    public bool HasDesiredObject()
    {
        return desiredKitchenObject != null;
    }

    //public bool HasDesireSatisfied();
    public bool HasDesireSatisfied()
    {
        if (desiredKitchenObject != null && desiredKitchenObject == kitchenObject)
        {
            // if holding desired object
            return true;
        }
        return false;

    }

    //public void ClearDesiredObject();
    public void ClearDesiredObject()
    {
        desiredKitchenObject = null;
    }

    //public Transform GetDesiredObjectFollowTransform();
    public Transform GetDesiredObjectFollowTransform()
    {
        return desiredKitchenObjectRequestPoint;
    }

    public KitchenObject GetRewardKitchenObject()
    {
        return rewardKitchenObject;
    }

    public void AcceptDesiredObjectFromPlayer(Player player)
    {
        //SetKitchenObject(player.GetKitchenObject());
        player.GetKitchenObject().SetKitchenObjectParent(this);
        KitchenObject.SpawnKitchenObject(rewardKitchenObject.GetKitchenObjectSO(), player);
        //rewardKitchenObjectTransform.GetComponent<KitchenObjectVisual>().MakeInvisible();
        Debug.Log("Quest Complete");
    }

}
