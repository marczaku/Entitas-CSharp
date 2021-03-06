using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentUniqueEntityApiGenerator : AbstractGenerator {

        public override string name { get { return "Component (Unique Entity API)"; } }

        const string STANDARD_TEMPLATE =
@"public partial class ${ContextName}Entity {

    public ${ComponentType} ${componentName} { get { return (${ComponentType})GetComponent(${Index}); } }
    public bool has${ComponentName} { get { return HasComponent(${Index}); } }

    public void Remove${ComponentName}() {
        RemoveComponent(${Index});
    }

    public void TryRemove${ComponentName}() {
		if(HasComponent(${Index}))
			RemoveComponent(${Index});
    }
}

public partial interface I${ComponentName}Entity {
    ${ComponentType} ${validComponentName} { get; }
    bool has${ComponentName} { get; }
}

public partial class CreateCollector${EntityType} {
    public ${ComponentType} ${validComponentName} { get { this.matchers.Add(${ContextName}Matcher.${ComponentName}); return new ${ComponentType}(); } }
    public bool has${ComponentName} { get { this.matchers.Add(${ContextName}Matcher.${ComponentName}); return true; } }
}
";
        
        const string FLAG_TEMPLATE =
@"public partial class ${ContextName}Entity {

    static readonly ${ComponentType} ${componentName}Component = new ${ComponentType}();

    public bool ${prefixedComponentName} {
        get { return HasComponent(${Index}); }
    }
}

public partial interface I${ComponentName}Entity {
    bool ${prefixedComponentName} { get; }
}

public partial class CreateCollector${EntityType} {
    public bool ${prefixedComponentName} {
        get { this.matchers.Add(${ContextName}Matcher.${ComponentName}); return true; }
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
                template.Replace(data, contextName),
                GetType().FullName
            );
        }
    }
}
