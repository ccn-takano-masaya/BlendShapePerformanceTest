using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

namespace Cocone.P3B.Test
{
    public class FieldTest : P3BTestBase<FieldTestInput, FieldTestOutput>
    {
        public override string title => "Field";

        private const string ZOOM_IN_IMG = "zoomIn";
        private const string ZOOM_OUT_IMG = "zoomOut";

        protected override void SetupCommands()
        {
            commands = new List<TestCommand>() {
                new IdleCommand(1),
                new ScreenshotCommand(ZOOM_IN_IMG),
                new RotateCommand(TestCommand.Direction.WorldUp, 22.5f, 45),
                new RotateCommand(TestCommand.Direction.WorldUp, -22.5f, -90),
                new RotateCommand(TestCommand.Direction.WorldUp, 22.5f, 45),
                new ResetCommand(),
                new IdleCommand(1),
                new RotateCommand(TestCommand.Direction.SelfRight, -20, -10),
                new MoveCommand(TestCommand.Direction.SelfBackward, 4, 4),
                new ScreenshotCommand(ZOOM_OUT_IMG),
                new MoveCommand(TestCommand.Direction.SelfForward, 4, 4),
                new IdleCommand(1),
            };
        }

        protected override async UniTask RunTestCase()
        {
#if (!PROFILE_COMMENT)
            // Setup Camera Set
            var cameraSetObj = Instantiate(TestController.Instance.worldCameraSet, Camera.main.transform);
            var cameraSet = cameraSetObj.GetComponent<WorldCameraSet>();

            // Load Field
            var fieldObject = await Addressables.InstantiateAsync(input.fieldAddress, root.transform);
            var fieldController = fieldObject.GetComponent<FieldController>();

            var lightmapConf = fieldController.LightmapConf;
            if (lightmapConf)
            {
                lightmapConf.Apply(input.lightmapIndex);
                cameraSet.SkyBoxCube.SetSkyAsset(lightmapConf.GetSkyAsset(input.lightmapIndex), 0f);
            }
            fieldController.UpdatePlanetItems();

            // Load Avatars
            Transform cameraTarget = null;
            for (int i = 0; i < input.animatedAvatars.Length; i++)
            {
                var data = input.animatedAvatars[i];
                var avatarObject = await Addressables.InstantiateAsync(data.addresss, fieldController.ArtToyGroup);
                avatarObject.transform.position = data.position[0];
                avatarObject.transform.rotation = Quaternion.Euler(data.rotation);
                if (!cameraTarget)
                {
                    cameraTarget = avatarObject.transform;
                }
            }
            for (int i = 0; i < input.freezedAvatars.Length; i++)
            {
                var data = input.freezedAvatars[i];
                var avatarObject = await Addressables.InstantiateAsync(data.addresss, fieldController.ArtToyGroup);
                avatarObject.transform.position = data.position[0];
                avatarObject.transform.rotation = Quaternion.Euler(data.rotation);
                avatarObject.GetComponent<ArtToyItem>().SetDisplay(false);
                avatarObject.GetComponent<NavMeshAgent>().enabled = false;
                if (!cameraTarget)
                {
                    cameraTarget = avatarObject.transform;
                }
            }

            // Load Items
            var locatorGroupField = typeof(FieldController).GetField("locatorGroup", BindingFlags.Instance | BindingFlags.NonPublic);
            var locatorGroup = (LocatorManagerGroup)locatorGroupField.GetValue(fieldController);

            var locatorGroupsField = typeof(LocatorManagerGroup).GetField("locatorGroups", BindingFlags.Instance | BindingFlags.NonPublic);
            var locatorGroups = (LocatorManagerGroup.LocatorGroupInfo[])locatorGroupsField.GetValue(locatorGroup);

            foreach(var group in locatorGroups)
            {
                if (group.key.Equals("TreasureBox"))
                {
                    for (int i = 0; i < group.locators.Length; i++)
                    {
                        var locator = group.locators[i];
                        var address = input.GetTreasureBox(locator.index);
                        if (string.IsNullOrEmpty(address))
                        {
                            continue;
                        }
                        var treasureBox = await Addressables.InstantiateAsync(address, locator.transform);
                    }
                }
                else if(group.key.Equals("Item"))
                {
                    for (int i = 0; i < group.locators.Length; i++)
                    {
                        var locator = group.locators[i];
                        var address = input.GetUsableItem(locator.index);
                        if (string.IsNullOrEmpty(address))
                        {
                            continue;
                        }
                        var usableItem = await Addressables.InstantiateAsync(address, locator.transform);
                    }
                }
            }

            if (!cameraTarget) cameraTarget = fieldController.InitLocator;
            cameraController.transform.position = cameraTarget.position + cameraTarget.forward * -5 + Vector3.up * 4.5f;
            cameraController.transform.rotation = Quaternion.LookRotation(cameraTarget.position - cameraController.transform.position);
            cameraController.Init();
#endif

            // Start
            await UniTask.DelayFrame(10);
            StartProfiler();

            for (int i = 0; i < commands.Count; i++)
            {
                await commands[i].Execute(this);
            }

            StopProfiler();
        }

        protected override void WriteAdditionalInput(Table table)
        {
            table.CreateRow("Field", input.fieldAddress);
            table.CreateRow("Lightmap Index", input.lightmapIndex);
            table.CreateRow("Animated Avatar", string.Join(Environment.NewLine, input.animatedAvatars.Select(x => x.addresss)));
            table.CreateRow("Freezed Avatar", string.Join(Environment.NewLine, input.freezedAvatars.Select(x => x.addresss)));
            table.CreateRow("Treasure Box", string.Join(Environment.NewLine, input.treasureBoxAddresses));
            table.CreateRow("Usable Item", string.Join(Environment.NewLine, input.usableItemAddresses));
        }

        protected override void WriteAdditionalOutput(MarkdownCreator markdownCreator)
        {
            markdownCreator.Table(new string[] {
                "Zoom In", "Zoom Out"
            }, new string[] {
                markdownCreator.InlineImage(GetImagePath(ZOOM_IN_IMG), string.Empty, 0, 400),
                markdownCreator.InlineImage(GetImagePath(ZOOM_OUT_IMG), string.Empty, 0, 400),
            });
        }
    }
}