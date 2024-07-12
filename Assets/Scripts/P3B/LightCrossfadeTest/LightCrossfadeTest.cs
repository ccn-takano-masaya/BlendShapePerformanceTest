using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cocone.P3B.Test
{
    public class LightCrossfadeTest : P3BTestBase<LightCrossfadeTestInput, LightCrossfadeTestOutput>
    {
        public override string title => "LightCrossfade";

        private const string IMG = "img";

#if (!PROFILE_COMMENT)
        private LightmapConf lightmapConf;
#endif
        protected override void SetupCommands()
        {
            commands = new List<TestCommand>() {
                new IdleCommand(1),
                new ScreenshotCommand(IMG),
                new CrossfadeCommand(1, 2),
                new IdleCommand(1),
                new CrossfadeCommand(2, 2),
                new IdleCommand(1),
                new CrossfadeCommand(0, 2),
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

            lightmapConf = fieldController.LightmapConf;
            if (lightmapConf)
            {
                lightmapConf.Apply(0);
                cameraSet.SkyBoxCube.SetSkyAsset(lightmapConf.GetSkyAsset(0), 0f);
            }
            fieldController.UpdatePlanetItems();
            // Load Avatars
            Transform cameraTarget = null;
            for (int i = 0; i < input.avatars.Length; i++)
            {
                var data = input.avatars[i];
                var avatarObject = await Addressables.InstantiateAsync(data.addresss, fieldController.ArtToyGroup);
                avatarObject.transform.position = data.position[0];
                avatarObject.transform.rotation = Quaternion.Euler(data.rotation);
                if (!cameraTarget)
                {
                    cameraTarget = avatarObject.transform;
                }
            }

            cameraController.transform.position = input.cameraPosition;
            cameraController.transform.rotation = Quaternion.Euler(input.cameraRotation);
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
            table.CreateRow("Avatar", string.Join(Environment.NewLine, input.avatars.Select(x => x.addresss)));
            table.CreateRow("Include LightProbes", input.includeLightProbes);
            table.CreateRow("Max Lightmap Size", input.maxLightmapSize);
        }

        protected override void WriteAdditionalOutput(MarkdownCreator markdownCreator)
        {
            markdownCreator.Image(GetImagePath(IMG), string.Empty, 0, 400);
        }

        private class CrossfadeCommand : TestCommand
        {
            int lightmapIndex;

            public CrossfadeCommand(int index, float duration)
            {
                this.duration = duration;
                this.lightmapIndex = index;
            }

            public override async UniTask Execute<T, U>(TestBase<T, U> test)
            {
                var lightCrossfadeTest = test as LightCrossfadeTest;
#if (!PROFILE_COMMENT)
                lightCrossfadeTest.lightmapConf.CrossFade(lightmapIndex, duration, lightCrossfadeTest.input.includeLightProbes);
#endif
                await UniTask.Delay(TimeSpan.FromSeconds(duration));
            }

            public override string ToString()
            {
                return $"Light cross fade to index {lightmapIndex} in {duration} second{(duration > 1 ? "s" : string.Empty)}";
            }
        }
    }
}