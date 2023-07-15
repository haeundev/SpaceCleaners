﻿using UnityEngine;

namespace ProceduralPlanets
{  
    public class Planet : MonoBehaviour
    {
        [Range(2, 256)] public int resolution = 10;
        public bool autoUpdate = true;

        public enum FaceRenderMask 
        {
            All,
            Top,
            Bottom,
            Left,
            Right,
            Front,
            Back
        }

        public FaceRenderMask faceRenderMask;

        public ShapeSettings shapeSettings;
        public ColourSettings colourSettings;

        [HideInInspector] public bool shapeSettingsFoldout;
        [HideInInspector] public bool colourSettingsFoldout;

        private readonly ShapeGenerator _shapeGenerator = new();
        private readonly ColourGenerator _colourGenerator = new();

        [SerializeField] [HideInInspector] private MeshFilter[] meshFilters;
        private TerrainFace[] _terrainFaces;


        private void Initialize()
        {
            _shapeGenerator.UpdateSettings(shapeSettings);
            _colourGenerator.UpdateSettings(colourSettings);

            if (meshFilters == null || meshFilters.Length == 0) meshFilters = new MeshFilter[6];
            _terrainFaces = new TerrainFace[6];

            Vector3[] directions =
                { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

            for (var i = 0; i < 6; i++)
            {
                if (meshFilters[i] == null)
                {
                    var meshObj = new GameObject("mesh");
                    meshObj.transform.parent = transform;

                    meshObj.AddComponent<MeshRenderer>();
                    meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    meshFilters[i].sharedMesh = new Mesh();
                }

                meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial =
                    new Material(colourSettings.planetMaterial);

                _terrainFaces[i] =
                    new TerrainFace(_shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
                var renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
                meshFilters[i].gameObject.SetActive(renderFace);
            }
        }

        public void GeneratePlanet()
        {
            Initialize();
            GenerateMesh();
            GenerateColours();
        }

        public void OnShapeSettingsUpdated()
        {
            if (autoUpdate)
            {
                Initialize();
                GenerateMesh();
            }
        }

        public void OnColourSettingsUpdated()
        {
            if (autoUpdate)
            {
                Initialize();
                GenerateColours();
            }
        }

        private void GenerateMesh()
        {
            for (var i = 0; i < 6; i++)
                if (meshFilters[i].gameObject.activeSelf)
                    _terrainFaces[i].ConstructMesh();

            _colourGenerator.UpdateElevation(_shapeGenerator.ElevationMinMax);
        }

        private void GenerateColours()
        {
            _colourGenerator.UpdateColours();
            for (var i = 0; i < 6; i++)
                if (meshFilters[i].gameObject.activeSelf)
                    _terrainFaces[i].UpdateUVs(_colourGenerator);
        }
    }
}