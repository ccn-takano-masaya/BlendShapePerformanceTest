using System;
using UnityEngine;
using System.Reflection;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace Cocone.P3B.Test
{
    [Serializable]
    public abstract class TestInputBase : ISerializationCallbackReceiver
    {
        [Serializable]
        public struct Setting
        {
            public string name;
            public string value;
        }

        public int targetFrameRate;
        public int qualityLevel;
        public int resolution;
        public int width;
        public int height;
        public bool usePostprocess;

        public Setting[] overrideQualitySettings;
        public Setting[] overrideURPAsset;
        public Setting[] overrideURPRendererData;
        public Setting[] overrideCamera;
        public Setting[] overrideURPCameraData;
        public Setting[] overridePostprocess;

        public virtual void OnBeforeSerialize() { }

        public virtual void OnAfterDeserialize()
        {
            targetFrameRate = Mathf.Clamp(targetFrameRate, 15, 300);
            qualityLevel = Mathf.Clamp(qualityLevel, 0, QualitySettings.count - 1);

            if (resolution > 0)
            {
                if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                {
                    width = Mathf.Clamp(resolution, 100, Display.main.systemWidth);
                    height = Mathf.FloorToInt((float)width * Display.main.systemHeight / Display.main.systemWidth);
                }
                else
                {
                    height = Mathf.Clamp(resolution, 100, Display.main.systemHeight);
                    width = Mathf.FloorToInt((float)height * Display.main.systemWidth / Display.main.systemHeight);
                }
            }
            else if(width > 0 && height > 0)
            {
                width = Mathf.Clamp(width, 100, Display.main.systemWidth);
                height = Mathf.Clamp(height, 100, Display.main.systemHeight);
            }
            else
            {
                width = Display.main.systemWidth;
                height = Display.main.systemHeight;
            }
        }

        public virtual void ApplySettings()
        {
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.SetQualityLevel(qualityLevel);
            Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);

            for (int i = 0; i < overrideQualitySettings.Length; i++)
            {
                var setting = overrideQualitySettings[i];
                OverrideStaticSetting(typeof(QualitySettings), setting.name, setting.value);
            }

            var urpAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
            for (int i = 0; i < overrideURPAsset.Length; i++)
            {
                var setting = overrideURPAsset[i];
                OverrideInstanceSetting(urpAsset, setting.name, setting.value);
            }

            var propertyInfo = typeof(UniversalRenderPipelineAsset).GetProperty("scriptableRendererData", BindingFlags.Instance | BindingFlags.NonPublic);
            var urpRendererData = (UniversalRendererData)propertyInfo.GetGetMethod(true).Invoke(urpAsset, null);
            for (int i = 0; i < overrideURPRendererData.Length; i++)
            {
                var setting = overrideURPRendererData[i];
                OverrideInstanceSetting(urpRendererData, setting.name, setting.value);
            }

            var camera = Camera.main;
            for (int i = 0; i < overrideCamera.Length; i++)
            {
                var setting = overrideCamera[i];
                OverrideInstanceSetting(camera, setting.name, setting.value);
            }

            var cameraData = camera.GetUniversalAdditionalCameraData();
            for (int i = 0; i < overrideURPCameraData.Length; i++)
            {
                var setting = overrideURPCameraData[i];
                OverrideInstanceSetting(cameraData, setting.name, setting.value);
            }

            var volumes = TestController.Instance.volumes;
            if (usePostprocess)
            {
                for (int i = 0; i < volumes.Length; i++)
                {
                    volumes[i].enabled = true;
                }
                for (int i = 0; i < overridePostprocess.Length; i++)
                {
                    var setting = overridePostprocess[i];
                    for (int j = 0; j < volumes.Length; j++)
                    {
                        var effects = volumes[j].profile.components;
                        for (int k = 0; k < effects.Count; k++)
                        {
                            var effect = effects[k];
                            if (effect.GetType().Name.Equals(setting.name))
                            {
                                effect.active = bool.TryParse(setting.value, out var result) ? result : false;
                            }
                        }
                    }
                }
            }
            else
            {
                Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = false;
            }
        }

        private bool OverrideInstanceSetting(object instance, string name, string value)
        {
            // find field
            var fieldInfo = instance.GetType().GetField(name);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value));
                return true;
            }

            // find property
            var propInfo = instance.GetType().GetProperty(name);
            if (propInfo != null)
            {
                var setFunc = propInfo.SetMethod;
                if (setFunc != null)
                {
                    setFunc.Invoke(instance, new object[] { ParseValue(fieldInfo.FieldType, value) });
                    return true;
                }
                else
                {
                    Debug.LogError($"No public set method found in " + propInfo);
                    return false;
                }
            }

            Debug.LogError($"No public member {name} found in {instance.GetType()}");
            return false;
        }

        private bool OverrideStaticSetting(Type type, string name, string value)
        {
            // find field
            var fieldInfo = type.GetField(name);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, ParseValue(fieldInfo.FieldType, value));
                return true;
            }

            // find property
            var propInfo = type.GetProperty(name);
            if (propInfo != null)
            {
                var setFunc = propInfo.SetMethod;
                if (setFunc != null)
                {
                    setFunc.Invoke(null, new object[] { ParseValue(propInfo.PropertyType, value) });
                    return true;
                }
                else
                {
                    Debug.LogError($"No public set method found in " + propInfo);
                    return false;
                }
            }

            Debug.LogError($"No public member {name} found in {type}");
            return false;
        }

        private object ParseValue(Type type, string value)
        {
            if (type == typeof(char))
            {
                return char.TryParse(value, out var result) ? result : ' ';
            }
            else if(type == typeof(bool))
            {
                return bool.TryParse(value, out var result) ? result : false;
            }
            else if(type == typeof(sbyte))
            {
                return sbyte.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(short))
            {
                return short.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(int))
            {
                return int.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(long))
            {
                return long.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(byte))
            {
                return byte.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(ushort))
            {
                return ushort.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(uint))
            {
                return uint.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(ulong))
            {
                return ulong.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(float))
            {
                return float.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(double))
            {
                return double.TryParse(value, out var result) ? result : 0;
            }
            else if (type == typeof(string))
            {
                return value;
            }
            else if (type.IsEnum)
            {
                return Enum.TryParse(type, value, true, out var result) ? result : 0;
            }
            else if (type == typeof(Vector2))
            {
                var tokens = ParseVector(value, 2);
                return new Vector2(tokens[0], tokens[1]);
            }
            else if (type == typeof(Vector3))
            {
                var tokens = ParseVector(value, 3);
                return new Vector3(tokens[0], tokens[1], tokens[2]);
            }
            else if (type == typeof(Vector2Int))
            {
                var tokens = ParseVector(value, 2);
                return new Vector2Int((int)tokens[0], (int)tokens[1]);
            }
            else if (type == typeof(Vector3Int))
            {
                var tokens = ParseVector(value, 3);
                return new Vector3Int((int)tokens[0], (int)tokens[1], (int)tokens[2]);
            }
            else
            {
                Debug.LogError("Not supported type " + type);
                return default;
            }
        }

        private float[] ParseVector(string value, int count) {
            var result = new float[count];

            if (value.StartsWith("(") && value.EndsWith(")"))
            {
                value = value.Substring(1, value.Length - 2);
            }

            var tokens = value.Split(',');
            for (int i = 0; i < tokens.Length && i < result.Length; i++)
            {
                float.TryParse(tokens[i], out result[i]);
            }
            return result;
        }
    }
}
