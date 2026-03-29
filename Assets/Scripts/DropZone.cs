using UnityEngine;

public class DropZone : MonoBehaviour
{
    public ItemType acceptedItemType;

    private PlayerActionLogger logger;

    void Start()
    {
        logger = FindFirstObjectByType<PlayerActionLogger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PickupItem item = other.GetComponent<PickupItem>();

        if (item == null)
        {
            item = other.GetComponentInParent<PickupItem>();
        }

        if (item == null) return;

        if (item.itemType == acceptedItemType)
        {
            Debug.Log(item.itemType + " successfully dropped at " + gameObject.name);

            if (logger != null)
            {
                logger.LogDeposit(item.itemType.ToString(), gameObject.name);
            }

            Destroy(item.gameObject);
        }
        else
        {
            Debug.Log("Wrong item type dropped at " + gameObject.name);
        }
    }
}