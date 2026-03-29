using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 6f;

    private PickupItem heldItem;
    private CollectibleItem heldCollectible;
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

        Transform cam = Camera.main.transform;
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Ray ray = new Ray(origin, cam.forward);
        RaycastHit hit;

        Debug.DrawRay(origin, cam.forward * pickupRange, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);

            PickupItem item = hit.collider.GetComponent<PickupItem>();
            if (item == null)
                item = hit.collider.GetComponentInParent<PickupItem>();

            if (item == null)
            {
                Debug.Log("Hit object is not a pickup item.");
                return;
            }

            CollectibleItem collectible = hit.collider.GetComponent<CollectibleItem>();
            if (collectible == null)
                collectible = hit.collider.GetComponentInParent<CollectibleItem>();

            heldItem = item;
            heldCollectible = collectible;

            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb == null)
                rb = heldItem.GetComponentInChildren<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Collider col = heldItem.GetComponent<Collider>();
            if (col == null)
                col = heldItem.GetComponentInChildren<Collider>();

            if (col != null)
            {
                col.enabled = false;
            }

            heldItem.transform.SetParent(holdPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;

            Debug.Log("Picked up: " + heldItem.name);

            if (logger != null)
            {
                string typeToLog = heldItem.itemType.ToString();
                string idToLog = heldCollectible != null ? heldCollectible.itemId : heldItem.name;
                logger.LogPickup(typeToLog, idToLog);
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
        CollectibleItem collectibleToDrop = heldCollectible;

        heldItem = null;
        heldCollectible = null;

        itemToDrop.transform.SetParent(null);

        Vector3 dropPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
        itemToDrop.transform.position = dropPosition;

        Collider col = itemToDrop.GetComponent<Collider>();
        if (col == null)
            col = itemToDrop.GetComponentInChildren<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        Rigidbody rb = itemToDrop.GetComponent<Rigidbody>();
        if (rb == null)
            rb = itemToDrop.GetComponentInChildren<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Dropped: " + itemToDrop.name);

        if (logger != null)
        {
            string typeToLog = itemToDrop.itemType.ToString();
            string idToLog = collectibleToDrop != null ? collectibleToDrop.itemId : itemToDrop.name;
            logger.LogDrop(typeToLog, idToLog);
        }
    }

    public string GetHeldItemType()
    {
        if (heldItem == null)
            return "None";

        return heldItem.itemType.ToString();
    }
}