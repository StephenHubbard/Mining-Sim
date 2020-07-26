using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderChunks : MonoBehaviour
{
    public Collider[] mySphereCollider;

    private bool collideToggle = false;

    void Start()
    {
    }

    public void toggleCollide()
    {
        collideToggle = !collideToggle;
        print(collideToggle);
    }

    void Update()
    {
        testCollide();
    }

    private void testCollide()
    {
        
    }
}
