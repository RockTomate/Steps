using System.IO;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;
using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Compile DLL", "Compiles C# scripts into DLL", StepCategories.BuildingCategory)]
    public class CompileDllStep : SimpleStep
    {
        private const string AssemblyInfoCategory = "Assembly Info";
        private const string ReferencesCategory = "References";
        private const string CompileSymbolsCategory = "Compile Symbols";
        private const string ArgumentsCategory = "Arguments";

        private const string TempResponseFileName = "temp_response_file.rsp";
        private const string TempAssemblyInfoFileName = "AssemblyInfo.cs";

        private static readonly string BaseUnityDllDirectoryPath = Path.GetDirectoryName(EditorApplication.applicationPath);
        private static Dictionary<string, string> _customReferences;

        private static Dictionary<string, string> CustomReferences
        {
            get
            {
                if (_customReferences == null)
                    _customReferences = CustomReferencePaths.ToDictionary(Path.GetFileName, x => x);

                return _customReferences;
            }
        }

        private static readonly List<string> CustomReferencePaths = new List<string>
        {
            PathHelpers.Combine(BaseUnityDllDirectoryPath, "Data\\Managed\\UnityEngine.dll"),
            PathHelpers.Combine(BaseUnityDllDirectoryPath, "Data\\Managed\\UnityEditor.dll"),
            PathHelpers.Combine(BaseUnityDllDirectoryPath, "Data\\Managed\\UnityEditor.Graphs.dll"),
        };

        // General ==========================================================================================================

        [InputField(tooltip: "File path to where new DLL will be exported to", required: true)]
        public string OutputPath;

        [InputField(tooltip: "If true, any warnings will cause compilation error")]
        public bool TreatWarningsAsErrors = false;

        [InputField(tooltip: "Scripts that will be compiled into a DLL", required: true)]
        public string[] ScriptPaths;

        [InputField(tooltip: "Paths to resources that will be embedded into DLL")]
        public string[] Resources;


        // Assembly Info ====================================================================================================

        [InputField("Generate", category: AssemblyInfoCategory, 
            tooltip:"Specify whether or not we want to generate assembly info for this DLL.\n\n" +
                    "NOTE: Keep this option disabled if you plan to include an existing assembly info file, otherwise you'll end up with compilation errors!")]
        public bool IncludeAssemblyInfo = false;

        [InputField(category: AssemblyInfoCategory,
            tooltip: "Specify the version the DLL." +
                     "\n\n[assembly: AssemblyVersion(\"<major>.<minor>.<revision>\")]")]
        public string AssemblyVersion = "0.0.0";

        [InputField(category: AssemblyInfoCategory,
            tooltip: "Value specifying the Win32 file version number. This normally defaults to the assembly version." +
                     "\n\n[assembly: AssemblyFileVersion(\"<major>.<minor>.<revision>\")]")]
        public string FileVersion = "0.0.0";

        [InputField(category: AssemblyInfoCategory, name: "Description",
            tooltip: "A short description that summarizes the nature and purpose of the assembly." +
                     "\n\n[assembly: AssemblyDescription(\"<description>\")]")]
        public string AssemblyDescription;

        [InputField(category: AssemblyInfoCategory,
            tooltip: "Value specifying a company name." +
                     "\n\n[assembly: AssemblyCompany(\"<company>\")]")]
        public string Company = "Default";

        [InputField(category: AssemblyInfoCategory,
            tooltip: "Value specifying copyright information." +
                     "\n\n[assembly: AssemblyCopyright(\"<copyright>\")]")]
        public string Copyright;

        [InputField(category: AssemblyInfoCategory,
            tooltip: "Value specifying trademark information." +
                     "\n\n[assembly: AssemblyTrademark(\"<trademark>\")]")]
        public string Trademark;

        [InputField(category: AssemblyInfoCategory,
            tooltip: "Value specifying product information." +
                     "\n\n[assembly: AssemblyProduct(\"<product>\")]")]
        public string Product;


        // References =======================================================================================================

        [InputField("Add \"UnityEngine\"", "Whether or not \"UnityEngine.dll\" should be included as a reference for this DLL", category: ReferencesCategory)]
        public bool IncludeUnityEngineDll;

        [InputField("Add \"UnityEditor\"", "Whether or not \"UnityEditor.dll\" should be included as a reference for this DLL", category: ReferencesCategory)]
        public bool IncludeUnityEditorDll;

        [InputField("Add \"UnityEditor.Graphs\"", "Whether or not \"UnityEditor.Graphs.dll\" should be included as a reference for this DLL", category: ReferencesCategory)]
        public bool IncludeUnityGraphsDll;

        [InputField(tooltip: "Paths to references", category: ReferencesCategory)]
        public string[] AdditionalReferences;


        // Compile Symbols ==================================================================================================

        [InputField("Additional Compile Symbols", category: CompileSymbolsCategory)]
        public string[] CompileSymbols;

        [InputField(tooltip: "Specify from which build target should compile symbols be copied", category: CompileSymbolsCategory)]
        public BuildTarget BuildTargetCompileSymbols = BuildTarget.NoTarget;


        // Arguments ========================================================================================================

        [InputField(tooltip: "Whether or not debug information should be included in the DLL", category: ArgumentsCategory)]
        public bool IncludeDebugInfo = false;

        [InputField(tooltip: "Whether DLL should be optimized", category: ArgumentsCategory)]
        public bool Optimize = true;

        [InputField(tooltip: "Whether unsafe code should be allowed", category: ArgumentsCategory)]
        public bool Unsafe = false;

        [InputField(tooltip: "Additional compile arguments", category: ArgumentsCategory)]
        public string[] AdditionalArguments;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var codeProvider = new CSharpCodeProvider();
            var rspFilePath = MakeResponseFile();
            var compilerParams = MakeCompilerParameters(rspFilePath);

            // create a missing directory path
            var directoryPath = PathHelpers.GetDirectoryName(OutputPath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            // compile the DLL then dispose of code provider
            var compilerResults = codeProvider.CompileAssemblyFromFile(compilerParams);
            codeProvider.Dispose();

            if (!compilerResults.Errors.HasErrors)
                return true;

            foreach (CompilerError compilerError in compilerResults.Errors)
            {
                RockLog.WriteLine(this, compilerError.IsWarning ? LogTier.Warning : LogTier.Error, compilerError.ToString());
            }

            return false;
        }

        /// <summary>
        /// Creates a <see cref="CompilerParameters"/> object based on settings provided.
        /// </summary>
        /// <param name="rspFilePath">Path to an rsp file.</param>
        /// <returns>Returns compiler parameters.</returns>
        private CompilerParameters MakeCompilerParameters(string rspFilePath)
        {
            // create compile parameters and give it base values
            var compilerParams = new CompilerParameters
            {
                GenerateExecutable = false,
                TreatWarningsAsErrors = TreatWarningsAsErrors,
                OutputAssembly = OutputPath,
                IncludeDebugInformation = IncludeDebugInfo,
                GenerateInMemory = true,
                CompilerOptions = string.Format("@\"{0}\"", rspFilePath),
            };

            return compilerParams;
        }

        /// <summary>
        /// Creates a temporary response file based on settings
        /// </summary>
        /// <returns>Path to a response file</returns>
        private string MakeResponseFile()
        {
            var responseFileString = new StringBuilder();
            var compileSymbols = string.Join(";", CompileSymbols);
            var rspFilePath = PathHelpers.Combine(MakeTempDir(), TempResponseFileName);

            // append arguments
            if (BuildTargetCompileSymbols != BuildTarget.NoTarget)
            {
                var buildTargetCompileSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(BuildTargetCompileSymbols));
                if (!string.IsNullOrEmpty(buildTargetCompileSymbols))
                    compileSymbols += ";" + buildTargetCompileSymbols;
            }

            responseFileString.AppendLine(string.Format("-out:\"{0}\"", OutputPath));
            responseFileString.AppendLine("-target:library");

            if (compileSymbols.Length > 0)
                responseFileString.AppendLine(string.Format("-define:\"{0}\"", compileSymbols));

            if (Optimize)
                responseFileString.AppendLine("-optimize");

            if (Unsafe)
                responseFileString.AppendLine("-unsafe");

            // append additional arguments
            foreach (var additionalArgument in AdditionalArguments)
            {
                responseFileString.AppendLine(additionalArgument);
            }

            // append resources
            if (Resources.Length > 0)
            {
                responseFileString.AppendLine();

                foreach (var resource in Resources)
                {
                    responseFileString.AppendLine(string.Format("-resource:\"{0}\"", resource));
                }
            }

            responseFileString.AppendLine();

            // append base references
            responseFileString.AppendLine("-reference:\"System.dll\"");
            responseFileString.AppendLine("-reference:\"System.Core.dll\"");

            if (IncludeUnityEngineDll)
                responseFileString.AppendLine(string.Format("-reference:\"{0}\"", CustomReferences["UnityEngine.dll"]));

            if (IncludeUnityEditorDll)
                responseFileString.AppendLine(string.Format("-reference:\"{0}\"", CustomReferences["UnityEditor.dll"]));

            if (IncludeUnityGraphsDll)
                responseFileString.AppendLine(string.Format("-reference:\"{0}\"", CustomReferences["UnityEditor.Graphs.dll"]));

            // append additional references
            foreach (var additionalReference in AdditionalReferences)
            {
                responseFileString.AppendLine(string.Format("-reference:\"{0}\"", additionalReference));
            }

            responseFileString.AppendLine();

            // append assembly info script file
            if (IncludeAssemblyInfo)
                responseFileString.AppendLine(string.Format("\"{0}\"", MakeAssemblyInfoFile()));

            // append all script file paths
            foreach (var scriptPath in ScriptPaths)
            {
                responseFileString.AppendLine(string.Format("\"{0}\"", scriptPath));
            }

            File.WriteAllText(rspFilePath, responseFileString.ToString(), Encoding.UTF8);
            return rspFilePath;
        }

        /// <summary>
        /// Generates a temporary assembly info script that will be compiled with the DLL.
        /// </summary>
        /// <returns>Path to an assembly info file.</returns>
        private string MakeAssemblyInfoFile()
        {
            var assemblyInfoString = new StringBuilder();
            var assemblyInfoFilePath = PathHelpers.Combine(MakeTempDir(), TempAssemblyInfoFileName);

            // append required namespaces
            assemblyInfoString.AppendLine("using System.Reflection;");
            assemblyInfoString.AppendLine();

            // append required description attributes

            if (!Company.IsNullOrEmpty())
                assemblyInfoString.AppendLine(string.Format("[assembly: AssemblyCompany(\"{0}\")]", Company));

            if (!Product.IsNullOrEmpty())
                assemblyInfoString.AppendLine(string.Format("[assembly: AssemblyProduct(\"{0}\")]", Product));

            if (!AssemblyDescription.IsNullOrEmpty())
                assemblyInfoString.AppendLine(string.Format("[assembly: AssemblyDescription(\"{0}\")]", AssemblyDescription));

            if (!Copyright.IsNullOrEmpty())
                assemblyInfoString.AppendLine(string.Format("[assembly: AssemblyCopyright(\"{0}\")]", Copyright));

            if (!Trademark.IsNullOrEmpty())
                assemblyInfoString.AppendLine(string.Format("[assembly: AssemblyTrademark(\"{0}\")]", Trademark));

            if (!AssemblyVersion.IsNullOrEmpty())
                assemblyInfoString.AppendLine(string.Format("[assembly: AssemblyVersion(\"{0}\")]", AssemblyVersion));

            if (!FileVersion.IsNullOrEmpty())
                assemblyInfoString.AppendLine(string.Format("[assembly: AssemblyFileVersion(\"{0}\")]", FileVersion));

            File.WriteAllText(assemblyInfoFilePath, assemblyInfoString.ToString(), Encoding.UTF8);
            return assemblyInfoFilePath;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Compiling"; }
        }
    }
}