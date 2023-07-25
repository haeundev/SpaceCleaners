using System;
using System.Collections;
using System.Collections.Generic;
using ProceduralPlanets;
using UniRx;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetEditor : Editor {

    PlanetGenerator _planetGenerator;
    Editor shapeEditor;
    private Animator animator;
    Editor colourEditor;
    
    public override void OnInspectorGUI()
	{
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                _planetGenerator.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            ClearChildren();
            _planetGenerator.GeneratePlanet();
        }

        DrawSettingsEditor(_planetGenerator.shapeSettings, _planetGenerator.OnShapeSettingsUpdated, ref _planetGenerator.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(_planetGenerator.colourSettings, _planetGenerator.OnColourSettingsUpdated, ref _planetGenerator.colourSettingsFoldout, ref colourEditor);
	}

    private void ClearChildren()
    {
        var p = target as PlanetGenerator;
        for (var i = 0; i < p.gameObject.transform.childCount; i++)
            DestroyImmediate(p.gameObject.transform.GetChild(i).gameObject);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

	private void OnEnable()
	{
        _planetGenerator = (PlanetGenerator)target;
	}
}
