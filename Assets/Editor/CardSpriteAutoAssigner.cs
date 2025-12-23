using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public static class CardSpriteAutoAssigner
{
    private const string CARD_FOLDER = "Assets/Art/Cards/PNG-cards-1.3";

    [MenuItem("Tools/Cards/Populate CardVisual Sprites")]
    public static void PopulateSelectedCardVisual()
    {
        CardVisual visual = Selection.activeGameObject?.GetComponent<CardVisual>();

        if (visual == null)
        {
            Debug.LogError("Select a Card prefab with a CardVisual component.");
            return;
        }

        var entries = new List<CardSpriteEntry>();

        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { CARD_FOLDER });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string file = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();

            // Expected: "queen_of_spades"
            string[] parts = file.Split("_of_");
            if (parts.Length != 2)
                continue;

            if (!TryParseRank(parts[0], out CardRank rank))
                continue;

            if (!TryParseSuit(parts[1], out CardSuit suit))
                continue;

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
                continue;

            entries.Add(new CardSpriteEntry
            {
                rank = rank,
                suit = suit,
                sprite = sprite
            });
        }

        // 🔑 EXPLICIT, CORRECT CARD ORDER
        entries.Sort((a, b) =>
        {
            int suitCompare = SuitOrder(a.suit).CompareTo(SuitOrder(b.suit));
            if (suitCompare != 0)
                return suitCompare;

            return RankOrder(a.rank).CompareTo(RankOrder(b.rank));
        });

        SerializedObject so = new SerializedObject(visual);
        SerializedProperty prop = so.FindProperty("cardSprites");

        if (prop == null)
        {
            Debug.LogError("Field 'cardSprites' not found on CardVisual.");
            return;
        }

        prop.arraySize = entries.Count;

        for (int i = 0; i < entries.Count; i++)
        {
            SerializedProperty element = prop.GetArrayElementAtIndex(i);

            SetEnumByName(element.FindPropertyRelative("rank"), entries[i].rank);
            SetEnumByName(element.FindPropertyRelative("suit"), entries[i].suit);
            element.FindPropertyRelative("sprite").objectReferenceValue = entries[i].sprite;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(visual);

        Debug.Log($"Populated {entries.Count} card sprites " +
                  (entries.Count == 52 ? "✅ (correct)" : "⚠️ (expected 52)"));
    }

    // ---------- ORDERING ----------

    private static int SuitOrder(CardSuit suit)
    {
        return suit switch
        {
            CardSuit.Clubs => 0,
            CardSuit.Diamonds => 1,
            CardSuit.Hearts => 2,
            CardSuit.Spades => 3,
            _ => 99
        };
    }

    private static int RankOrder(CardRank rank)
    {
        return rank switch
        {
            CardRank.Ace => 1,
            CardRank.Two => 2,
            CardRank.Three => 3,
            CardRank.Four => 4,
            CardRank.Five => 5,
            CardRank.Six => 6,
            CardRank.Seven => 7,
            CardRank.Eight => 8,
            CardRank.Nine => 9,
            CardRank.Ten => 10,
            CardRank.Jack => 11,
            CardRank.Queen => 12,
            CardRank.King => 13,
            _ => 99
        };
    }

    // ---------- ENUM SAFETY ----------

    private static void SetEnumByName<T>(SerializedProperty prop, T value) where T : Enum
    {
        string name = value.ToString();
        for (int i = 0; i < prop.enumNames.Length; i++)
        {
            if (prop.enumNames[i] == name)
            {
                prop.enumValueIndex = i;
                return;
            }
        }
    }

    // ---------- PARSING ----------

    private static bool TryParseRank(string value, out CardRank rank)
    {
        switch (value)
        {
            case "ace": rank = CardRank.Ace; return true;
            case "king": rank = CardRank.King; return true;
            case "queen": rank = CardRank.Queen; return true;
            case "jack": rank = CardRank.Jack; return true;
            case "10": rank = CardRank.Ten; return true;
            case "9": rank = CardRank.Nine; return true;
            case "8": rank = CardRank.Eight; return true;
            case "7": rank = CardRank.Seven; return true;
            case "6": rank = CardRank.Six; return true;
            case "5": rank = CardRank.Five; return true;
            case "4": rank = CardRank.Four; return true;
            case "3": rank = CardRank.Three; return true;
            case "2": rank = CardRank.Two; return true;
        }

        rank = default;
        return false;
    }

    private static bool TryParseSuit(string value, out CardSuit suit)
    {
        switch (value)
        {
            case "clubs": suit = CardSuit.Clubs; return true;
            case "diamonds": suit = CardSuit.Diamonds; return true;
            case "hearts": suit = CardSuit.Hearts; return true;
            case "spades": suit = CardSuit.Spades; return true;
        }

        suit = default;
        return false;
    }
}
