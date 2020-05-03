using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentUniqueContextApiGenerator : AbstractGenerator {

        public override string name { get { return "Component (Unique Context API)"; } }

        const string STANDARD_TEMPLATE =
@"public partial class ${ContextName}Context {

    public ${ContextName}Entity ${componentName}Entity { get { return GetGroup(${ContextName}Matcher.${ComponentName}).GetSingleEntity(); } }
    public ${ComponentType} ${componentName} { get { return (${ComponentType})${componentName}Entity.GetComponent(${Index}); } }
    public bool has${ComponentName} { get { return ${componentName}Entity != null; } }

    public void Set${ComponentName}(${ContextName}Entity entity, ${newMethodParameters}) {
        if (has${ComponentName}) {
            throw new Entitas.EntitasException(""Could not set ${ComponentName}!\n"" + this + "" already has an entity with ${ComponentType}!"",
                ""You should check if the context already has a ${componentName}Entity before setting it or use context.Replace${ComponentName}()."");
        }
        var index = ${Index};
        var component = entity.CreateComponent<${ComponentType}>(index);
${memberAssignmentList}
        entity.AddComponent(index, component);
    }

	public ${ContextName}Entity Create${ComponentName}(${newMethodParameters}){
		if (has${ComponentName}) {
            throw new Entitas.EntitasException(""Could not create ${ComponentName}!\n"" + this + "" already has an entity with ${ComponentType}!"",
                ""You should check if the context already has a ${componentName}Entity before creating it or use context.Replace${ComponentName}()."");
        }
		var entity = CreateEntity();
		Set${ComponentName}(entity, ${newMethodArgs});
		return entity;
	}

    public void Replace${ComponentName}(${ContextName}Entity entity, ${newMethodParameters}) {
        var oldEntity = ${componentName}Entity;
        if (oldEntity != null) {
            Unset${ComponentName}();
        }
        Set${ComponentName}(entity, ${newMethodArgs});
    }

    public ${ContextName}Entity CreateOrReplace${ComponentName}(${newMethodParameters}) {
        var oldEntity = ${componentName}Entity;
        if (oldEntity != null) {
            Replace${ComponentName}(oldEntity, ${newMethodArgs});
			return oldEntity;
        } else {
			return Create${ComponentName}(${newMethodArgs});
		}
    }

    public void Unset${ComponentName}() {
        ${componentName}Entity.RemoveComponent(${Index});
    }

    public void TryUnset${ComponentName}() {
        if (has${ComponentName}) {
            Unset${ComponentName}();
        }
    }

    public void Destroy${ComponentName}() {
        ${componentName}Entity.Destroy();
    }

    public void TryDestroy${ComponentName}() {
		var entity = ${componentName}Entity;
		if(entity != null)
			entity.Destroy();
    }
}
";
        
        const string FLAG_TEMPLATE =
@"public partial class ${ContextName}Context {

    public ${ContextName}Entity ${componentName}Entity { get { return GetGroup(${ContextName}Matcher.${ComponentName}).GetSingleEntity(); } }
    public bool has${ComponentName} { get { return ${componentName}Entity != null; } }

    public void Set${ComponentName}(${ContextName}Entity entity) {
        if (has${ComponentName}) {
            throw new Entitas.EntitasException(""Could not set ${ComponentName}!\n"" + this + "" already has an entity with ${ComponentType}!"",
                ""You should check if the context already has a ${componentName}Entity before setting it or use context.Replace${ComponentName}()."");
        }
        var index = ${Index};
        var component = entity.CreateComponent<${ComponentType}>(index);
        entity.AddComponent(index, component);
    }

	public ${ContextName}Entity Create${ComponentName}(){
		if (has${ComponentName}) {
            throw new Entitas.EntitasException(""Could not create ${ComponentName}!\n"" + this + "" already has an entity with ${ComponentType}!"",
                ""You should check if the context already has a ${componentName}Entity before creating it or use context.Replace${ComponentName}()."");
        }
		var entity = CreateEntity();
		Set${ComponentName}(entity);
		return entity;
	}

    public ${ContextName}Entity CreateOrReplace${ComponentName}() {
        var oldEntity = ${componentName}Entity;
        if (oldEntity != null) {
            Replace${ComponentName}(oldEntity);
			return oldEntity;
        } else {
			return Create${ComponentName}();
		}
    }

    public void Replace${ComponentName}(${ContextName}Entity entity) {
        TryUnset${ComponentName}();
        Set${ComponentName}(entity);
    }

    public void Unset${ComponentName}() {
        ${componentName}Entity.RemoveComponent(${Index});
    }

    public void TryUnset${ComponentName}() {
        if (has${ComponentName}) {
            Unset${ComponentName}();
        }
    }

    public void Destroy${ComponentName}() {
        ${componentName}Entity.Destroy();
    }

    public void TryDestroy${ComponentName}() {
		var entity = ${componentName}Entity;
		if(entity != null)
			entity.Destroy();
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .Where(d => d.IsUnique())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generate(contextName, data))
                .ToArray();
        }

        CodeGenFile generate(string contextName, ComponentData data) {
            var template = data.GetMemberData().Length == 0
                ? FLAG_TEMPLATE
                : STANDARD_TEMPLATE;

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                data.ComponentNameWithContext(contextName).AddComponentSuffix() + ".cs",
                template.Replace(data, contextName)
                    .Replace("${memberAssignmentList}", getMemberAssignmentList(data.GetMemberData())),
                GetType().FullName
            );
        }
        
        string getMemberAssignmentList(MemberData[] memberData) {
            return string.Join("\n", memberData
                .Select(info => "        component." + info.name + " = new" + info.name.UppercaseFirst() + ";")
                .ToArray()
            );
        }
    }
}
