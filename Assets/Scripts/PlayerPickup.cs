using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 3f;

    private PickupItem heldItem;
    private PlayerActionLogger logger;

    void Start()
    {
        logger = FindFirstObjectByType<PlayerActionLogger>();
    }

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
        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint is not assigned.");
            return;
        }

        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, pickupRange);

        PickupItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in nearbyColliders)
        {
            PickupItem item = col.GetComponent<PickupItem>();

            if (item == null)
                item = col.GetComponentInParent<PickupItem>();

            if (item == null)
                continue;

            float dist = Vector3.Distance(transform.position, item.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestItem = item;
            }
        }

        if (closestItem == null)
        {
            Debug.Log("No pickup item in range.");
            return;
        }

        heldItem = closestItem;

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider[] cols = heldItem.GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = false;
        }

        heldItem.transform.SetParent(holdPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        Debug.Log("Picked up: " + heldItem.name);

        if (logger != null)
        {
            logger.LogPickup(heldItem.itemType.ToString(), heldItem.name);
        }
    }

    void DropItem()
    {
        if (heldItem == null) return;

        PickupItem itemToDrop = heldItem;
        heldItem = null;

        itemToDrop.transform.SetParent(null);

        Vector3 dropPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
        itemToDrop.transform.position = dropPosition;

        Collider[] cols = itemToDrop.GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = true;
        }

        Rigidbody rb = itemToDrop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Dropped: " + itemToDrop.name);

        if (logger != null)
        {
            logger.LogDrop(itemToDrop.itemType.ToString(), itemToDrop.name);
        }
    }

    public string GetHeldItemType()
    {
        if (heldItem == null)
            return "None";

        return heldItem.itemType.ToString();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}