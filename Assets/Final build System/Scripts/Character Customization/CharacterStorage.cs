using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

/*
    this is a storage class to store all the selected variations  the player chose in the start sceen,
    its made to be scalable with the number of variations  we have ex. 10 arms customizations , 20 legs customizations
    whatever we want.

    this holder class holds a list of catigories.
 */
[CreateAssetMenu(fileName = "Character Storage", menuName = "Storage/Character Storage", order = 1)]
public class CharacterStorage : ScriptableObject
{
    public List<CustomizationsStorage> customizations;
    public Gender gender;
}



/*
   Holder class to hold all the options we want in one catigory
 */
[System.Serializable]
public struct CustomizationsStorage
{
    public Option option;
    public List<bool> selectedOption;
}