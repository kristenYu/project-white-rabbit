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
    
    public GameObject gameObjectToSpawn;
    public WorldController worldController;

    private float randomX;
    private float randomY;
    public const int harvestableSpawnNumber = 3;
    private GameObject spawnedObject;

    public harvestable currentHarvestable;
    private void Awake()
    {
        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>();
    }

    public void SpawnNewHarvestable(int amountToSpawn, List<GameObject> spawnedObjectList)
    {
        for(int i = 0; i < amountToSpawn; i++)
        {
            randomX = Random.Range(this.gameObject.transform.position.x - (this.gameObject.transform.localScale.x / 2),
                this.gameObject.transform.position.x + (this.gameObject.transform.localScale.x / 2));
            randomY = Random.Range(this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2),
                this.gameObject.transform.position.y + (this.gameObject.transform.localScale.y / 2));

            spawnedObject = Instantiate(gameObjectToSpawn, new Vector3(randomX, randomY, 0.0f), Quaternion.identity);
            spawnedObject.transform.parent = worldController.transform;
            spawnedObjectList.Add(spawnedObject);
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
