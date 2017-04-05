using System;
using System.IO;

namespace Entitas.CodeGenerator {

    public class WriteToCsProjPostprocessor : ICodeGenFilePostProcessor {

        public string name { get { return "Write to CsProj"; } }
        public int priority { get { return 100; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return false; } }

        readonly string _csProjPath;
        readonly string _directory;
        const string ENTITAS_INCLUDE_BEGIN = "<!-- Entitas Include Begin -->";
        const string ENTITAS_INCLUDE_END = "<!-- Entitas Include End -->";

        public WriteToCsProjPostprocessor() : this(new CodeGeneratorConfig(Preferences.LoadConfig()).projectPath, new CodeGeneratorConfig(Preferences.LoadConfig()).targetDirectory) {
        }

        public WriteToCsProjPostprocessor(string csProjPath, string directory) {
            this._csProjPath = csProjPath;
            this._directory = getSafeDir(directory);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            if(!File.Exists(this._csProjPath))
                return files;

            string csProj = File.ReadAllText(this._csProjPath);
            string oldItemGroup = getItemGroup(csProj);

            string fileInclude = string.Empty;
            Uri csProjFullUri = new Uri(Path.GetFullPath(Path.GetDirectoryName(this._csProjPath)) + "/", UriKind.Absolute);

            foreach(var file in files) {
                var fileFullUri = new Uri(Path.GetFullPath(this._directory + file.fileName), UriKind.Absolute);
                var relativeFilePath = csProjFullUri.MakeRelativeUri(fileFullUri).ToString().Replace('/', '\\');
                fileInclude += string.Format("\r\n    <Compile Include=\"{0}\" />", relativeFilePath);
            }

            string itemGroup = string.Format("{0}\r\n  <ItemGroup>{1}\r\n  </ItemGroup>\r\n{2}\r\n", ENTITAS_INCLUDE_BEGIN, fileInclude, ENTITAS_INCLUDE_END);
            if(!itemGroup.Equals(oldItemGroup + "\r\n")) {
                csProj = cleanOldItemGroup(csProj);
                int itemGroupStart = csProj.LastIndexOf("</Project>", StringComparison.Ordinal);
                csProj = csProj.Insert(itemGroupStart, itemGroup);

                File.WriteAllText(this._csProjPath, csProj);
            }
            return files;
        }

        static string getSafeDir(string directory) {
            if(!directory.EndsWith("/", StringComparison.Ordinal)) {
                directory += "/";
            }
            if(!directory.EndsWith("Generated/", StringComparison.Ordinal)) {
                directory += "Generated/";
            }
            return directory;
        }

        string getItemGroup(string fileContet) {
            int start, end;
            if(tryGetEntitasIncludeIndices(fileContet, out start, out end)) {
                return fileContet.Substring(start, end - start + 1);
            }
            return string.Empty;
        }

        bool tryGetEntitasIncludeIndices(string fileContent, out int start, out int end) {
            start = fileContent.IndexOf(ENTITAS_INCLUDE_BEGIN, StringComparison.Ordinal);
            if(start > -1) {
                end = fileContent.IndexOf(ENTITAS_INCLUDE_END, StringComparison.Ordinal) + ENTITAS_INCLUDE_END.Length - 1;
                if(end > -1 && end > start) {
                    return true;
                }
            }
            start = -1;
            end = -1;
            return false;
        }

        string cleanOldItemGroup(string csProjContent) {
            int start, end;
            if(tryGetEntitasIncludeIndices(csProjContent, out start, out end)) {
                csProjContent = csProjContent.Remove(start, end - start + 1);
                if(csProjContent.Substring(start, 1) == "\n")
                    csProjContent = csProjContent.Remove(start, 1);
                else if(csProjContent.Substring(start, 2) == "\r\n")
                    csProjContent = csProjContent.Remove(start, 2);
            }
            return csProjContent;
        }
    }
}