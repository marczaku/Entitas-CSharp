using Entitas;
using Entitas.CodeGeneration.Attributes;

[GameState, ContextComponent]
public sealed class ScoreComponent : IComponent {

    public int value;
}
