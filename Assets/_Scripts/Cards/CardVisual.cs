using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer frontOfCard;
    [SerializeField] private SpriteRenderer backOfCard;
    [SerializeField] private Animator animator;

    [Header("Sprites")]
    [SerializeField] private CardSpriteEntry[] cardSprites;

    public void SetFrontOfCard(CardInstance card)
    {
        frontOfCard.sprite = GetSprite(card.suit, card.rank);
        SetFaceDown(card.faceDown);
    }

    public void SetBackOfCard(Sprite backOfCardSprite)
    {
        backOfCard.sprite = backOfCardSprite;
    }

    public void SetFaceDown(bool faceDown)
    {
        frontOfCard.gameObject.SetActive(!faceDown);
    }

    private Sprite GetSprite(CardSuit suit, CardRank rank)
    {
        foreach (var entry in cardSprites)
        {
            if (entry.suit == suit && entry.rank == rank)
                return entry.sprite;
        }

        Debug.LogError($"Missing sprite for {rank} of {suit}");
        return null;
    }

    public void SetSortingOrder(int order)
    {
        frontOfCard.sortingOrder = order;
        backOfCard.sortingOrder = order;
    }

    public void FallDown()
    {
        animator.SetTrigger("FallDown");
    }
}