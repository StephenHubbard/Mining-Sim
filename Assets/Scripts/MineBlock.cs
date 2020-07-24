using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MineBlock : MonoBehaviour
{

    public AudioSource miningHitSFX;
    private bool colliderInBlock = false;
    private GameObject currentBlock = null;

    private void Update()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        currentBlock = other.gameObject;
        colliderInBlock = true;
    }

    private void OnTriggerExit(Collider other)
    {
        colliderInBlock = false;
    }

    private void destroyBlock()
    {
        if (Input.GetMouseButton(0) && colliderInBlock)
        {
            HitBlock();
        }
    }

    private void HitBlock()
    {
        miningHitSFX.Play();
        Destroy(currentBlock);
        colliderInBlock = false;
    }

    public void checkIfMineBlockExists()
    {
        destroyBlock();
    }

}
