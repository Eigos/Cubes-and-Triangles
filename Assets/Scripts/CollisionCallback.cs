using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCallback : MonoBehaviour
{
    public delegate void OnCollision(GameObject thisObj, GameObject collisionObj);
    public OnCollision CollisionFunction = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CollisionFunction != null)
            CollisionFunction(gameObject, collision.gameObject);


    }
}
