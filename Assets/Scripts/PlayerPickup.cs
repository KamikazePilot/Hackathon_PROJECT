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
        if (Camera.main == null)
        {
            Debug.LogError("No MainCamera found.");
            return;
        }

        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint is not assigned.");
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
                item = hit.collider.GetComponentInParent<PickupItem>();
            }

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
            else
            {
                Collider childCol = heldItem.GetComponentInChildren<Collider>();
                if (childCol != null)
                {
                    childCol.enabled = false;
                }
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
        else
        {
            Debug.Log("Raycast hit nothing.");
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

        Collider col = itemToDrop.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        else
        {
            Collider childCol = itemToDrop.GetComponentInChildren<Collider>();
            if (childCol != null)
            {
                childCol.enabled = true;
            }
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
}