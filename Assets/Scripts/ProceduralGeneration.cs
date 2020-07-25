using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int depth;
    [SerializeField] float heightValue, smoothnessHeight;
    [SerializeField] int minStoneHeight, maxStoneHeight;
    [SerializeField] GameObject dirt, grass, stone, diamond, ruby, amethyst;
    [Range(0, 100)]
    [SerializeField] int seed;
    public GameObject player;
    public LookAtPlayer myCam;
    private bool worldIsDestroyed = true;

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

    // Mineral Fields
    private int diamondStarterInt = 0;
    [Range(0, 10)]
    public int spreadDiamondsMultiplier = 3;
    [Range(0, 10)]
    public int diamondRarity = 0;

    private int rubyStarterInt = 0;
    [Range(0, 10)]
    public int spreadRubysMultiplier = 3;
    [Range(0, 10)]
    public int rubyRarity = 0;

    private int amethystStarterInt = 0;
    [Range(0, 10)]
    public int spreadAmethystsMultiplier = 3;
    [Range(0, 10)]
    public int amethystRarity = 0;

    private int dirtStarterInt = 0;
    [Range(0, 10)]
    public int spreadDirtMultiplier = 3;
    [Range(0, 10)]
    public int dirtRarity = 0;

    private bool dirtComplete = false;
    private bool rubyComplete = false;
    private bool amethystComplete = false;
    private bool diamondComplete = false;


    private void Awake()
    {
        seed = Random.Range(0, 1000000);
    }

    private void Start()
    {

    }


    private void Update()
    {
        spreadMinerals();
    }

    public void DestroyWorld()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
        if (player)
        {
            Destroy(player.gameObject);
        }

        worldIsDestroyed = true;
    }

    public void GenerateWorld()
    {
        if (worldIsDestroyed == true)
        {
            Instantiate(player, new Vector3(0, 25, 25), Quaternion.identity);
            player.transform.position = new Vector3(0, 27, 15);
            dirtStarterInt = 0;
            diamondStarterInt = 0;
            amethystStarterInt = 0;
            rubyStarterInt = 0;
            GenerateCaves();
            Generation();
        
            myCam = FindObjectOfType<LookAtPlayer>();
            myCam.lockCameraToPlayer();
        }
        worldIsDestroyed = false;
    }

    private void spreadMinerals()
    {
        if (dirtStarterInt < spreadDirtMultiplier)
        {
            GenGems(dirt, "Dirt(Clone)", dirtRarity);
            dirtStarterInt++;
        }
        else
        {
            dirtComplete = true;
        }

        if (amethystStarterInt < spreadAmethystsMultiplier && dirtComplete == true)
        {
            GenGems(amethyst, "Amethyst(Clone)", amethystRarity);
            amethystStarterInt++;
        }
        else if (dirtComplete == false)
        {
            return;
        }
        else 
        {
            amethystComplete = true;
        }

        if (rubyStarterInt < spreadRubysMultiplier && amethystComplete == true)
        {
            GenGems(ruby, "Ruby(Clone)", rubyRarity);
            rubyStarterInt++;
        }
        else if (amethystComplete == false)
        {
            return;
        }
        else
        {
            rubyComplete = true;
        }
        
        if (diamondStarterInt < spreadDiamondsMultiplier && rubyComplete == true)
        {
            GenGems(diamond, "Diamond(Clone)", diamondRarity);
            diamondStarterInt++;
        }
        else if (rubyComplete == false)
        {
            return;
        }
        else
        {
            diamondComplete = true;
        }
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


            GenHeight(z, height, totalStoneSpawnDistance);
            GenDepth(z);
            GenTopLevel(z, height, totalStoneSpawnDistance);

        }
    }

    private void GenTopLevel(int z, int height, int totalStoneSpawnDistance)
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

    private void GenDepth(int z)
    {

        for (int y = 0; y < depth; y++)
        {
            int mineralGen = Random.Range(0, 101);
            if (cavePoints[z, y] == 1 && mineralGen == 1 && y > 70)
            {
                spawnObj(diamond, -y, z);
            }
            else if (cavePoints[z, y] == 1 && mineralGen == 2 && y > 40)
            {
                spawnObj(ruby, -y, z);
            }
            else if (cavePoints[z, y] == 1 && mineralGen == 3 && y < 70)
            {
                spawnObj(amethyst, -y, z);
            }
            else if (cavePoints[z, y] == 1 && mineralGen > 90)
            {
                spawnObj(dirt, -y, z);
            }
            else if (cavePoints[z, y] == 1)
            {
                spawnObj(stone, -y, z);
            }
        }

    }

    private void GenGems(GameObject gem, string gemString, int gemRarity)
    {
        for (int z = 0; z < width; z++)
        {
            for (int y = 0; y < depth; y++)
            {
                Vector3 mineralLocation = new Vector3(0, -y, z);
                Collider[] getBlockType = Physics.OverlapSphere(mineralLocation, 0.1f);
                if (getBlockType.Length > 0)
                {
                    if (getBlockType[0].gameObject.name == gemString)
                    {
                        Vector3 gemLocation = getBlockType[0].gameObject.transform.position;
                        Collider[] gemIsTouching = Physics.OverlapSphere(gemLocation, .6f);
                        if (gemIsTouching.Length > 0)
                        {

                            for (int i = 0; i < gemIsTouching.Length; i++)
                            {
                                if (gemIsTouching[i].gameObject.name != gemString)
                                {
                                    bool shouldSpreadGem = false;
                                    if (Random.Range(0, gemRarity) == 1)
                                    {
                                        shouldSpreadGem = true;
                                    }

                                    if (shouldSpreadGem == true)
                                    {
                                        GameObject newGem = Instantiate(gem, gemIsTouching[i].transform.position, Quaternion.identity);
                                        newGem.transform.parent = this.transform;
                                        Destroy(gemIsTouching[i].gameObject);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void GenHeight(int z, int height, int totalStoneSpawnDistance)
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
