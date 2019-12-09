using Entitas.CodeGeneration.Attributes;

namespace My.Namespace {

    [Context("Test"), Context("Test2"), ContextComponent]
    public sealed class ContextComponentClassToGenerate {
        public string value;
    }
}
