using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderChunks : MonoBehaviour
{

    PlayerMovement player;
    Collider myCollider;


    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        myCollider = GetComponent<SphereCollider>();
    }


    void Update()
    {
        checkPlayerLocation();
        renderChunks();
    }

    private void checkPlayerLocation()
    {
        //print(player.transform.position);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }

    private void renderChunks()
    {
        Collider[] findChunks = Physics.OverlapSphere(player.transform.position, 10f);
        if (findChunks.Length > 0)
        {
            for (int i = 0; i < findChunks.Length; i++)
            {
                //print(findChunks[i].gameObject.name);

                if (findChunks[i].gameObject.GetComponent<Chunk>())
                {
                    //print(findChunks[i].gameObject.GetComponent<Chunk>().name);
                    Transform thisChunk = findChunks[i].gameObject.transform;
                    foreach (Transform child in thisChunk)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

}
