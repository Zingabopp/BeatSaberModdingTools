using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace BeatSaberModdingTools.BuildTools
{
    [Flags]
    public enum BuildToolsParameters
    {
        None = 0 << 0,
        Virtualize = 1 << 0,
        NoStrip = 2 << 1
    }

    public class Virtualizer
    {
        public void thing(string inputFile, string destinationFile, BuildToolsParameters parameters, ReaderParameters readerParameters)
        {
            var errorStrength = file.optional ? "warning" : "error";
            string fname = null;
            try
            {
                if (file.file[0] == '"') continue;

                var fparts = file.file.Split('?');
                fname = fparts[0];

                if (fname == "") continue;

                var outp = Path.Combine(directoryName ?? throw new InvalidOperationException(),
                    Path.GetFileName(fname) ?? throw new InvalidOperationException());

                var aliasp = fparts.FirstOrDefault(s => s.StartsWith("alias="))?.Substring("alias=".Length);
                if (aliasp != null)
                    outp = Path.Combine(directoryName, aliasp);

                if (fparts.Contains("optional"))
                    errorStrength = "warning";

                bool stripDll = !parameters.HasFlag(BuildToolsParameters.NoStrip);
                bool virtualize = parameters.HasFlag(BuildToolsParameters.Virtualize);

                Console.WriteLine($"Copying \"{fname}\" to \"{outp}\"");
                if (File.Exists(outp)) File.Delete(outp);

                if (Path.GetExtension(fname)?.ToLower() == ".dll")
                {
                    try
                    {
                        if (fparts.Contains("native"))
                            goto copy;
                        else
                        {
                            if (stripDll)
                            {
                                var resolver = new DefaultAssemblyResolver();
                                resolver.AddSearchDirectory(Path.GetDirectoryName(fname));
                                foreach (var path in resolveIn)
                                    resolver.AddSearchDirectory(path);
                                var parameters = new ReaderParameters
                                {
                                    AssemblyResolver = resolver,
                                    ReadWrite = false,
                                    ReadingMode = ReadingMode.Immediate,
                                    InMemory = true
                                };

                                using var modl = ModuleDefinition.ReadModule(fparts[0], parameters);
                                var virtualize = fparts.Contains("virt");

                                foreach (var t in modl.Types)
                                {
                                    void Clear(TypeDefinition type)
                                    {
                                        foreach (var m in type.Methods)
                                        {
                                            if (m.Body != null)
                                            {
                                                m.Body.Instructions.Clear();
                                                m.Body.InitLocals = false;
                                                m.Body.Variables.Clear();
                                            }
                                        }
                                        foreach (var ty in type.NestedTypes)
                                        {
                                            Clear(ty);
                                        }
                                    }
                                    if (virtualize)
                                        VirtualizedModule.VirtualizeType(t);
                                    Clear(t);
                                }

                                modl.Write(outp);

                                return;
                            }
                            else if (fparts.Contains("virt"))
                            {
                                using var module = VirtualizedModule.Load(fname);
                                module.Virtualize(outp);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{Path.Combine(Environment.CurrentDirectory, args[0])}({file.line}): warning: {e}");
                    }
                }

            copy:
                File.Copy(fname, outp);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"{Path.Combine(Environment.CurrentDirectory, args[0])}({file.line}): {errorStrength}: \"{file.file}\" {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Path.Combine(Environment.CurrentDirectory, args[0])}({file.line}): {errorStrength}: {e}");
            }
        }
    }
    public class VirtualizedModule : IDisposable
    {
        private readonly FileInfo file;
        private ModuleDefinition module;

        public static VirtualizedModule Load(string engineFile)
        {
            return new VirtualizedModule(engineFile);
        }

        private VirtualizedModule(string assemblyFile)
        {
            file = new FileInfo(assemblyFile);

            LoadModules();
        }

        private void LoadModules()
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(file.Directory?.FullName);

            var parameters = new ReaderParameters
            {
                AssemblyResolver = resolver,
                ReadWrite = false,
                ReadingMode = ReadingMode.Immediate,
                InMemory = true
            };

            module = ModuleDefinition.ReadModule(file.FullName, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFile"></param>
        public void Virtualize(string targetFile)
        {

            foreach (var type in module.Types)
            {
                VirtualizeType(type);
            }

            module.Write(targetFile);
        }

        internal static void VirtualizeType(TypeDefinition type)
        {
            if (type.IsSealed)
            {
                // Unseal
                type.IsSealed = false;
            }

            if (type.IsInterface) return;
            if (type.IsAbstract) return;

            // These two don't seem to work.
            if (type.Name == "SceneControl" || type.Name == "ConfigUI") return;

            // Take care of sub types
            foreach (var subType in type.NestedTypes)
            {
                VirtualizeType(subType);
            }

            foreach (var method in type.Methods)
            {
                if (method.IsManaged
                    && method.IsIL
                    && !method.IsStatic
                    && !method.IsVirtual
                    && !method.IsAbstract
                    && !method.IsAddOn
                    && !method.IsConstructor
                    && !method.IsSpecialName
                    && !method.IsGenericInstance
                    && !method.HasOverrides)
                {
                    method.IsVirtual = true;
                    method.IsPublic = true;
                    method.IsPrivate = false;
                    method.IsNewSlot = true;
                    method.IsHideBySig = true;
                }
            }

            foreach (var field in type.Fields)
            {
                if (field.IsPrivate) field.IsFamily = true;
            }
        }

        public void Dispose() => module?.Dispose();
    }

    public static class VirtualizerExtensions
    {
        public static TypeDefinition Clear(this TypeDefinition type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            foreach (var m in type.Methods)
            {
                if (m.Body != null)
                {
                    m.Body.Instructions.Clear();
                    m.Body.InitLocals = false;
                    m.Body.Variables.Clear();
                }
            }
            foreach (var ty in type.NestedTypes)
            {
                Clear(ty);
            }
            return type;
        }
    }
}
