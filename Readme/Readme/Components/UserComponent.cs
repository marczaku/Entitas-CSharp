using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, ContextComponent]
public sealed class UserComponent : IComponent {

    public string name;
    public int age;
}
