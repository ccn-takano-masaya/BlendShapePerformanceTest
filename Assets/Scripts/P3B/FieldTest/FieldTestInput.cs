using UnityEngine;
using System;

namespace Cocone.P3B.Test
{
    [Serializable]
    public class FieldTestInput : P3BTestInputBase
    {
        [Serializable]
        public class AvatarData
        {
            public string addresss;
            public Vector3[] position;
            public Vector3 rotation;
        }

        public string fieldAddress;
        public int lightmapIndex;
        public AvatarData[] animatedAvatars;
        public AvatarData[] freezedAvatars;
        public string[] treasureBoxAddresses;
        public string[] usableItemAddresses;

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (string.IsNullOrEmpty(fieldAddress))
            {
                fieldAddress = "field/WFD-TW00_005_00_01";
            }
            if (animatedAvatars == null)
            {
                animatedAvatars = new AvatarData[0];
            }
            if (freezedAvatars == null)
            {
                freezedAvatars = new AvatarData[0];
            }
            if (treasureBoxAddresses == null)
            {
                treasureBoxAddresses = new string[0];
            }
            if (usableItemAddresses == null)
            {
                usableItemAddresses = new string[0];
            }
        }

        public string GetTreasureBox(int index)
        {
            return treasureBoxAddresses.Length > 0 ? treasureBoxAddresses[index % treasureBoxAddresses.Length] : null;
        }

        public string GetUsableItem(int index)
        {
            return usableItemAddresses.Length > 0 ? usableItemAddresses[index % usableItemAddresses.Length] : null;
        }
    }
}