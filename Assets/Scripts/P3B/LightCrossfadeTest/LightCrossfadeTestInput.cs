using UnityEngine;
using System;

namespace Cocone.P3B.Test
{
    [Serializable]
    public class LightCrossfadeTestInput : P3BTestInputBase
    {
        [Serializable]
        public class AvatarData
        {
            public string addresss;
            public Vector3[] position;
            public Vector3 rotation;
        }

        public string fieldAddress;
        public AvatarData[] avatars;
        public bool includeLightProbes;
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
        public int maxLightmapSize;

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (maxLightmapSize < 128)
            {
                maxLightmapSize = 512;
            }
        }

        public override void ApplySettings()
        {
            base.ApplySettings();
#if (!PROFILE_COMMENT)
            BlendTexture.maxSize = maxLightmapSize;
#endif
        }
    }
}