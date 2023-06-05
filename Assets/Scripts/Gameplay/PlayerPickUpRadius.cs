using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPickUpRadius : MonoBehaviour
{
    private List<BombMesh> _itemsInPickupRange;
    
    void Start()
    {
        _itemsInPickupRange = new List<BombMesh>();
    }

    public bool CanPickUpSomething()
    {
        return _itemsInPickupRange.Any();
    }

    public Bomb GetItemForPickup()
    {
        // TODO: Could find the physically closest bomb in the list here and use that instead.
        return _itemsInPickupRange.First().GetBomb();
    }

    private void OnTriggerEnter(Collider c)
    {
        var bomb = c.GetComponent<BombMesh>();
        if (bomb != null)
        {
            _itemsInPickupRange.Add(bomb);
        }
    }

    private void OnTriggerExit(Collider c)
    {
        var bomb = c.GetComponent<BombMesh>();
        if (bomb != null)
        {
            _itemsInPickupRange.Remove(bomb);
        }
    }
}
