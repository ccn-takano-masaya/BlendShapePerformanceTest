using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ftol.Fashion
{
    public interface IFashionItemView
    {
        void AttachItem(Dictionary<string, Transform> bones);
        void SetBlendShapeWeight(string blendShapeName, float value);
        void Dispose();
    }

    public class FashionItemView : IFashionItemView, IDisposable
    {
        private readonly FashionItemAsset itemAsset;
        private GameObject prefab;

        private readonly Dictionary<string, List<SkinnedMeshRenderer>> blendShapes = new();
        private readonly Dictionary<string, Transform> attachBones = new();

        public FashionItemView(FashionItemAsset itemAsset, Transform itemRoot)
        {
            this.itemAsset = itemAsset;

            prefab = Object.Instantiate(itemAsset.gameObject, itemRoot);

            attachBones.Clear();
            FindAttachBone(prefab.transform);

            var renderers = prefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                for (var i = 0; i < renderer.sharedMesh.blendShapeCount; ++i)
                {
                    var blendShapeName = renderer.sharedMesh.GetBlendShapeName(i);
                    if (!blendShapes.ContainsKey(blendShapeName))
                    {
                        blendShapes[blendShapeName] = new List<SkinnedMeshRenderer>();
                    }
                    blendShapes[blendShapeName].Add(renderer);
                }
            }
        }

        public void Dispose()
        {
            foreach (var bone in attachBones)
            {
                bone.Value.transform.SetParent(prefab.transform);
            }
            Object.Destroy(prefab);
            prefab = null;
        }

        private void FindAttachBone(Transform root)
        {
            if (root.name.StartsWith("Attach"))
            {
                attachBones.Add(root.name, root);
            }

            foreach (Transform bone in root)
            {
                FindAttachBone(bone);
            }
        }

        public void AttachItem(Dictionary<string, Transform> bones)
        {
            // Item側のAttach_で始まる名前の骨は素体側のBone_で始まる同じ名前の骨にアタッチする
            foreach (var bone in attachBones)
            {
                var targetName = bone.Key.Replace("Attach", "Bone");
                if (bones.TryGetValue(targetName, out var target))
                {
                    bone.Value.SetParent(target, true);
                    bone.Value.localPosition = Vector3.zero;
                    bone.Value.name = $"{bone.Key}@{itemAsset.name}";
                    Debug.Log($"{bone.Key}");
                }
            }
        }

        public void SetBlendShapeWeight(string blendShapeName, float value)
        {
            if (!blendShapes.TryGetValue(blendShapeName, out var renderers))
            {
                return;
            }

            foreach (var renderer in renderers)
            {
                var index = renderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
                renderer.SetBlendShapeWeight(index, value);
            }
        }
    }
}
