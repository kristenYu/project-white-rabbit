using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlanting
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestCropSpawnsCorrectly()
    {
        GameObject worldControllerObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/WorldController"));
        WorldController worldController = worldControllerObject.GetComponent<WorldController>();

        GameObject testCropObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Crops/carrot_crop"));
        Crop testCrop = testCropObject.GetComponent<Crop>();
        Assert.AreEqual(worldController.currentDay, testCrop.startingDay);
        Assert.AreEqual(Crop.CropStage.Sprout, testCrop.currentStage);


    }

    [UnityTest]
    public IEnumerator TestCropGrowing()
    {
        /*
        GameObject worldControllerObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/WorldController"));
        WorldController worldController = worldControllerObject.GetComponent<WorldController>();
        worldController.setDurationsForTesting(0.1f, 0.1f, 0.1f);

        GameObject testCropObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Crops/carrot_crop"));
        Crop testCrop = testCropObject.GetComponent<Crop>();
        worldController.activeCropList.Add(testCropObject);

        //test growing with time
        Assert.AreEqual(Crop.CropStage.Sprout, testCrop.currentStage);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(Crop.CropStage.Sprout, testCrop.currentStage);
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(Crop.CropStage.SmallPlant, testCrop.currentStage);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(Crop.CropStage.LargePlant, testCrop.currentStage);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(Crop.CropStage.FullyGrown, testCrop.currentStage);
        */

        yield return null; 

    }
}
