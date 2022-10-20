using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerManager))]
public class PlayerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlayerManager manager = (PlayerManager)target;

        if (GUILayout.Button("Update Input"))
        {
            manager.UpdateInput();
        }
    }
}
