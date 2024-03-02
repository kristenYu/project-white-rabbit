using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableSpawner : MonoBehaviour
{
    public enum harvestable
    {
        mushrooom = 0,
        berry,
    }
    
    public GameObject mushroomGameObject;
    public WorldController worldController;

    private float randomX;
    private float randomY;
    private const int harvestableSpawnNumber = 3;
    private GameObject spawnedObject;

    public harvestable currentHarvestable; 
    void Start()
    {
        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>();
    }

    private void Update()
    {
        switch(currentHarvestable)
        {
            case harvestable.mushrooom:
                if (worldController.mushroomHarvestableList.Count >= WorldController.maxMushroomSpawn)
                {
                    worldController.shouldSpawnHarvestables = false;
                }
                if (worldController.shouldSpawnHarvestables)
                {
                    if (WorldController.maxMushroomSpawn - worldController.mushroomHarvestableList.Count > harvestableSpawnNumber)
                    {
                        SpawnNewHarvestable(harvestableSpawnNumber);
                    }
                    else
                    {
                        SpawnNewHarvestable(WorldController.maxMushroomSpawn - worldController.mushroomHarvestableList.Count);
                    }
                }
                break;
            case harvestable.berry:
                if (worldController.mushroomHarvestableList.Count >= WorldController.maxBerrySpawn)
                {
                    worldController.shouldSpawnHarvestables = false;
                }
                if (worldController.shouldSpawnHarvestables)
                {
                    if (WorldController.maxBerrySpawn - worldController.berryHarvestableList.Count > harvestableSpawnNumber)
                    {
                        SpawnNewHarvestable(harvestableSpawnNumber);
                    }
                    else
                    {
                        SpawnNewHarvestable(WorldController.maxMushroomSpawn - worldController.berryHarvestableList.Count);
                    }
                }
                break;

        }
       
    }

    public void SpawnNewHarvestable(int amountToSpawn)
    {
        for(int i = 0; i < amountToSpawn; i++)
        {
            randomX = Random.Range(this.gameObject.transform.position.x - (this.gameObject.transform.localScale.x / 2),
                this.gameObject.transform.position.x + (this.gameObject.transform.localScale.x / 2));
            randomY = Random.Range(this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2),
                this.gameObject.transform.position.y + (this.gameObject.transform.localScale.y / 2));

            spawnedObject = Instantiate(mushroomGameObject, new Vector3(randomX, randomY, 0.0f), Quaternion.identity);
            worldController.mushroomHarvestableList.Add(spawnedObject);
            spawnedObject.transform.parent = worldController.transform;

        }
    }

    public void RespawnExistingHarvestable()
    {
        foreach(GameObject harvestable in worldController.mushroomHarvestableList)
        {
            Instantiate(harvestable, harvestable.transform.position, Quaternion.identity);
        }
        foreach (GameObject harvestable in worldController.berryHarvestableList)
        {
            Instantiate(harvestable, harvestable.transform.position, Quaternion.identity);
        }

    }

}
