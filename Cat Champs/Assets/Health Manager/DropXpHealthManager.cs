using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropXpHealthManager : HealthManager
{
    public GameObject[] xpDrops;

    protected override void HealthDepletedInternal()
    {
        // Drop XP by instantiating all the XP drops
        foreach (GameObject xpDrop in xpDrops)
        {
            // Randomize the position of the drop
            Vector3 dropPosition = transform.position;
            dropPosition.x += Random.Range(-1f, 1f);
            dropPosition.y += Random.Range(-1f, 1f);
            Instantiate(xpDrop, dropPosition, Quaternion.identity);
        }
        
    }
}
