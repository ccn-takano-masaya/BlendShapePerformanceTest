using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using Ftol.Avatar;

namespace Cocone.P3B.Test
{
    public class AvatarTest : P3BTestBase<AvatarTestInput, AvatarTestOutput>
    {
        public override string title => "Avatar Test";

        private const string ZOOM_IN_IMG = "zoomIn";
        private const string ZOOM_OUT_IMG = "zoomOut";

        private float loadTime;

        protected override void SetupCommands()
        {
            commands = new List<TestCommand>() {
                new IdleCommand(1),
                new ScreenshotCommand(ZOOM_IN_IMG),
                new IdleCommand(5),
            };
        }

        protected override async UniTask RunTestCase()
        {
            float startTime = Time.realtimeSinceStartup;
            int maxItemNo = input.maxCollectionItem;
            await AvatarData.LoadAvatarDatas();     //アバターデータを読み込んでおく
            var ftolFashionManager = FtolFashionManager.GetInstance();

            for(int i = 0; i<maxItemNo; i++)
                ftolFashionManager.AddAvator();

#if (!PROFILE_COMMENT)
            // Load Collection Room
            var collectionRoomObject = await Addressables.InstantiateAsync(input.collectionRoomAddress, root.transform);
            var collectionRoomController = collectionRoomObject.GetComponent<CollectionRoomController>();

            var lightmapConf = collectionRoomController.GetComponent<LightmapConf>();
            if (lightmapConf)
            {
                lightmapConf.Apply(input.lightmapIndex);
            }

            // Load Collections
            var locatorGroup = collectionRoomObject.GetComponent<CollectionRoomLocatorGroup>();
            var locatorGroupsField = typeof(LocatorManagerGroup).GetField("locatorGroups", BindingFlags.Instance | BindingFlags.NonPublic);
            var locatorGroups = (LocatorManagerGroup.LocatorGroupInfo[])locatorGroupsField.GetValue(locatorGroup);
            var count = 0;
            foreach (var group in locatorGroups)
            {
                if (count >= input.maxCollectionItem) break;
                for (int i = 0; i < group.locators.Length; i++)
                {
                    if (count >= input.maxCollectionItem) break;
                    count++;
                    var locator = group.locators[i];
                    var address = input.GetCollectionItem(locator.index);
                    if (string.IsNullOrEmpty(address))
                    {
                        continue;
                    }
                    var collection = await Addressables.InstantiateAsync(address, root.transform);

                    var collectionItem = collection.GetComponent<ArtToyItem>();
                    var size = locatorGroup.GetSize(locator.index);
                    collectionItem.Scale =
                        size == ToySize.Large ? 1.54f :
                        size == ToySize.Middle ? 1 : 0.66f;

                    if (!input.enableAnimation)
                    {
                        collectionItem.SetDisplay(false);
                        collectionItem.ChangePose(1);
                    }

                    var navMeshAgent = collection.GetComponentInChildren<NavMeshAgent>();
                    if (navMeshAgent != null) navMeshAgent.enabled = false;

                    collectionRoomController.Add(locator.index, collection.transform);
                }
            }
#endif
            loadTime = Time.realtimeSinceStartup - startTime;

#if (!PROFILE_COMMENT)
            cameraController.transform.position = collectionRoomController.InitCameraTarget.position;
            cameraController.transform.Rotate(Vector3.right, collectionRoomController.InitCameraPosition.y * 30, Space.World);
            cameraController.transform.Rotate(Vector3.up, collectionRoomController.InitCameraPosition.x, Space.World);
            cameraController.Init();
#endif

            // Start
            await UniTask.DelayFrame(10);
            StartProfiler();

            for(int i = 0; i < commands.Count; i++)
            {
                await commands[i].Execute(this);
            }

            StopProfiler();
        }

        protected override void WriteAdditionalInput(Table table)
        {
            table.CreateRow("Lightmap Index", input.lightmapIndex);
            table.CreateRow("Enable Animation", input.enableAnimation);
        }

        protected override void WriteAdditionalOutput(MarkdownCreator markdownCreator)
        {
            markdownCreator.Paragraph($"Loading Time - {loadTime:F2}(sec)");

            markdownCreator.Table(new string[] {
                "ScreenShot"
            }, new string[] {
                markdownCreator.InlineImage(GetImagePath(ZOOM_IN_IMG), string.Empty, 0, 400),
            });
        }
    }
}