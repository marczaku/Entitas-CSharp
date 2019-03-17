using System.IO;
using System.Linq;

namespace Entitas.CodeGeneration.Plugins {

    public class EntityGenerator : ICodeGenerator {

        public string name { get { return "Entity"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        const string ENTITY_TEMPLATE =
@"public sealed partial class ${ContextName}Entity : Entitas.Entity {
public ${ContextName}EntityId Id { get; private set; }

    protected override void SetCreationIndex(int creationIndex) {
        base.SetCreationIndex(creationIndex);
        this.Id = new ${ContextName}EntityId(creationIndex);
    }
}

[System.Serializable]
public struct ${ContextName}EntityId : System.IEquatable<${ContextName}EntityId> {

    public int value;

    public ${ContextName}EntityId(int value) {
        this.value = value;
    }

    public static bool operator !=(${ContextName}EntityId a, ${ContextName}EntityId b) {
        return a.value != b.value;
    }

    public static bool operator ==(${ContextName}EntityId a, ${ContextName}EntityId b) {
        return a.value == b.value;
    }

    public bool Equals(${ContextName}EntityId other) {
        return other.value == this.value;
    }

    public override bool Equals(object obj) {
        var other = (${ContextName}EntityId)obj;
        return other.value == this.value;
    }

    public override int GetHashCode() {
        return this.value.GetHashCode();
    }

    public static ${ContextName}EntityId Empty {
        get {
            return new ${ContextName}EntityId { value = 0 };
        }
    }

    public override string ToString() {
        return ""${ContextName}Entity #""+ this.value;
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(d => generateEntityClass(d))
                .ToArray();
        }

        CodeGenFile generateEntityClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Entity.cs",
                ENTITY_TEMPLATE.Replace("${ContextName}", contextName),
                GetType().FullName
            );
        }
    }
}
