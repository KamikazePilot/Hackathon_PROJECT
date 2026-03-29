using UnityEngine;

public class DropZone : MonoBehaviour
{
    public ItemType acceptedItemType;

    private void OnTriggerEnter(Collider other)
    {
        PickupItem item = other.GetComponent<PickupItem>();

        if (item != null)
        {
            if (item.itemType == acceptedItemType)
            {
                Debug.Log(item.itemType + " successfully dropped at " + gameObject.name);
            }
            else
            {
                Debug.Log("Wrong item type dropped at " + gameObject.name);
            }
        }
    }
}