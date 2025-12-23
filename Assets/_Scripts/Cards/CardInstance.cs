[System.Serializable]
public struct CardInstance
{
    public CardSuit suit;
    public CardRank rank;
    public int value;
    public bool faceDown;
}

public enum CardSuit
{
    Spades,
    Hearts,
    Diamonds,
    Clubs
}

public enum CardRank
{
    Ace = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}