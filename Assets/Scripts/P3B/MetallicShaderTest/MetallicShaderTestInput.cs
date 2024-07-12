using UnityEngine;
using System;

namespace Cocone.P3B.Test
{
    [Serializable]
    public class MetallicShaderTestInput : P3BTestInputBase
    {
        [Serializable]
        public class ArtToyData
        {
            public string addresss;
            public Vector3 position;
            public Vector3 rotation;
        }

        [Serializable]
        public class SceneData
        {
            public Vector3 cameraPosition;
            public Vector3 cameraRotation;
            public int lightmapIndex;
            public string fieldAddress;
            public ArtToyData[] artToys;
        }

        public SceneData[] sceneData;

        public override void OnAfterDeserialize()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            base.OnAfterDeserialize();
        }
    }
}