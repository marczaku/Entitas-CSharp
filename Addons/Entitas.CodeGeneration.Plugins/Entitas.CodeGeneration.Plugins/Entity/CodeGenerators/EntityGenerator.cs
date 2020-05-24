using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EntityGenerator : ICodeGenerator {

        public string name { get { return "Entity"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string TEMPLATE =
            @"public sealed partial class ${EntityType} : Entitas.IdEntity<${EntityType}Id>, I${EntityType} {
    protected override void SetCreationIndex(int creationIndex) {
        base.SetCreationIndex(creationIndex);
        this.id = new ${ContextName}EntityId(creationIndex);
    }

}

public partial interface I${EntityType} : Entitas.IIdEntity<${EntityType}Id> { }

public partial class CreateCollector${EntityType} : I${EntityType} {
	readonly System.Collections.Generic.HashSet<Entitas.IMatcher<${EntityType}>> matchers = new System.Collections.Generic.HashSet<Entitas.IMatcher<${EntityType}>>();

	public Entitas.IMatcher<${EntityType}> CreateMatcher() {
		return ${ContextName}Matcher.AllOf(System.Linq.Enumerable.ToArray(this.matchers));
	}

    public void Retain(object owner) {
        throw new System.NotImplementedException();
    }

    public void Release(object owner) {
        throw new System.NotImplementedException();
    }

    public int retainCount { get; }
    public void Initialize(int creationIndex, int totalComponents, System.Collections.Generic.Stack<Entitas.IComponent>[] componentPools, Entitas.ContextInfo contextInfo = null, Entitas.IAERC aerc = null) {
        throw new System.NotImplementedException();
    }

    public void Reactivate(int creationIndex) {
        throw new System.NotImplementedException();
    }

    public void AddComponent(int index, Entitas.IComponent component) {
        throw new System.NotImplementedException();
    }

    public void RemoveComponent(int index) {
        throw new System.NotImplementedException();
    }

    public void ReplaceComponent(int index, Entitas.IComponent component) {
        throw new System.NotImplementedException();
    }

    public Entitas.IComponent GetComponent(int index) {
        throw new System.NotImplementedException();
    }

    public Entitas.IComponent[] GetComponents() {
        throw new System.NotImplementedException();
    }

    public int[] GetComponentIndices() {
        throw new System.NotImplementedException();
    }

    public bool HasComponent(int index) {
        throw new System.NotImplementedException();
    }

    public bool HasComponents(int[] indices) {
        throw new System.NotImplementedException();
    }

    public bool HasAnyComponent(int[] indices) {
        throw new System.NotImplementedException();
    }

    public void RemoveAllComponents() {
        throw new System.NotImplementedException();
    }

    public System.Collections.Generic.Stack<Entitas.IComponent> GetComponentPool(int index) {
        throw new System.NotImplementedException();
    }

    public Entitas.IComponent CreateComponent(int index, System.Type type) {
        throw new System.NotImplementedException();
    }

    public T CreateComponent<T>(int index) where T : new() {
        throw new System.NotImplementedException();
    }

    public void Destroy() {
        throw new System.NotImplementedException();
    }

    public void InternalDestroy() {
        throw new System.NotImplementedException();
    }

    public void RemoveAllOnEntityReleasedHandlers() {
        throw new System.NotImplementedException();
    }

    public int totalComponents { get; }
    public int creationIndex { get; }
    public bool isEnabled { get; }
    public System.Collections.Generic.Stack<Entitas.IComponent>[] componentPools { get; }
    public Entitas.ContextInfo contextInfo { get; }
    public Entitas.IAERC aerc { get; }
    public event Entitas.EntityComponentChanged OnComponentAdded {
        add {throw new System.NotImplementedException();}
        remove {throw new System.NotImplementedException();}
    }
    public event Entitas.EntityComponentChanged OnComponentRemoved {
        add {throw new System.NotImplementedException();}
        remove {throw new System.NotImplementedException();}
    }
    public event Entitas.EntityComponentReplaced OnComponentReplaced {
        add {throw new System.NotImplementedException();}
        remove {throw new System.NotImplementedException();}
    }
    public event Entitas.EntityEvent OnEntityReleased {
        add {throw new System.NotImplementedException();}
        remove {throw new System.NotImplementedException();}
    }
    public event Entitas.EntityEvent OnDestroyEntity {
        add {throw new System.NotImplementedException();}
        remove {throw new System.NotImplementedException();}
    }
    public ${EntityType}Id id { get; }
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
