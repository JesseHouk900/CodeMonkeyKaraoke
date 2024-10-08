using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPropObjectParent
{
    public Transform GetPropObjectFollowTransform();

    public PropObject GetPropObject();

    public void SetPropObject(PropObject propObject);

    public void ClearPropObject();

    public bool HasPropObject();
}
