using Cysharp.Threading.Tasks;
using Ftol.Avatar;
using Ftol.Fashion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

namespace Ftol.Fashion
{
    public class EditModel
    {
        // TODO: protoへ統合
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

        private static readonly int MaxHistoryCount = 50;

#if false
    private AvatarViewEditData editData;
    private readonly IItemMaster fashionMaster;

    private readonly List<AvatarViewEditData> histories = new();
    private int historyIndex;

    private readonly ReactiveProperty<AvatarNotifyData> updateAvatarViewProperty = new();
    public ReadOnlyReactiveProperty<AvatarNotifyData> UpdateAvatarViewProperty => updateAvatarViewProperty;

    public IReadOnlyList<string> DefaultItems => defaultItems.Select(x => x.Value).ToList();
#endif

        private readonly Dictionary<ItemType, string> defaultItems = new()
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
    }

}



public class FtolFashionManager/* : MonoBehaviour*/
{
    private static FtolFashionManager _singleInstance = new FtolFashionManager();
    public static FtolFashionManager GetInstance()
    {
        return _singleInstance;
    }

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

    private readonly Dictionary<ItemType, string> defaultItems = new()
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

    //private List<GameObject> _avatorList = new List<GameObject>();
    private List<AvatarData> _avatorDataList = new List<AvatarData>();

    // Start is called before the first frame update
    async void Start()
    {
        await AvatarData.LoadAvatarDatas();     //アバターデータを読み込んでおく

        //var avatorData = new AvatarData();
        //avatorData.Initialize("Avator000");

        //CreateAvator("Avator000");
    }

    public void AddAvator()
    {
        int count = _avatorDataList.Count;
        string avatorName = $"Avator{count:D3}";
        AvatarData avator = CreateAvator(avatorName);
        if(avator == null)
        {
            return;
        }
        _avatorDataList.Add(avator);

        //座標を設定
        float x_one_dist = 0.8f;
        float y_one_dist = 1.2f;
        int x_no = 3;       //横に並べる数
        float x_width = x_one_dist * (x_no - 1);

        int x = count % x_no;
        int y = count / x_no;

        var rootObj = avator.GetRootObj();
        rootObj.transform.position = new Vector3(-(x_width/2) + (x * x_one_dist), y * y_one_dist, 0);
    }

    public void DeleteAvator()
    {
        int count = _avatorDataList.Count;
        if(count <= 0)
        {
            return;
        }
        int index = count -1;
        var avatorObject = _avatorDataList[index];
        avatorObject.Dispose();

        _avatorDataList[index] = null;

        //Destroy(avatorObject);
        _avatorDataList.RemoveAt(index);
    }

    public AvatarData CreateAvator(string avatorName)
    {
#if true
        var avatorData = new AvatarData();
        avatorData.Initialize(avatorName);
        return avatorData;
#else
        var avatorRoot = CreateFashionBase();
        if (avatorRoot == null)
        {
            return null;
        }
        foreach (var item in defaultItems)
        {
            var key = item.Key;
            CreateFashionItem(key,avatorRoot);
        }
        avatorRoot.name = avatorName;
        return avatorRoot;
#endif
    }

    private GameObject CreateFashionBase()
    {
        string prefabName = "Avatar/Mesh_FaceBase/Mesh_FaceBase";
        GameObject prefab = (GameObject)Resources.Load(prefabName);
        if (prefab == null)
        {
            return null;
        }

        var avatorRoot = new GameObject();
        var position = new Vector3(0, 0, 0);
        GameObject baseObject = GameObject.Instantiate(prefab, position, Quaternion.identity);

        baseObject.transform.SetParent(avatorRoot.transform, false);
        return avatorRoot;
    }

    private bool CreateFashionItem(ItemType itemType,GameObject avatorRoot)
    {
        string itemName = defaultItems[itemType];
        string prefabName = _fashionItemDir + itemName + "/" + itemName;

        GameObject prefab = (GameObject)Resources.Load(prefabName);
        if (prefab == null) {
            return false;
        }

        var position = new Vector3(0, 0, 0);
        GameObject itemObject = GameObject.Instantiate(prefab, position, Quaternion.identity);
        itemObject.transform.SetParent(avatorRoot.transform);
        return true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
