using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cocone.P3B.Test
{
    public class MetallicShaderTest : P3BTestBase<MetallicShaderTestInput, MetallicShaderTestOutput>
    {
        public override string title => "MetallicShaderTest";

        private List<string> imgs;

        protected override void SetupCommands()
        {
            commands = new List<TestCommand>() { };
        }

        protected override async UniTask RunTestCase()
        {
            // Setup Camera Set
            var cameraSetObj = Instantiate(TestController.Instance.worldCameraSet, Camera.main.transform);
#if (!PROFILE_COMMENT)
            var cameraSet = cameraSetObj.GetComponent<WorldCameraSet>();
#endif
            imgs = new List<string>();

            // Start
            await UniTask.DelayFrame(10);
            StartProfiler();

            for (int i = 0; i < input.sceneData.Length; i++)
            {
#if (!PROFILE_COMMENT)
                await TestScene(cameraSet.SkyBoxCube, input.sceneData[i]);
#endif
            }

            StopProfiler();
        }

#if (!PROFILE_COMMENT)
        private async UniTask TestScene(SkyBoxCube skyBoxCube, MetallicShaderTestInput.SceneData sceneData)
        {
            paused = true;
            // Load Field
            var fieldObject = await Addressables.InstantiateAsync(sceneData.fieldAddress, root.transform);
            var fieldController = fieldObject.GetComponent<FieldController>();

            var lightmapConf = fieldController.LightmapConf;
            if (lightmapConf)
            {
                lightmapConf.Apply(sceneData.lightmapIndex);
                skyBoxCube.SetSkyAsset(lightmapConf.GetSkyAsset(sceneData.lightmapIndex), 0f);
            }
            fieldController.UpdatePlanetItems();

            // Load Avatars
            for (int i = 0; i < sceneData.artToys.Length; i++)
            {
                var data = sceneData.artToys[i];
                var avatarObject = await Addressables.InstantiateAsync(data.addresss, fieldController.ArtToyGroup);
                avatarObject.GetComponent<ArtToyItem>().NavMeshAgent.enabled = false;
                avatarObject.transform.position = data.position;
                avatarObject.transform.rotation = Quaternion.Euler(data.rotation);
            }

            cameraController.transform.position = sceneData.cameraPosition;
            cameraController.transform.rotation = Quaternion.Euler(sceneData.cameraRotation);
            cameraController.Init();

            imgs.Add(sceneData.fieldAddress.Replace("field/", ""));
            var screenshot = new ScreenshotCommand(imgs[imgs.Count - 1]);
            await screenshot.Execute(this);

            await UniTask.DelayFrame(10);
            paused = false;

            await UniTask.Delay(5000);

            paused = true;
            Destroy(fieldObject);
            await UniTask.DelayFrame(10);
            paused = false;
        }
#endif

        protected override void WriteAdditionalOutput(MarkdownCreator markdownCreator)
        {
            for (int i = 0; i < imgs.Count; i ++)
            {
                markdownCreator.Image(GetImagePath(imgs[i]), string.Empty, 0, 400);
            }
        }
    }
}