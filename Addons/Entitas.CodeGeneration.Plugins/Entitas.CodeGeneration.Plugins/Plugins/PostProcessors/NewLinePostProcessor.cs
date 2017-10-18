using System;

namespace Entitas.CodeGeneration.Plugins {

    public class NewLinePostProcessor : ICodeGenFilePostProcessor {

        public string name { get { return "Convert newlines"; } }
        public int priority { get { return 95; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            foreach(var file in files) {
                file._fileContent = file.fileContent.Replace("\n", "\r\n");
            }

            return files;
        }
    }
}
