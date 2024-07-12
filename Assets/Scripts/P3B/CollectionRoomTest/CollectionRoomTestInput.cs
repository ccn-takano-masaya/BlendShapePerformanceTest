using System;

namespace Cocone.P3B.Test
{
    [Serializable]
    public class CollectionRoomTestInput : P3BTestInputBase
    {
        public string collectionRoomAddress;
        public int lightmapIndex;
        public string[] collectionAddresses;
        public int maxCollectionItem;
        public bool enableAnimation;

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (string.IsNullOrEmpty(collectionRoomAddress))
            {
                collectionRoomAddress = "item/UCR-T0002003";
            }
            if (maxCollectionItem <= 0)
            {
                maxCollectionItem = int.MaxValue;
            }
        }

        public string GetCollectionItem(int index)
        {
            return collectionAddresses.Length > 0 ? collectionAddresses[index % collectionAddresses.Length] : null;
        }
    }
}