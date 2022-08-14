using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DeckCard
{
    public string name;
    public string category;
    public string description;
    public string iconName;
    public Color color;
}
[Serializable]
public struct DeckCardSerializable
{
    public string name;    
    public string category;
    public string description;
    public string iconName;
    public float r,g,b;
}

public class Category
{
    private string categoryName;
    private List<DeckCard> cards;

    public Category()
    {
        categoryName = "default";
        cards = new List<DeckCard>();
    }
    public Category(string newName)
    {
        categoryName = newName;
        cards = new List<DeckCard>();
    }

    public string GetCategoryName()
    {
        return categoryName;
    }

    public List<DeckCard> GetCategoryCards()
    {
        return cards;
    }
}

public class Deck : MonoBehaviour
{
    private List<DeckCard> deck;
    private List<Category> categories;
    void Start()
    {
        UpdateDeck("DefaultDeck.json");
        //CheckCategories();
    }

    public void UpdateDeck(string deckName)
    {
        deck = new List<DeckCard>();
        deck = FileHandler.ReadListFromJSON<DeckCard>(deckName);

        categories = new List<Category>();
        SplitCategories(deck);
    }

    public void SplitCategories(List<DeckCard> deck)
    {
        foreach(DeckCard card in deck) 
        {
            Category currCat = categories.Find(x=>x.GetCategoryName() == card.category);
            if(currCat != null)currCat.GetCategoryCards().Add(card);
            else
            {
                currCat = new Category(card.category);
                currCat.GetCategoryCards().Add(card);
                categories.Add(currCat);
            }
        }
    }
    public void CheckCategories()
    {
        int min = -1;
        string minName = "";
        Debug.Log("Всего категорий в колоде: " + categories.Count);
        foreach(Category cat in categories)
        {
            Debug.Log(cat.GetCategoryName() + ", карт: " + cat.GetCategoryCards().Count);
            if(cat.GetCategoryCards().Count<min||min==-1) 
            {
                min = cat.GetCategoryCards().Count; 
                minName = cat.GetCategoryName();
            };
        }
         Debug.Log("Наименьшая численность в категории: "+minName + " ("+ min +" карт)");
    }

    public List<Category> GetCategories()
    {
        return categories;
    }

    public List<DeckCard> GetDeck()
    {
        return deck;
    }
}
