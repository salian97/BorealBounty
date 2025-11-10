using UnityEditor;
using UnityEngine;
using brainflow;

[CustomEditor(typeof(EEGDataAnalyzer))]
public class EEGDataAnalyzerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Limit the displayed BoardIds to just two choices
        SerializedProperty boardProp = serializedObject.FindProperty("board_id");
        SerializedProperty serialProp = serializedObject.FindProperty("serial_port");

        int[] boardValues = new int[] { (int)BoardIds.SYNTHETIC_BOARD, (int)BoardIds.NEUROPAWN_KNIGHT_BOARD };
        string[] labels = new string[] { "SYNTHETIC_BOARD", "NEUROPAWN_KNIGHT_BOARD" };

        int currentValue = boardProp.intValue;
        int currentIndex = 0;
        for (int i = 0; i < boardValues.Length; ++i)
        {
            if (boardValues[i] == currentValue)
            {
                currentIndex = i;
                break;
            }
        }

        int selectedIndex = EditorGUILayout.Popup(new GUIContent("Board Id", "Select BrainFlow board (limited set)"), currentIndex, labels);
        boardProp.intValue = boardValues[selectedIndex];

        // show serial_port only when NeuroPawn is selected
        if (boardProp.intValue == (int)BoardIds.NEUROPAWN_KNIGHT_BOARD)
        {
            EditorGUILayout.PropertyField(serialProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
