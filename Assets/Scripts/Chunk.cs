using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    PlayerMovement player;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        disableChunk();
    }

    private void disableChunk()
    {
        Transform playerTransform = player.transform;
        float playerDistance = Vector3.Distance(playerTransform.position, transform.position);

        if (playerDistance > 40f)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
