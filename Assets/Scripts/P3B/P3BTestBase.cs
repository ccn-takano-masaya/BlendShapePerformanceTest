using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Ftol.Avatar;

namespace Cocone.P3B.Test
{
    public abstract class P3BTestBase<T, U> : TestBase<T, U> where T : P3BTestInputBase where U : P3BTestOutputBase
    {
        public abstract string title { get; }

        protected GameObject root;
        private MarkdownCreator markdownCreator => output.markdownCreator;
        private ProfilerBase[] timeProfilers;
        private ProfilerBase[] memoryProfilers;
        private ProfilerBase[] renderingCommandProfilers;
        private ProfilerBase[] renderingMeshProfilers;
        private ProfilerBase[] renderingMaterialProfilers;
        private ProfilerBase[] renderingTimeProfilers;
        private ProfilerBase[] particleSystemProfilers;
        private ProfilerBase[] animationProfilers;
        private ProfilerBase[] physicsProfilers;
        private ProfilerBase[] scriptProfilers;

        protected override async UniTask<bool> RunTest()
        {
            SetupCommands();

            timeProfilers = new ProfilerBase[]
            {
                new TimeProfiler(TimeType.CPU),
                new TimeProfiler(TimeType.GPU),
            };
            memoryProfilers = new ProfilerBase[]
            {
                new MemoryProfiler(MemoryType.SystemUsedMemory),
                new MemoryProfiler(MemoryType.TextureMemory),
                new MemoryProfiler(MemoryType.MeshMemory),
            };
            renderingCommandProfilers = new ProfilerBase[]
            {
                new RenderProfiler(RenderType.SetPassCallsCount),
                new RenderProfiler(RenderType.DrawCallsCount),
            };
            renderingMeshProfilers = new ProfilerBase[]
            {
                new RenderProfiler(RenderType.TrianglesCount),
                new RenderProfiler(RenderType.VerticesCount),
            };
            renderingMaterialProfilers = new ProfilerBase[]
            {
                new MemoryProfiler(MemoryType.MaterialCount),
            };
            renderingTimeProfilers = new ProfilerBase[]
            {
                new ScriptProfiler("PostLateUpdate.FinishFrameRendering"),
                new ScriptProfiler("PostLateUpdate.PlayerUpdateCanvases"),
                new ScriptProfiler("TextureStreamingManager.Update"),
            };
            particleSystemProfilers = new ProfilerBase[]
            {
                new ScriptProfiler("ParticleSystem.Update"),
                new ScriptProfiler("ParticleSystem.EndUpdateAll"),
                new ScriptProfiler("ParticleSystem.ScheduleGeometryJobs"),
                new ScriptProfiler("ParticleSystem.Draw"),
            };
            animationProfilers = new ProfilerBase[]
            {
                new ScriptProfiler("Animators.Update"),
                new ScriptProfiler("AnimatorControllerPlayable.PrepareFrame"),
                new ScriptProfiler("MeshSkinning.Update"),
                new ScriptProfiler("SkinnedMeshFinalizeUpdate"),
            };
            physicsProfilers = new ProfilerBase[]
            {
                new ScriptProfiler("Physics.Simulate"),
                new ScriptProfiler("Physics.Processing"),
                new ScriptProfiler("Physics.FetchResults"),
                new ScriptProfiler("Physics.SyncColliderTransform"),
                new ScriptProfiler("Physics.SyncRigibodyTransform"),
                new ScriptProfiler("Physics.ProcessReports"),
                new ScriptProfiler("Physics.UpdateBodies"),
            };
            scriptProfilers = new ProfilerBase[]
            {
                new ScriptProfiler("BehaviourUpdate"),
                new ScriptProfiler("LateBehaviourUpdate"),
                new ScriptProfiler("FixedBehaviourUpdate"),
                new ScriptProfiler("MagicaPhysicsManager"),
                new ScriptProfiler("NavMeshManager"),
            };

            profilers = new List<ProfilerBase>();
            profilers.AddRange(timeProfilers);
            profilers.AddRange(memoryProfilers);
            profilers.AddRange(renderingCommandProfilers);
            profilers.AddRange(renderingMeshProfilers);
            profilers.AddRange(renderingMaterialProfilers);
            profilers.AddRange(renderingTimeProfilers);
            profilers.AddRange(particleSystemProfilers);
            profilers.AddRange(animationProfilers);
            profilers.AddRange(physicsProfilers);
            profilers.AddRange(scriptProfilers);

            if (!await base.RunTest())
            {
                return false;
            }

            root = new GameObject("root");

            WriteHeader();
            WriteInput();
            WriteTestFlow();
            await Clean();
            await RunTestCase();
            await WriteTestCase();
            WriteAppendix();

            return true;
        }

