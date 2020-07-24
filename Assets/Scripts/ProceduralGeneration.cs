using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] int minStoneHeight, maxStoneHeight;
    [SerializeField] GameObject dirt, grass, stone;

    private void Start()
    {
        Generation();
    }

    private void Generation()
    {
        for (int z = 0; z < width; z++)
        {
            int minHeight = height - 1;
            int maxHeight = height + 2;

            height = Random.Range(minHeight, maxHeight);
            int minStoneSpawnDistance = height - minStoneHeight;
            int maxStoneSPawnDistance = height - maxStoneHeight;
            int totalStoneSpawnDistance = Random.Range(minStoneSpawnDistance, maxStoneSPawnDistance);

            for (int y = 0; y < height; y++)
            {
                if (y < totalStoneSpawnDistance)
                {
                    spawnObj(stone, y, z);
                }
                else
                {
                   spawnObj(dirt, y, z);
                }
            }
            if (totalStoneSpawnDistance == height)
            {
                spawnObj(stone, height, z);
            }
            else
            {
                spawnObj(grass, height, z);
            }
        }
    }

    private void spawnObj (GameObject obj, int width, int height)
    {
        obj = Instantiate(obj, new Vector3(0, width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
}
