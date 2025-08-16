using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Categories", menuName = "Customization/Categories Holder", order = 1)]
public partial class Categories : ScriptableObject
{

    
    [SerializeField] List<Item> items = new List<Item>();
    
    [System.Serializable]
    public struct Item
    {
        public Option categoryName;
        public Sprite categorySprite;
    }


    public List<Item> GetItems() { return items; }

}
