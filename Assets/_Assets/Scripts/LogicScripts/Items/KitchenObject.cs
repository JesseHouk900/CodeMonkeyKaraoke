using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour, IShowHide
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private KitchenObjectVisual kitchenObjectVisual;

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public IKitchenObjectParent GetIKitchenObjectParent()
    {
        return kitchenObjectParent; 
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }
        this.kitchenObjectParent = kitchenObjectParent;
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("IKitchenObjectParent already has kitchen object!");
        }
        else
        {
            kitchenObjectParent.SetKitchenObject(this);

            transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

        }
    }
    
    public KitchenObjectVisual GetKitchenObjectVisual()
    {
        return kitchenObjectVisual; 
    }

    public void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // the player is holding an object
            if (player.GetKitchenObject() == this)
            {
                float forwardThrowPower = 5f;
                float upwardThrowPower = 5f;
                Rigidbody kitchenRB = player.GetKitchenObject().GetComponent<Rigidbody>();
                kitchenRB.isKinematic = false;
                kitchenRB.AddForce(transform.forward * forwardThrowPower + transform.up * upwardThrowPower, ForceMode.Impulse);
                //GetKitchenObject().SetKitchenObjectParent(null);
                player.GetKitchenObject().transform.parent = null;
                player.ClearKitchenObject();
            }
        }
        else
        {
            // the player is not holding an object
            SetKitchenObjectParent(player);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(rb.velocity * collision.relativeVelocity.magnitude);
        }
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * collision.relativeVelocity.magnitude, Color.white);
            //Debug.Log($@"thisCollider: {contact.thisCollider.transform.parent.gameObject}
            //             otherCollider: {contact.otherCollider.transform.parent.gameObject}");

        }

        //// Play a sound if the colliding objects had a big impact.
        //if (collision.relativeVelocity.magnitude > 2)
            
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();

        Destroy(gameObject);
    }


    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        kitchenObject.GetKitchenObjectVisual().Show();

        return kitchenObject;
    }

}
