using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 3f;

    private PickupItem heldItem;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame && heldItem == null)
        {
            Debug.Log("Tried to pick up.");
            TryPickUp();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame && heldItem != null)
        {
            Debug.Log("Dropped item.");
            DropItem();
        }
    }

    void TryPickUp()
    {
        if (Camera.main == null)
        {
            Debug.LogError("No MainCamera found.");
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);

            PickupItem item = hit.collider.GetComponent<PickupItem>();
            if (item == null)
            {
                Debug.Log("Hit object is not a pickup item.");
                return;
            }

            heldItem = item;

            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Collider col = heldItem.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            heldItem.transform.SetParent(holdPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;

            Debug.Log("Picked up: " + heldItem.name);
        }
        else
        {
            Debug.Log("Raycast hit nothing.");
        }
    }

    void DropItem()
    {
        if (heldItem == null) return;

        heldItem.transform.SetParent(null);
        heldItem.transform.position = holdPoint.position + Camera.main.transform.forward * 1f;

        Collider col = heldItem.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        heldItem = null;
    }
}