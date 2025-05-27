using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeckManager))]
public class DeckManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        var cardManager = (DeckManager)target;

        // Add a button below the default inspector
        if (GUILayout.Button("Shuffle Deck"))
        {
            cardManager.ShuffleDeck();
        }
    }
}