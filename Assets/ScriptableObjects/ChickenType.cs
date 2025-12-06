using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChickenTypesNames{
    Ross,
    Cobb
}

[CreateAssetMenu(fileName ="New Chicken type", menuName ="ScriptableObjects/Chicken Type", order = 1)]
public class ChickenType : ScriptableObject
{
    [Header("Nombre del tipo")]
    public string typeName;

}
