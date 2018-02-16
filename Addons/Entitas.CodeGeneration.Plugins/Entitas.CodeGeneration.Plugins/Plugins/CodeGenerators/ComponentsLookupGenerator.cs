using System.Collections.Generic;
using System.Linq;
using System.IO;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentsLookupGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Components Lookup"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        public const string COMPONENTS_LOOKUP = "ComponentsLookup";

        const string COMPONENTS_LOOKUP_TEMPLATE =
@"public static class ${Lookup} {

${componentConstants}

${totalComponentsConstant}

    public static readonly string[] componentNames = {
${componentNames}
    };

    public static readonly System.Type[] componentTypes = {
${componentTypes}
    };

    public static readonly System.Collections.Generic.Dictionary<System.Type, int> componentTypeIndices = new System.Collections.Generic.Dictionary<System.Type, int>(){
${componentTypeIndices}
    };
}
";

        const string COMPONENT_CONSTANTS_TEMPLATE =
@"    public const int ${ComponentName} = ${Index};";

        const string TOTAL_COMPONENTS_CONSTANT_TEMPLATE =
@"    public const int TotalComponents = ${totalComponents};";

        const string COMPONENT_NAMES_TEMPLATE =
@"        ""${ComponentName}""";

        const string COMPONENT_TYPES_TEMPLATE =
@"        typeof(${ComponentType})";

		const string COMPONENT_TYPE_INDICES_TEMPLATE =
@"        { typeof(${ComponentType}), ${Index} }";

		public void Configure(Properties properties) {
            _ignoreNamespacesConfig.Configure(properties);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var files = new List<CodeGenFile>();

            var contextData = data
                .OfType<ContextData>()
                .ToArray();

            files.AddRange(generateEmptyLookup(contextData));

            var componentData = data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .ToArray();

            var lookup = generateLookup(componentData);
            var fileNames = new HashSet<string>(lookup.Select(file => file.fileName));

            files = files
                .Where(file => !fileNames.Contains(file.fileName))
                .ToList();

            files.AddRange(lookup);

            return files.ToArray();
        }

        CodeGenFile[] generateEmptyLookup(ContextData[] data) {
            var emptyData = new ComponentData[0];
            return data
                .Select(d => generateComponentsLookupClass(d.GetContextName(), emptyData))
                .ToArray();
        }

        CodeGenFile[] generateLookup(ComponentData[] data) {
            var contextNameToComponentData = data
                .Aggregate(new Dictionary<string, List<ComponentData>>(), (dict, d) => {
                    var contextNames = d.GetContextNames();
                    foreach (var contextName in contextNames) {
                        if (!dict.ContainsKey(contextName)) {
                            dict.Add(contextName, new List<ComponentData>());
                        }

                        dict[contextName].Add(d);
                    }

                    return dict;
                });

            foreach (var key in contextNameToComponentData.Keys.ToArray()) {
                contextNameToComponentData[key] = contextNameToComponentData[key]
                    .OrderBy(d => d.GetFullTypeName())
                    .ToList();
            }

            return contextNameToComponentData
                .Select(kv => generateComponentsLookupClass(kv.Key, kv.Value.ToArray()))
                .ToArray();
        }

        CodeGenFile generateComponentsLookupClass(string contextName, ComponentData[] data) {
            var componentConstants = string.Join("\n", data
                .Select((d, index) => {
                    return COMPONENT_CONSTANTS_TEMPLATE
                    .Replace("${ComponentName}", d.GetFullTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces))
                        .Replace("${Index}", index.ToString());
                }).ToArray());

            var totalComponentsConstant = TOTAL_COMPONENTS_CONSTANT_TEMPLATE
                .Replace("${totalComponents}", data.Length.ToString());

            var componentNames = string.Join(",\n", data
                .Select(d => COMPONENT_NAMES_TEMPLATE
                        .Replace("${ComponentName}", d.GetFullTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces))
                ).ToArray());

            var componentTypes = string.Join(",\n", data
                .Select(d => COMPONENT_TYPES_TEMPLATE
                    .Replace("${ComponentType}", d.GetFullTypeName())
                ).ToArray());

			var componentTypeIndices = string.Join(",\n", data
				.Select((d, index) => {
					return COMPONENT_TYPE_INDICES_TEMPLATE
					.Replace("${ComponentType}", d.GetFullTypeName())
						.Replace("${Index}", index.ToString());
				}).ToArray());

			var fileContent = COMPONENTS_LOOKUP_TEMPLATE
                .Replace("${Lookup}", contextName + COMPONENTS_LOOKUP)
                .Replace("${componentConstants}", componentConstants)
                .Replace("${totalComponentsConstant}", totalComponentsConstant)
                .Replace("${componentNames}", componentNames)
                .Replace("${componentTypes}", componentTypes)
				.Replace("${componentTypeIndices}", componentTypeIndices);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + COMPONENTS_LOOKUP + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
