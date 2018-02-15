using Entitas.Utils;
using System.IO;
using System.Linq;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextMatcherGenerator : ICodeGenerator {

        public string name { get { return "Context (Matcher API)"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        const string CONTEXT_TEMPLATE =
@"public sealed partial class ${ContextName}Matcher {

    public static Entitas.IAllOfMatcher<${ContextName}Entity> AllOf(params int[] indices) {
        return Entitas.Matcher<${ContextName}Entity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<${ContextName}Entity> AllOf(params Entitas.IMatcher<${ContextName}Entity>[] matchers) {
          return Entitas.Matcher<${ContextName}Entity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<${ContextName}Entity> AnyOf(params int[] indices) {
          return Entitas.Matcher<${ContextName}Entity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<${ContextName}Entity> AnyOf(params Entitas.IMatcher<${ContextName}Entity>[] matchers) {
          return Entitas.Matcher<${ContextName}Entity>.AnyOf(matchers);
    }

    static Entitas.IMatcher<${ContextName}Entity> CreateMatcher(int componentIndex) {
        var matcher = (Entitas.Matcher<${ContextName}Entity>)Entitas.Matcher<${ContextName}Entity>.AllOf(componentIndex);
        matcher.componentNames = ${ContextName}ComponentsLookup.componentNames;
        return matcher;
    }

    static Entitas.IMatcher<${ContextName}Entity>[] _${contextName}Matchers;
    public static Entitas.IMatcher<${ContextName}Entity>[] ${contextName}Matchers {
        get {
            if (_${contextName}Matchers == null) {
                _${contextName}Matchers = new Entitas.IMatcher<${ContextName}Entity>[${ContextName}ComponentsLookup.TotalComponents];
                for (int i = 0; i < ${ContextName}ComponentsLookup.TotalComponents; ++i) {
                    _${contextName}Matchers[i] = CreateMatcher(i);
                }
            }
            return _${contextName}Matchers;
        }
    }

	public static Entitas.IMatcher<${ContextName}Entity> Get(int componentIndex) {
		return _${contextName}Matchers[componentIndex];
	}

	public static Entitas.IMatcher<${ContextName}Entity> Get<TComponent>() where TComponent : Entitas.IComponent {
		var componentIndex = ${ContextName}ComponentsLookup.componentTypeIndices[typeof(TComponent)];
		return _${contextName}Matchers[componentIndex];
	}

}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(d => generateContextClass(d))
                .ToArray();
        }

        CodeGenFile generateContextClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Matcher.cs",
                CONTEXT_TEMPLATE
                    .Replace("${ContextName}", contextName)
					.Replace("${contextName}", contextName.LowercaseFirst()),
				GetType().FullName
            );
        }
    }
}
