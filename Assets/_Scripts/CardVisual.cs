using UnityEngine;

public class CardVisual : MonoBehaviour
{
    public CardData cardData;
    public bool isFaceDown;

    public void Reveal()
    {
        if (!isFaceDown) return;

        var SR = GetComponent<SpriteRenderer>();
        if (SR != null && cardData != null)
        {
            SR.sprite = cardData.cardSprite;
            isFaceDown = false;
        }
    }
}