        protected abstract void SetupCommands();

        protected virtual async UniTask RunTestCase() { }

        protected virtual void WriteAdditionalInput(Table table) {}

        protected virtual void WriteAdditionalOutput(MarkdownCreator markdownCreator) { }

        private async UniTask Clean()
        {
            foreach (Transform child in root.transform)
            {
                Destroy(child.gameObject);
            }
            await Resources.UnloadUnusedAssets();
        }

        private void WriteHeader()
        {
            markdownCreator.Heading(title + " Report", 1);
            markdownCreator.Paragraph("Created Time - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
            markdownCreator.TOC();
        }

        private void WriteInput()
        {
            markdownCreator.Heading("Settings", 1);
            var table = new Table();

            table.CreateRow("Target Frame Rate", Application.targetFrameRate);
            table.CreateRow("Quality Level", QualitySettings.names[QualitySettings.GetQualityLevel()]);
            table.CreateRow("Width", Screen.width);
            table.CreateRow("Height", Screen.height);

            if (input.overrideQualitySettings.Length > 0)
            {
                table.CreateHeader("QualitySettings");
                for (int i = 0; i < input.overrideQualitySettings.Length; i++)
                {
                    var setting = input.overrideQualitySettings[i];
                    table.CreateRow(setting.name, setting.value);
                }
            }

            if (input.overrideURPAsset.Length > 0)
            {
                table.CreateHeader("UniversalRenderPipelineAsset");
                for (int i = 0; i < input.overrideURPAsset.Length; i++)
                {
                    var setting = input.overrideURPAsset[i];
                    table.CreateRow(setting.name, setting.value);
                }
            }

            if (input.overrideURPRendererData.Length > 0)
            {
                table.CreateHeader("UniversalRendererData");
                for (int i = 0; i < input.overrideURPRendererData.Length; i++)
                {
                    var setting = input.overrideURPRendererData[i];
                    table.CreateRow(setting.name, setting.value);
                }
            }

            if (input.overrideCamera.Length > 0)
            {
                table.CreateHeader("Camera");
                for (int i = 0; i < input.overrideCamera.Length; i++)
                {
                    var setting = input.overrideCamera[i];
                    table.CreateRow(setting.name, setting.value);
                }
            }

            if (input.overrideURPCameraData.Length > 0)
            {
                table.CreateHeader("UniversalAdditionalCameraData");
                for (int i = 0; i < input.overrideURPCameraData.Length; i++)
                {
                    var setting = input.overrideURPCameraData[i];
                    table.CreateRow(setting.name, setting.value);
                }
            }

            if (input.overridePostprocess.Length > 0)
            {
                table.CreateHeader("VolumeProfile");
                for (int i = 0; i < input.overridePostprocess.Length; i++)
                {
                    var setting = input.overridePostprocess[i];
                    table.CreateRow(setting.name, setting.value);
                }
            }

            WriteAdditionalInput(table);

            var sb = new StringBuilder();
            table.BuildString(sb);
            markdownCreator.Paragraph(sb.ToString());
        }

        private void WriteTestFlow()
        {
            markdownCreator.Heading("Test Flow", 1);

            float duration = 0;
            var table = new Table();
            for (int i = 0; i < commands.Count; i++)
            {
                table.CreateRow($"{duration} ~ {duration + commands[i].duration}(s)", commands[i]);
                duration += commands[i].duration;
            }
            var sb = new StringBuilder();
            table.BuildString(sb);
            markdownCreator.Paragraph(sb.ToString());
        }

        private async UniTask WriteTestCase()
        {
            markdownCreator.Heading("Result", 1);

            var xAxis = new List<float>();
            var acc = 0f;
            for (int i = 0; i < timeProfilers[0].values.Count; i++)
            {
                acc += timeProfilers[0].values[i] * 1e-6f;
                xAxis.Add(acc);
            }

            WriteAdditionalOutput(markdownCreator);

            for (int i = 0; i < timeProfilers.Length; i++)
            {
                var profiler = timeProfilers[i];
                ProfilerHelper.ToMarkdown(markdownCreator, profiler);
                await WriteChart(profiler, xAxis);
            }

            ProfilerHelper.ToMarkdown(markdownCreator, memoryProfilers, "Memory");
            await WriteChart(memoryProfilers, xAxis, "Memory");

            markdownCreator.Heading("Rendering", 2);
            ProfilerHelper.ToMarkdown(markdownCreator, renderingCommandProfilers, "Rendering Commands", 3);
            await WriteChart(renderingCommandProfilers, xAxis, "Rendering Commands");
            ProfilerHelper.ToMarkdown(markdownCreator, renderingMeshProfilers, "Rendering Meshes", 3);
            await WriteChart(renderingMeshProfilers, xAxis, "Rendering Meshes");
            ProfilerHelper.ToMarkdown(markdownCreator, renderingMaterialProfilers, "Rendering Materials", 3);
            await WriteChart(renderingMaterialProfilers, xAxis, "Rendering Materials");
            ProfilerHelper.ToMarkdown(markdownCreator, renderingTimeProfilers, "Rendering Time", 3);
            await WriteChart(renderingTimeProfilers, xAxis, "Rendering Time");

            ProfilerHelper.ToMarkdown(markdownCreator, particleSystemProfilers, "Particle System");
            await WriteChart(particleSystemProfilers, xAxis, "Particle System");

            ProfilerHelper.ToMarkdown(markdownCreator, animationProfilers, "Animation");
            await WriteChart(animationProfilers, xAxis, "Animation");

            ProfilerHelper.ToMarkdown(markdownCreator, physicsProfilers, "Physics");
            await WriteChart(physicsProfilers, xAxis, "Physics");

            ProfilerHelper.ToMarkdown(markdownCreator, scriptProfilers, "Script");
            await WriteChart(scriptProfilers, xAxis, "Script");
        }

        private async UniTask WriteChart(ProfilerBase profiler, IList<float> xAxis)
        {
            var texture = await ProfilerHelper.GetChartImage(profiler, xAxis);
            if (texture)
            {
                var path = SaveImage(texture, profiler.Name);
                markdownCreator.Image(path, string.Empty, 0, 300);
            }
        }

        private async UniTask WriteChart(ProfilerBase[] profilers, IList<float> xAxis, string title)
        {
            var texture = await ProfilerHelper.GetChartImage(profilers, xAxis);
            if (texture)
            {
                var path = SaveImage(texture, title);
                markdownCreator.Image(path, string.Empty, 0, 300);
            }
        }

        private void WriteAppendix()
        {
            markdownCreator.Heading("Appendix", 1);
            SystemInfoHelper.ToMarkdown(markdownCreator);
            ApplicationHelper.ToMarkdown(markdownCreator);
            QualitySettingsHelper.ToMarkdown(markdownCreator);
            URPAssetHelper.ToMarkdown(markdownCreator);
            URPRendererDataHelper.ToMarkdown(markdownCreator);
            CameraHelper.ToMarkdown(markdownCreator);
            PostProcessHelper.ToMarkdown(markdownCreator);
        }
    }
}