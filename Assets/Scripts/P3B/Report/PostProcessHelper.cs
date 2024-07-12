using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Cocone.P3B.Test
{
    public static class PostProcessHelper
    {
        public static void ToMarkdown(MarkdownCreator creator, int size = 2, string title = "Post-processing")
        {
            creator.Heading(title, size);
            var table = new Table();

            var volumes = VolumeManager.instance.GetVolumes(1 << Camera.main.gameObject.layer);

            for (int i = 0; i < volumes.Length; i++)
            {
                var volume = volumes[i];
                table.CreateHeader(volume.name);
                table.CreateRow("isGlobal", volume.isGlobal);
                if (!volume.isGlobal)
                {
                    table.CreateRow("blendDistance", volume.blendDistance);
                }
                table.CreateRow("weight", volume.weight);
                table.CreateRow("priority", volume.priority);
                var profile = volume.profile;
                var effects = new List<string>();
                for (int j = 0; j < profile.components.Count; j++)
                {
                    if (profile.components[j].active)
                    {
                        effects.Add(profile.components[j].GetType().Name);
                    }
                }
                table.CreateRow("effects", string.Join(Environment.NewLine, effects));
            }

            var sb = new StringBuilder();
            table.BuildString(sb);
            creator.Paragraph(sb.ToString());
        }
    }
}