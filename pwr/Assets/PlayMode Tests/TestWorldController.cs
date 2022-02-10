using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using UnityEngine.UI;
public class TestWorldController
{
    [UnityTest]
    public IEnumerator TestDaysChanging()
    {
        GameObject testObject1 = new GameObject();
        GameObject testObject2 = new GameObject();
        testObject1.AddComponent<TextMeshProUGUI>();
        testObject2.AddComponent<RawImage>();
        TextMeshProUGUI testGUI = testObject1.GetComponent<TextMeshProUGUI>();
        RawImage testImage = testObject2.GetComponent<RawImage>();
        GameObject worldControllerObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/WorldController"));
        WorldController worldController = worldControllerObject.GetComponent<WorldController>();
        worldController.TODText = testGUI;
        worldController.TODImage = testImage;
        //START HAS NOT BEEN CALLED YET 

        yield return null; //wait one frame for start to be called
        //START IS CALLED 
        worldController.setDurationsForTesting(0.1f, 0.1f, 0.1f);
        Assert.AreEqual(1, worldController.currentDay);
        Assert.AreEqual(WorldController.TOD.Day, worldController.currentTOD);

        yield return new WaitForSecondsRealtime(0.1f);
        Assert.AreEqual(WorldController.TOD.Twilight, worldController.currentTOD);
        yield return new WaitForSecondsRealtime(0.1f);
        Assert.AreEqual(WorldController.TOD.Night, worldController.currentTOD);
        yield return new WaitForSecondsRealtime(0.1f);
        Assert.AreEqual(WorldController.TOD.Day, worldController.currentTOD);
        Assert.AreEqual(2, worldController.currentDay);


        Object.Destroy(testObject1);
        Object.Destroy(testObject2);
        Object.Destroy(worldControllerObject);
    }

    
}
