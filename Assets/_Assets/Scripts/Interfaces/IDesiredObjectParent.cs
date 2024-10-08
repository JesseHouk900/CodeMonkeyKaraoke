using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDesiredObjectParent
{
    public Transform GetDesiredObjectFollowTransform();
    public KitchenObject GetDesiredObject();

    public void SetDesiredObject(KitchenObject kitchenObject);

    public void ClearDesiredObject();

    public bool HasDesiredObject();

    public bool HasDesireSatisfied();

    public void AcceptDesiredObjectFromPlayer(Player player);
}
