using System.Collections.Generic;
using System.Linq;
using System.Threading;
//using Cocone.Asset;
using Cysharp.Threading.Tasks;
using Ftol.Fashion;
//using R3;
using UnityEngine;
//using System.Linq;
//using System.Xml.Xsl;
//using UnityEditorInternal;
using System;

namespace Ftol.Avatar
{

    public class AvatarData : IDisposable/*: MonoBehaviour*/
    {
        const string _fashionItemDir = "AvatarItems/"; //"Assets/Resources/AvatarItems/";

        public enum ItemType
        {
            None,
            Hair,
            HairFront,
            HairBack,
            HairExtension,
            HairAccessories,
            EyelashLeft,
            EyelashRight,
            EyeWhiteLeft,
            EyeWhiteRight,
            EyeIrisLeft,
            EyeIrisRight,
            EyebrowLeft,
            EyebrowRight,
            EyeEffectLeft,
            EyeEffectRight,
            EarLeft,
            EarRight,
            EarAccessoriesLeft,
            EarAccessoriesRight,
            Face,
            FaceAccessories,
            FacePaint,
            Nose,
            Mouth,
            Body,
            BodyAccessories,
            Effect,
            Hand,
            HandPaint,
            GrabAccessories,
            Bracelet,
            Ring,
        }

        static private string _avatorBaseName = "Avatar/AvatarBase";

        private readonly static Dictionary<ItemType, string> defaultItems = new()
        {
            [ItemType.Body] = "A00000BD00",
            [ItemType.EyebrowLeft] = "A00000EBL0",
            [ItemType.EyebrowRight] = "A00000EBR0",
            [ItemType.EyeIrisLeft] = "A00000EIL0",
            [ItemType.EyeIrisRight] = "A00000EIR0",
            [ItemType.EyelashLeft] = "A00000ELL0",
            [ItemType.EyelashRight] = "A00000ELR0",
            [ItemType.EarLeft] = "A00000ERL0",
            [ItemType.EarRight] = "A00000ERR0",
            [ItemType.EyeWhiteLeft] = "A00000EWL0",
            [ItemType.EyeWhiteRight] = "A00000EWR0",
            [ItemType.HairBack] = "A00000HB00",
            [ItemType.HairFront] = "A00000HF00",
            [ItemType.Mouth] = "A00000MT00",
            [ItemType.Nose] = "A00000NS00",
        };

        private Transform boonRoot;
        private Transform itemRoot;

        public Dictionary<string, IFashionItemView> FashionItems
        {
            get;
        } = new();

        private readonly Dictionary<string, Transform> bones = new();
        //private IAssetLoader<GameObject> itemLoader;

        //private static GameObject _fashionBaseGameObj = null;
        private static Dictionary<string, GameObject> _fashionGameObjs = new();

        private GameObject _rootObject = null;

        public void Dispose()
        {
            DetachAll();
            if (_rootObject != null)
            {
                GameObject.Destroy(_rootObject);
                _rootObject = null;
            }
            bones.Clear();
        }

        public GameObject GetRootObj()
        {
            return _rootObject;
        }

        /// <summary>
        /// アバターデータをロードしておく
        /// </summary>
        /// <returns></returns>
        static public async UniTask LoadAvatarDatas()
        {
            //アバターベースデータをロード
            var baseObj = await Resources.LoadAsync<GameObject>(_avatorBaseName);
            _fashionGameObjs[_avatorBaseName] = baseObj as GameObject;

            //ファッションアイテムをロード
            foreach (var item in defaultItems)
            {
                var itemType = item.Key;
                string itemName = defaultItems[itemType];
                string fashionPrefabName = _fashionItemDir + itemName + "/" + itemName;

                var fashionObj = await Resources.LoadAsync<GameObject>(fashionPrefabName);
                GameObject fashionGameObj = fashionObj as GameObject;

                if(fashionGameObj.GetComponent<FashionItemAsset>() == null)
                    fashionGameObj.AddComponent<FashionItemAsset>();

                _fashionGameObjs[fashionPrefabName] = fashionGameObj;
            }
        }

