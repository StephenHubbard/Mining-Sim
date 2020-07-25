using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int depth;
    [SerializeField] float heightValue, smoothnessHeight;
    [SerializeField] int minStoneHeight, maxStoneHeight;
    [SerializeField] GameObject dirt, grass, stone, diamond, ruby;
    [Range(0, 100)]
    [SerializeField] int seed;
    
    // Cave Variables
    [SerializeField] int smoothCycles;
    private int[,] cavePoints;
    private int[,] mineralPoints;
    // randFillPercent 55 is recommended
    [Range(0, 100)]
    public int randFillPercent;
    // threshold 4 is recommended
    [Range(0, 8)]
    public int threshold;

    private void Awake()
    {
        seed = Random.Range(0, 1000000);
        GenerateCaves();
    }

    private void Start()
    {
        Generation();
    }

    private void GenerateCaves()
    {
        cavePoints = new int[width, depth];

        System.Random randChoice = new System.Random(seed.GetHashCode());

        for (int z = 0; z < width; z++)
        {
            for (int y = 0; y < depth; y++)
            {
                if (z == 0 || y == 0 || z == width - 1 || y == depth - 1)
                {
                    cavePoints[z, y] = 1;
                }
                else if (randChoice.Next(0, 100) < randFillPercent)
                {
                    cavePoints[z, y] = 1;
                }
                else
                {
                    cavePoints[z, y] = 0;
                }
            }
        }

        for (int i = 0; i < smoothCycles; i++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int y = 0; y < depth; y++)
                {
                    int neighboringWalls = GetNeighbors(z, y);

                    if (neighboringWalls > threshold)
                    {
                        cavePoints[z, y] = 1;
                    }
                    else if (neighboringWalls < threshold)
                    {
                        cavePoints[z, y] = 0;
                    }
                }
            }
        }
    }


    private int GetNeighbors(int pointZ, int pointY)
    {
        int wallNeighbors = 0;

        for (int z = pointZ - 1; z <= pointZ + 1; z++)
        {
            for (int y = pointY - 1; y <= pointY + 1; y++)
            {
                if (z >= 0 && z < width && y >= 0 && y < depth)
                {
                    if (z != pointZ || y != pointY)
                    {
                        if (cavePoints[z, y] == 1)
                        {
                            wallNeighbors++;
                        }
                    }
                }
                else
                {
                    wallNeighbors++;
                }
            }
        }

        return wallNeighbors;
    }

    private void Generation()
    {
        for (int z = 0; z < width; z++)
        {
            int height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise(z / smoothnessHeight, seed));
            int minStoneSpawnDistance = height - minStoneHeight;
            int maxStoneSPawnDistance = height - maxStoneHeight;
            int totalStoneSpawnDistance = Random.Range(minStoneSpawnDistance, maxStoneSPawnDistance);

            System.Random randChoice = new System.Random(seed.GetHashCode());


            genHeight(z, height, totalStoneSpawnDistance);
            genDepth(z);
            genTopLevel(z, height, totalStoneSpawnDistance);
            StartCoroutine(ExecuteAfterTime(1f, z));

        }
    }

    private void genTopLevel(int z, int height, int totalStoneSpawnDistance)
    {
        if (cavePoints[z, height] == 1)
        {
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

    private IEnumerator ExecuteAfterTime(float time, int z)
    {
        yield return new WaitForSeconds(time);

        genMinerals(z);
    }

    private void genDepth(int z)
    {

        for (int y = 0; y < depth; y++)
        {
            int mineralGen = Random.Range(1, 101);
            if (cavePoints[z, y] == 1 && mineralGen == 100)
            {
                spawnObj(diamond, -y, z);
            }
            else if (cavePoints[z, y] == 1 && mineralGen == 99)
            {
                spawnObj(ruby, -y, z);
            }
            else if (cavePoints[z, y] == 1)
            {
                spawnObj(stone, -y, z);
            }
        }

    }

    private void genMinerals(int z)
    {
        int spreadDiamondsMultiplier = 0;

        for (int y = 0; y < depth; y++)
        {
            Vector3 mineralLocation = new Vector3(0, -y, z);
            Collider[] getBlockType = Physics.OverlapSphere(mineralLocation, 0.1f);
            if (getBlockType.Length > 0)
            {
                if (getBlockType[0].gameObject.GetComponent<Diamond>())
                {
                    Vector3 diamondLocation = getBlockType[0].gameObject.transform.position;
                    Collider[] diamondIsTouching = Physics.OverlapSphere(diamondLocation, .6f);
                    if (diamondIsTouching.Length > 0)
                    {
                        
                        for (int i = 0; i < diamondIsTouching.Length; i++)
                        {
                            if (!diamondIsTouching[i].gameObject.GetComponent<Diamond>())
                            {
                                GameObject newDiamond = Instantiate(diamond, diamondIsTouching[i].transform.position, Quaternion.identity);
                                newDiamond.transform.parent = this.transform;
                                Destroy(diamondIsTouching[i].gameObject);
                                
                            }
                        }
                    }
                }
            }
        }

        //spreadDiamondsMultiplier++;
        //if (spreadDiamondsMultiplier < 3)
        //{
        //    genMinerals(z);
        //}
    }

    private void genHeight(int z, int height, int totalStoneSpawnDistance)
    {
        for (int y = 0; y < height; y++)
        {
            if (cavePoints[z, y] == 1)
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
        }
    }

    // Spawn inside parent game object
    private void spawnObj (GameObject obj, int y, int z)
    {
        obj = Instantiate(obj, new Vector3(0, y, z), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
}
