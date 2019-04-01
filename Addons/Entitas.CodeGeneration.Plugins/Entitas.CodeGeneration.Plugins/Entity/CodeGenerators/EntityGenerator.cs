using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EntityGenerator : ICodeGenerator {

        public string name { get { return "Entity"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string TEMPLATE =
            @"public sealed partial class ${EntityType} : Entitas.IdEntity<${EntityType}Id> {
    protected override void SetCreationIndex(int creationIndex) {
        base.SetCreationIndex(creationIndex);
        this.id = new ${ContextName}EntityId(creationIndex);
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
                .Select(generate)
                .ToArray();
        }

        CodeGenFile generate(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                contextName.AddEntitySuffix() + ".cs",
                TEMPLATE.Replace(contextName),
                GetType().FullName
            );
        }
    }
}
