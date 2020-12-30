using System;
using UnityEngine;

public class RigidBodyStopper : MonoBehaviour
{
    Rigidbody2D rigidBody;
    
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(rigidBody.velocity != Vector2.zero) rigidBody.velocity = Vector2.zero;
    }
}