using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropObject : MonoBehaviour
{
    [SerializeField] private PropObjectSO propObjectSO;

    private List<Transform> propObjectCollidedWithTransformList;
    private IPropObjectParent propObjectParent;

    private void Awake()
    {
        propObjectCollidedWithTransformList = new List<Transform>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Smack");
        propObjectCollidedWithTransformList.Add(other.transform);

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("un-Smack");
        propObjectCollidedWithTransformList.Remove(other.transform);

    }
    public PropObjectSO GetPropObjectSO()
    {
        return propObjectSO;
    }
    public IPropObjectParent GetIPropObjectParent()
    {
        return propObjectParent;
    }

    public void SetPropObjectParent(IPropObjectParent propObjectParent)
    {
        if (this.propObjectParent != null)
        {
            this.propObjectParent.ClearPropObject();
        }
        this.propObjectParent = propObjectParent;
        if (propObjectParent != null)
        {

            if (propObjectParent.HasPropObject())
            {
                Debug.LogError("IPropObjectParent already has prop object");
            }
            else
            {
                propObjectParent.SetPropObject(this);

                transform.parent = propObjectParent.GetPropObjectFollowTransform();
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
            }
        }
        //else
        //{
        //    Debug.LogError("Error: propObjectParent is null");
        //}
    }

    public void Interact(Player player)
    {
        if (player.HasPropObject())
        {
            // player is holding an object
            if (player.GetPropObject() == this)
            {
                // this is the object that the player is holding
                float forwardThrowPower = 5f;
                float upwardThrowPower = 5f;
                Rigidbody propRB = player.GetPropObject().GetComponent<Rigidbody>();
                propRB.isKinematic = false;
                propRB.AddForce(transform.forward * forwardThrowPower + transform.up * upwardThrowPower, ForceMode.Impulse);
                //GetPropObject().SetPropObjectParent(null);
                player.GetPropObject().transform.parent = null;
                player.ClearPropObject();
            }
        }
        else
        {
            // player is not holding an object
            bool interactedWith = false;
            if (propObjectCollidedWithTransformList.Count > 0)
            {
                // prop object is colliding with at least one object

                foreach (Transform colliderTransform in propObjectCollidedWithTransformList)
                {
                    Debug.Log($"colliderTransform: {colliderTransform.name}");
                    // loop through collided with collision transforms
                    if (colliderTransform.parent != null && colliderTransform.parent.gameObject.TryGetComponent(out BaseCounter baseCounter))
                    {
                        // the prop object is colliding with a BaseCounter object

                        Debug.Log("Interacting with propObject on a BaseCounter");
                        baseCounter.Interact(player);
                        interactedWith = true;
                    }
                }
            }
            if (!interactedWith)
            {
                // default interaction
                Debug.Log("Interacting with propObject");
                SetPropObjectParent(player);
                GetPropObjectSO().rigidbody.isKinematic = true;

            }
        }
    }

    public void DestroySelf()
    {
        propObjectParent.ClearPropObject();

        Destroy(gameObject);

    }


}
