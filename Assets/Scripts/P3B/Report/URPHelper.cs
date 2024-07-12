using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Cocone.P3B.Test
{
    public static class URPAssetHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, int size = 2, string title = "UniversalRenderPipelineAsset")
        {
            creator.Heading(title, size);
            var table = new Table();
            var urpAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
            table.CreateHeader("Rendering");
            table.CreateRow("supportsCameraDepthTexture", urpAsset.supportsCameraDepthTexture);
            table.CreateRow("supportsCameraOpaqueTexture", urpAsset.supportsCameraOpaqueTexture);
            table.CreateRow("opaqueDownsampling", urpAsset.opaqueDownsampling);

            table.CreateHeader("Quality");
            table.CreateRow("supportsHDR", urpAsset.supportsHDR);
            table.CreateRow("msaaSampleCount", urpAsset.msaaSampleCount);
            table.CreateRow("renderScale", urpAsset.renderScale);
            table.CreateRow("upscalingFilter", urpAsset.upscalingFilter);
            table.CreateRow("enableLODCrossFade", urpAsset.enableLODCrossFade);
            if (urpAsset.enableLODCrossFade)
            {
                table.CreateRow("lodCrossFadeDitheringType", urpAsset.lodCrossFadeDitheringType);
            }

            table.CreateHeader("Lighting");
            table.CreateRow("mainLightRenderingMode", urpAsset.mainLightRenderingMode);
            if (urpAsset.mainLightRenderingMode != LightRenderingMode.Disabled)
            {
                table.CreateRow("supportsMainLightShadows", urpAsset.supportsMainLightShadows);
                if (urpAsset.supportsMainLightShadows)
                {
                    table.CreateRow("mainLightShadowmapResolution", urpAsset.mainLightShadowmapResolution);
                }
            }

            table.CreateRow("additionalLightsRenderingMode", urpAsset.additionalLightsRenderingMode);
            if (urpAsset.additionalLightsRenderingMode != LightRenderingMode.Disabled)
            {
                table.CreateRow("maxAdditionalLightsCount", urpAsset.maxAdditionalLightsCount);
                table.CreateRow("supportsAdditionalLightShadows", urpAsset.supportsAdditionalLightShadows);
                if (urpAsset.supportsAdditionalLightShadows)
                {
                    table.CreateRow("additionalLightsShadowmapResolution", urpAsset.additionalLightsShadowmapResolution);
                    table.CreateRow("additionalLightsShadowResolutionTierLow", urpAsset.additionalLightsShadowResolutionTierLow);
                    table.CreateRow("additionalLightsShadowResolutionTierMedium", urpAsset.additionalLightsShadowResolutionTierMedium);
                    table.CreateRow("additionalLightsShadowResolutionTierHigh", urpAsset.additionalLightsShadowResolutionTierHigh);
                }
            }

            table.CreateRow("reflectionProbeBlending", urpAsset.reflectionProbeBlending);
            table.CreateRow("reflectionProbeBoxProjection", urpAsset.reflectionProbeBoxProjection);

            table.CreateHeader("Shadows");
            table.CreateRow("shadowDistance", urpAsset.shadowDistance);
            table.CreateRow("shadowCascadeCount", urpAsset.shadowCascadeCount);
            table.CreateRow("cascade2Split", urpAsset.cascade2Split);
            table.CreateRow("cascade3Split", urpAsset.cascade3Split);
            table.CreateRow("cascade4Split", urpAsset.cascade4Split);
            table.CreateRow("cascadeBorder", urpAsset.cascadeBorder);
            table.CreateRow("shadowDepthBias", urpAsset.shadowDepthBias);
            table.CreateRow("shadowNormalBias", urpAsset.shadowNormalBias);
            table.CreateRow("supportsSoftShadows", urpAsset.supportsSoftShadows);

            table.CreateHeader("Post-processing");
            table.CreateRow("colorGradingMode", urpAsset.colorGradingMode);
            table.CreateRow("colorGradingLutSize", urpAsset.colorGradingLutSize);
            table.CreateRow("useFastSRGBLinearConversion", urpAsset.useFastSRGBLinearConversion);

            table.CreateHeader("Advanced");
            table.CreateRow("useSRPBatcher", urpAsset.useSRPBatcher);
            table.CreateRow("supportsDynamicBatching", urpAsset.supportsDynamicBatching);
            table.CreateRow("supportsMixedLighting", urpAsset.supportsMixedLighting);
            table.CreateRow("storeActionsOptimization", urpAsset.storeActionsOptimization);

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }
    }

    public static class URPRendererDataHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, int size = 2, string title = "UniversalRendererData")
        {
            creator.Heading(title, size);
            var table = new Table();
            var urpAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
            var propertyInfo = typeof(UniversalRenderPipelineAsset).GetProperty("scriptableRendererData", BindingFlags.Instance | BindingFlags.NonPublic);
            var urpRendererData = (UniversalRendererData)propertyInfo.GetGetMethod(true).Invoke(urpAsset, null);
            table.CreateRow("renderingMode", urpRendererData.renderingMode);
            table.CreateRow("depthPrimingMode", urpRendererData.depthPrimingMode);
            table.CreateRow("copyDepthMode", urpRendererData.copyDepthMode);
            table.CreateRow("useNativeRenderPass", urpRendererData.useNativeRenderPass);
            table.CreateRow("shadowTransparentReceive", urpRendererData.shadowTransparentReceive);
            table.CreateRow("usePostProcess", urpRendererData.postProcessData != null);
            table.CreateRow("intermediateTextureMode", urpRendererData.intermediateTextureMode);
 
            var features = new List<string>();
            for (int i = 0; i < urpRendererData.rendererFeatures.Count; i++)
            {
                var feature = urpRendererData.rendererFeatures[i];
                if (feature.isActive)
                {
                    features.Add(feature.name);
                }
            }
            table.CreateRow("Renderer Features", string.Join(Environment.NewLine, features));

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }
    }
}