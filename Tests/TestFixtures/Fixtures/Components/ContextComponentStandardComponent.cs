using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), ContextComponent]
public sealed class ContextComponentStandardComponent : IComponent {
    public string value;
}
