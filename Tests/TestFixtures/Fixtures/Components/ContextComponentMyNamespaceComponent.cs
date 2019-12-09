using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace My.Namespace {

    [Context("Test"), ContextComponent]
    public sealed class ContextComponentMyNamespaceComponent : IComponent {
        public string value;
    }
}
