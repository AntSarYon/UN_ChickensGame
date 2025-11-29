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

    [Header("Preferencias de comida")]
    public List<Ingredient> likedIngredients;
    public List<Ingredient> dislikedIngredients;

    [Header("Likes temporales")]
    public bool likeHarina;
    public bool likeSoja;
    public bool likeGusanos;
    public bool likeMaiz;

    [Header("Desc. Gustos")]
    public string gustos;

    [Header("Desc. Pasiva")]
    public string pasiva;

    [Header("Le gustan los juguetes?")]
    public bool likeToys;
}
