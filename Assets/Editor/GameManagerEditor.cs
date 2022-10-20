using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameManager manager = (GameManager)target;

        if (GUILayout.Button("Win Game"))
        {
            manager.EndGame(true);
        }

        if (GUILayout.Button("Lose Game"))
        {
            manager.EndGame(false);
        }
    }
}
