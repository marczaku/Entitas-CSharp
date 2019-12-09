using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), ContextComponent, Event(EventTarget.Any)]
public sealed class ContextComponentEventComponent : IComponent {
    public string value;
}
