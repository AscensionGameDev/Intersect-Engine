using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Builder.Convertors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Framework.Content.Pipeline.Builder;

namespace Intersect.Client.MonoGame.Content
{
    internal class MonoPipelineManager : PipelineManager
    {
        internal static void Patch(Harmony harmony)
        {
            harmony.Patch(
                MethodInfoResolveAssemblies,
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(MonoPipelineManager), nameof(ResolveAssembliesPostfix)))
            );
        }

        private static readonly FieldInfo FieldInfoImporters =
            AccessTools.DeclaredField(typeof(PipelineManager), "_importers");

        private static readonly FieldInfo FieldInfoProcessors =
            AccessTools.DeclaredField(typeof(PipelineManager), "_processors");

        private static readonly MethodInfo MethodInfoResolveAssemblies =
            AccessTools.DeclaredMethod(typeof(PipelineManager), "ResolveAssemblies");

        protected virtual void ResolveAssembliesPostfix()
        {
            Debugger.Break();
        }

        /// <inheritdoc />
        public MonoPipelineManager(string projectDir, string outputDir, string intermediateDir) : base(projectDir, outputDir, intermediateDir)
        {
        }
    }
}
