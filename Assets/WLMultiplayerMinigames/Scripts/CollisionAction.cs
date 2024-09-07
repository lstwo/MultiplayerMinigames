using ModWobblyLife;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionAction : MonoBehaviour
{
    public string tagMask;

    [Space(5)]

    public UnityEvent<Collision> onCollisionEnter;
    public UnityEvent<Collider> onTriggerEnter;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == tagMask)
        {
            onCollisionEnter?.Invoke(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == tagMask)
        {
            onTriggerEnter?.Invoke(other);
        }
    }
}
