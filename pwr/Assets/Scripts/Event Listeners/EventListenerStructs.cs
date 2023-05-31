using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventListenerStructs 
{
    //Events for when a game object meets a specific value
    [System.Serializable]
    public struct IntValueStruct
    {
        public GameObject targetObject;
        public int startingValue;
        public int currentValue;
        public int targetValue; 
    }

    [System.Serializable]
    public struct FloatValueStruct
    {
        public GameObject targetObject;
        public float startingValue;
        public float currentValue;
        public float targetValue;
    }

    //struct to check when a seed is planted
    [System.Serializable]
    public struct PlantSeedStruct
    {
        public string name;
        public int targetValue; 
    }

    [System.Serializable]
    public struct CookingStruct
    {
        public string ingredientType; 
        public int targetValue; 
    }

    [System.Serializable]
    public struct HarvestStruct
    {
        public string name;
        public int targetValue; 
    }

    public struct PlaceStruct
    {
        public int targetValue;
    }
}
