using System;
using System.Collections;
using System.Collections.Generic;
using ProceduralPlanets;
using UniRx;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {

    Planet planet;
    Editor shapeEditor;
    private Animator animator;
    Editor colourEditor;
    private static readonly int Attack = Animator.StringToHash("Attack");

    public override void OnInspectorGUI()
	{
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => {
            animator.SetTrigger(Attack);
        });
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            ClearChildren();
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor);
	}

    private void ClearChildren()
    {
        var p = target as Planet;
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
        planet = (Planet)target;
	}
}
