using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), ContextComponent, FlagPrefix("My")]
public sealed class CustomPrefixFlagComponent : IComponent {
}
