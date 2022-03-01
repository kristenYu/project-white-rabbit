using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using UnityEngine.UI;

public class TestPlanting
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestPlantingSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestCropGrowing()
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

        GameObject testCropObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Crops/carrot_crop"));
        Crop testCrop = testCropObject.GetComponent<Crop>();
        testCrop.worldController = worldController; 
        worldController.activeCropList.Add(testCropObject);

        yield return null;
        worldController.setDurationsForTesting(0.1f, 0.1f, 0.1f);

        //test growing with time
        Assert.AreEqual(Crop.CropStage.Sprout, testCrop.currentStage);
        yield return new WaitForSecondsRealtime(0.1f);
        Assert.AreEqual(Crop.CropStage.Sprout, testCrop.currentStage);
        yield return new WaitForSecondsRealtime(0.3f);
        Assert.True(testCrop.isReadyToGrow);
        Assert.AreEqual(Crop.CropStage.SmallPlant, testCrop.currentStage);
        //yield return new WaitForSecondsRealtime(0.3f);
        //Assert.AreEqual(Crop.CropStage.LargePlant, testCrop.currentStage);
        //yield return new WaitForSecondsRealtime(0.3f);
        //Assert.AreEqual(Crop.CropStage.FullyGrown, testCrop.currentStage);


        Object.Destroy(testCrop);

    }
   
}
