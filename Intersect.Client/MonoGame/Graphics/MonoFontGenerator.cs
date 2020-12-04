using HarmonyLib;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Framework.Content.Pipeline.Builder;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Intersect.Client.MonoGame.Graphics
{
    internal class MonoFontGenerator
    {
        private static string GenerateXml(SpriteFontDescriptor spriteFontDescriptor)
        {
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(
                stringBuilder, new XmlWriterSettings
                {
                    Encoding = Encoding.Unicode,
                    Indent = true
                }
            ))
            {
                xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");

                xmlWriter.WriteStartElement("XnaContent");
                xmlWriter.WriteAttributeString(
                    "xmlns", "Graphics", null, "Microsoft.Xna.Framework.Content.Pipeline.Graphics"
                );

                xmlWriter.WriteStartElement("Asset");
                xmlWriter.WriteAttributeString("Type", "Graphics:FontDescription");

                xmlWriter.WriteStartElement(nameof(SpriteFontDescriptor.FontName));
                xmlWriter.WriteValue(spriteFontDescriptor.FontName);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(nameof(SpriteFontDescriptor.Size));
                xmlWriter.WriteValue(spriteFontDescriptor.Size);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(nameof(SpriteFontDescriptor.Spacing));
                xmlWriter.WriteValue(spriteFontDescriptor.Spacing);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(nameof(SpriteFontDescriptor.UseKerning));
                xmlWriter.WriteValue(spriteFontDescriptor.UseKerning);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement(nameof(SpriteFontDescriptor.Style));
                xmlWriter.WriteValue(spriteFontDescriptor.Style.ToTextStyle());
                xmlWriter.WriteEndElement();

                if (spriteFontDescriptor.DefaultCharacter != null)
                {
                    xmlWriter.WriteStartElement(nameof(SpriteFontDescriptor.DefaultCharacter));
                    xmlWriter.WriteValue(spriteFontDescriptor.DefaultCharacter.ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteStartElement(nameof(SpriteFontDescriptor.CharacterRegions));
                foreach (var region in spriteFontDescriptor.CharacterRegions)
                {
                    if (!string.IsNullOrWhiteSpace(region.Comment))
                    {
                        xmlWriter.WriteComment(region.Comment);
                    }

                    xmlWriter.WriteStartElement(nameof(CharacterRegion));

                    xmlWriter.WriteStartElement(nameof(CharacterRegion.Start));
                    xmlWriter.WriteRaw($"&#x{region.Start:X4};");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement(nameof(CharacterRegion.End));
                    xmlWriter.WriteRaw($"&#x{region.End:X4};");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return stringBuilder.ToString();
        }

        public static AssetReference GenerateSpriteFont(SpriteFontDescriptor spriteFontDescriptor)
        {
            try
            {
                var fontDirectoryName = "fonts";
                var cache = new DirectoryInfo(".cache");
                if (!cache.Exists)
                {
                    cache.Create();
                    cache.Attributes &= FileAttributes.Hidden;
                }

                var projectDirectory = Path.Combine(cache.FullName, "pipeline");
                var fontDirectory = Path.Combine(projectDirectory, fontDirectoryName);
                var outputDirectory = Path.Combine(projectDirectory, "bin");
                var intermediateDirectory = Path.Combine(projectDirectory, "obj");

                if (!Directory.Exists(fontDirectory))
                {
                    Directory.CreateDirectory(fontDirectory);
                }

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                if (!Directory.Exists(intermediateDirectory))
                {
                    Directory.CreateDirectory(intermediateDirectory);
                }

                var assetName = spriteFontDescriptor.ToSpriteFontName();
                var spriteFontSourceFilePath = Path.Combine(fontDirectory, $"{assetName}.spritefont");
                var spriteFontOutputPartialFilePath = Path.Combine(fontDirectoryName, $"{assetName}.xnb");
                var xmlSource = GenerateXml(spriteFontDescriptor);
                File.WriteAllText(spriteFontSourceFilePath, xmlSource);

                var pipelineManager = new PipelineManager(projectDirectory, outputDirectory, intermediateDirectory)
                {
                    Profile = GraphicsProfile.HiDef, CompressContent = true, Platform = TargetPlatform.DesktopGL
                };

                var patchedMethods = IntersectGame.Harmony.GetPatchedMethods().ToList();
                string importerName = string.Empty, processorName = string.Empty;
                pipelineManager.ResolveImporterAndProcessor(
                    spriteFontSourceFilePath, ref importerName, ref processorName
                );

                var buildEvent = pipelineManager.BuildContent(spriteFontSourceFilePath, spriteFontOutputPartialFilePath);
                var assetReference = new AssetReference(null, ContentType.Font, assetName, buildEvent.DestFile);

                return assetReference;
            }
            catch (Exception exception)
            {
                Debugger.Break();

                return default;
            }
        }
    }

    internal class SpriteFontDescriptor
    {
        public string FontName { get; set; } = "Arial";

        public int Size { get; set; } = 12;

        public float Spacing { get; set; } = 0;

        public bool UseKerning { get; set; } = true;

        public char? DefaultCharacter { get; set; } = null;

        public SpriteFontStyle Style { get; set; } = SpriteFontStyle.Regular;

        public List<CharacterRegion> CharacterRegions { get; set; } = new List<CharacterRegion>();

        public string ToSpriteFontName() => GameFont.FormatAssetName(FontName, Size);
    }

    internal class CharacterRegion
    {
        public string Comment { get; set; }

        public int Start { get; set; } = 0x20;

        public int End { get; set; } = 0xFF;
    }

    internal enum SpriteFontStyle
    {
        Regular,

        Bold,

        Italic,

        BoldItalic
    }

    internal static class SpriteFontStyleExtensions
    {
        public static string ToTextStyle(this SpriteFontStyle spriteFontStyle)
        {
            switch (spriteFontStyle)
            {
                case SpriteFontStyle.BoldItalic:
                    return "Bold, Italic";

                default:
                    return spriteFontStyle.ToString();
            }
        }
    }

    [HarmonyPatch(typeof(FontDescriptionProcessor))]
    internal class FontDescriptionProcessorPatch
    {
        [HarmonyPatch(nameof(FindFont)), HarmonyPostfix]
        public static void FindFont(FontDescriptionProcessor __instance, ref string __result, string name, string style)
        {
            if (!string.IsNullOrWhiteSpace(__result))
            {
                return;
            }

            __result = string.Empty;
        }
    }

    [HarmonyPatch(typeof(PipelineManager))]
    internal class PipelineManagerPatch
    {
        private static Type ImporterInfo { get; }

        private static FieldInfo ImporterInfo_attribute { get; }

        private static FieldInfo ImporterInfo_type { get; }

        private static FieldInfo ImporterInfo_assemblyTimestamp { get; }

        private static Type ProcessorInfo { get; }

        private static FieldInfo ProcessorInfo_attribute { get; }

        private static FieldInfo ProcessorInfo_type { get; }

        private static FieldInfo ProcessorInfo_assemblyTimestamp { get; }

        private static FieldInfo PipelineManager__importers { get; set; }

        private static MethodInfo PipelineManager__importers_Add { get; set; }

        private static FieldInfo PipelineManager__processors { get; set; }

        private static MethodInfo PipelineManager__processors_Add { get; set; }

        static PipelineManagerPatch()
        {
            var nestedTypes = typeof(PipelineManager).GetNestedTypes(BindingFlags.NonPublic);

            ImporterInfo = nestedTypes.FirstOrDefault(
                t => string.Equals(nameof(ImporterInfo), t.Name, StringComparison.Ordinal)
            );

            ImporterInfo_attribute = AccessTools.DeclaredField(ImporterInfo, "attribute");
            ImporterInfo_type = AccessTools.DeclaredField(ImporterInfo, "type");
            ImporterInfo_assemblyTimestamp = AccessTools.DeclaredField(ImporterInfo, "assemblyTimestamp");

            ProcessorInfo = nestedTypes.FirstOrDefault(
                t => string.Equals(nameof(ProcessorInfo), t.Name, StringComparison.Ordinal)
            );

            ProcessorInfo_attribute = AccessTools.DeclaredField(ProcessorInfo, "attribute");
            ProcessorInfo_type = AccessTools.DeclaredField(ProcessorInfo, "type");
            ProcessorInfo_assemblyTimestamp = AccessTools.DeclaredField(ProcessorInfo, "assemblyTimestamp");

            PipelineManager__importers = AccessTools.DeclaredField(typeof(PipelineManager), "_importers");
            PipelineManager__importers_Add = AccessTools.DeclaredMethod(PipelineManager__importers.FieldType, "Add");
            PipelineManager__processors = AccessTools.DeclaredField(typeof(PipelineManager), "_processors");
            PipelineManager__processors_Add = AccessTools.DeclaredMethod(PipelineManager__processors.FieldType, "Add");
        }

        [HarmonyPatch(nameof(ResolveAssemblies)), HarmonyPostfix]
        public static void ResolveAssemblies(PipelineManager __instance)
        {
            var importers = PipelineManager__importers.GetValue(__instance);
            PipelineManager__importers_Add.Invoke(
                importers, new object[] {CreateImporterInfo(typeof(FontDescriptionImporter))}
            );

            var processors = PipelineManager__processors.GetValue(__instance);
            PipelineManager__processors_Add.Invoke(
                processors, new object[] {CreateProcessorInfo(typeof(FontDescriptionProcessor))}
            );
        }

        private static object CreateImporterInfo(Type importerType)
        {
            var info = Activator.CreateInstance(ImporterInfo);
            var attribute = importerType.GetCustomAttribute<ContentImporterAttribute>();
            ImporterInfo_attribute.SetValue(info, attribute);
            ImporterInfo_type.SetValue(info, importerType);
            ImporterInfo_assemblyTimestamp.SetValue(info, DateTime.Now);

            return info;
        }

        private static object CreateProcessorInfo(Type processorType)
        {
            var info = Activator.CreateInstance(ProcessorInfo);
            var attribute = processorType.GetCustomAttribute<ContentProcessorAttribute>();
            ProcessorInfo_attribute.SetValue(info, attribute);
            ProcessorInfo_type.SetValue(info, processorType);
            ProcessorInfo_assemblyTimestamp.SetValue(info, DateTime.Now);

            return info;
        }
    }
}