        public GameObject Initialize(string nodeName)
        //public void Initialize(IAssetLoader<GameObject> itemLoader)
        {
            bones.Clear();

            //this.itemLoader = itemLoader;
            _rootObject = CreateFashionBase();
            var childTransform = _rootObject.transform.GetComponentsInChildren<Transform>();
            //var match = childTransform.Where((_ => _.name == "Bone_Root"));
            var boneRootTransform = childTransform.First((_ => _.name == "Bone_Root"));
            if (boneRootTransform != null)
            {
                boonRoot = boneRootTransform;
                AddBones(boonRoot);
            }

            var itemRootTransform = childTransform.First((_ => _.name == "ItemRoot"));
            if (itemRootTransform != null)
            {
                itemRoot = itemRootTransform;
            }

            foreach (var item in defaultItems)
            {
                var key = item.Key;
                CreateFashionItem(key, _rootObject);
            }
            _rootObject.name = nodeName;
            return _rootObject;
        }

        /// <summary>
        /// アバターのベースを作る
        /// </summary>
        /// <returns></returns>
        private GameObject CreateFashionBase()
        {
            GameObject prefab = _fashionGameObjs[_avatorBaseName];
            if (prefab == null)
            {
                return null;
            }
            var position = new Vector3(0, 0, 0);
            GameObject baseObject = GameObject.Instantiate(prefab, position, Quaternion.identity);
            return baseObject;
        }

        private bool CreateFashionItem(ItemType itemType, GameObject avatorRoot)
        {
            string itemName = defaultItems[itemType];
            string prefabName = _fashionItemDir + itemName + "/" + itemName;

            //GameObject prefab = (GameObject)Resources.Load(prefabName);
            GameObject prefab = _fashionGameObjs[prefabName];
            if (prefab == null)
            {
                return false;
            }

            //var position = new Vector3(0, 0, 0);
            //GameObject itemObject = Instantiate(prefab, position, Quaternion.identity);
            //var itemAsset = prefab.AddComponent<FashionItemAsset>();
            FashionItemAsset itemAsset = prefab.GetComponent<FashionItemAsset>();
            AttachItem(itemAsset);
            //itemObject.transform.SetParent(avatorRoot.transform);
            return true;
        }

#if false
        public void OnDestroy()
        {
            DetachAll();
        }
#endif
        private void AddBones(Transform root)
        {
            try
            {
                bones.Add(root.name, root);
                foreach (Transform bone in root)
                {
                    AddBones(bone);
                }
            }
            catch
            {
                Debug.Log("exception");
            }
        }

        public void DetachAll()
        {
            foreach (var item in FashionItems.Values)
            {
                item.Dispose();
            }
            FashionItems.Clear();
        }

        private bool IsExist(string itemId)
        {
            return FashionItems.ContainsKey(itemId);
        }

#if false
        public async UniTask UpdateAvatar(IAvatarData avatarViewData, CancellationToken token = default)
        {
            var wearingItems = avatarViewData.CollectWearingItems().OrderBy(x => x.Order).ToArray();
            if (wearingItems.Length > 0)
            {
                var observables = wearingItems.Where(x => !IsExist(x.Id)).Select(x =>
                {
                    return itemLoader.LoadAsync(x.Id, true).Select(item =>
                    {
                        if (item == null)
                        {
                            return (x, null);
                        }
                        if (item.TryGetComponent<FashionItemAsset>(out var itemAsset))
                        {
                            return (x, itemAsset);
                        }

                        return (x, null);
                    }).FirstAsync(token).AsUniTask();
                });

                var (cancel, items) = await UniTask.WhenAll(observables).SuppressCancellationThrow();
                if (cancel)
                {
                    return;
                }

                foreach (var (item, asset) in items)
                {
                    AttachItem(item, asset);
                }
            }

            DetachItems(wearingItems);
        }
#endif

        private void AttachItem(/*FashionItem item,*/ FashionItemAsset itemAsset)
        {
            if (itemAsset == null)
            {
                return;
            }

            if (!itemAsset.IsValid())
            {
                return;
            }

            if (FashionItems.TryGetValue(itemAsset.name, out var itemView))
            {
                return;
            }
            itemView = new FashionItemView(itemAsset, itemRoot);
            itemView.AttachItem(bones);
            FashionItems.Add(itemAsset.Name, itemView);
        }

#if false
        public void SetBlendShapeWeight(IEnumerable<FashionItem> items, string blendShape)
        {
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                if (FashionItems.TryGetValue(item.Id, out var itemView))
                {
                    itemView.SetBlendShapeWeight(blendShape, item.BlendShapeWeight);
                }
            }
        }
#endif

#if false
        private void DetachItems(FashionItem[] items)
        {
            var ids = FashionItems.Keys.ToArray();
            foreach (var id in ids)
            {
                if (items.Any(x => id == x.Id))
                {
                    continue;
                }

                FashionItems[id].Dispose();
                FashionItems.Remove(id);
            }
        }
#endif
    }
}
