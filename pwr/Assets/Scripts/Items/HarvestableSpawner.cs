using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableSpawner : MonoBehaviour
{
    public GameObject mushroomGameObject;
    public WorldController worldController;

    private float randomX;
    private float randomY;
    private const int mushroomSpawnNumber = 5;
    private GameObject spawnedObject;

    void Start()
    {
        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>();
    }

    private void Update()
    {
        if(worldController.shouldSpawnMushrooms)
        {
            SpawnNewMushrooms(mushroomSpawnNumber);
            worldController.shouldSpawnMushrooms = false;
        }
    }

    public void SpawnNewMushrooms(int amountToSpawn)
    {
        for(int i = 0; i < amountToSpawn; i++)
        {
            randomX = Random.Range(this.gameObject.transform.position.x - (this.gameObject.transform.localScale.x / 2),
                this.gameObject.transform.position.x + (this.gameObject.transform.localScale.x / 2));
            randomY = Random.Range(this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2),
                this.gameObject.transform.position.y + (this.gameObject.transform.localScale.y / 2));

            spawnedObject = Instantiate(mushroomGameObject, new Vector3(randomX, randomY, 0.0f), Quaternion.identity);
            worldController.harvestableList.Add(spawnedObject);
            spawnedObject.transform.parent = worldController.transform;

        }
    }

    public void RespawnExistingMushrooms()
    {
        foreach(GameObject harvestable in worldController.harvestableList)
        {
            Instantiate(harvestable, harvestable.transform.position, Quaternion.identity);
        }

    }

}
