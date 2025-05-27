using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card/New Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite cardSprite;
    public Sprite cardBackground;
    
    public int cardValue;
    public string cardSuit;

    // Sets the value of the card
    public void SetCardData(string cardName, Sprite cardSprite, Sprite cardBackground)
    {
        this.cardName = cardName;
        this.cardSprite = cardSprite;
        this.cardBackground = cardBackground;
    }
    
    // Clones the card into a new card
    public CardData Clone()
    {
        var clone = ScriptableObject.CreateInstance<CardData>();
        clone.cardName = this.cardName;
        clone.cardSprite = this.cardSprite;
        return clone;
    }

}