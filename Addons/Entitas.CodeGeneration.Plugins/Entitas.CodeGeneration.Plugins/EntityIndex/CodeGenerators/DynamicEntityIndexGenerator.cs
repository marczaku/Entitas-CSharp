using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class DynamicEntityIndexGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Entity Index"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string CLASS_TEMPLATE =
            @"public partial class Contexts {

${indexConstants}

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeEntityIndices() {
${addIndices}
    }
}";

		const string CONTEXT_CLASS_TEMPLATE =
@"public partial class ${ContextName}Context {
${getIndices}
}";

        const string INDEX_CONSTANTS_TEMPLATE = @"    public const string ${IndexName} = ""${IndexName}"";";

        const string ADD_INDEX_TEMPLATE =
            @"        ${contextName}.AddEntityIndex(new ${IndexType}<${ContextName}Entity, ${KeyType}>(
            ${IndexName},
            ${contextName}.GetGroup(${ContextName}Matcher.${Matcher}),
            (e, c) => ((${ComponentType})c).${MemberName}));";

        const string ADD_CUSTOM_INDEX_TEMPLATE =
            @"        ${contextName}.AddEntityIndex(new ${IndexType}(${contextName}));";

        const string GET_INDEX_TEMPLATE =
            @"    public System.Collections.Generic.HashSet<${ContextName}Entity> GetEntitiesWith${ComponentName}(${KeyType} ${MemberName}) {
        return ((${IndexType}<${ContextName}Entity, ${KeyType}>)GetEntityIndex(Contexts.${IndexName})).GetEntities(${MemberName});
    }";

        const string GET_PRIMARY_INDEX_TEMPLATE =
            @"    public ${ContextName}Entity TryGetEntityWith${ComponentName}(${KeyType} ${MemberName}) {
        return ((${IndexType}<${ContextName}Entity, ${KeyType}>)GetEntityIndex(Contexts.${IndexName})).GetEntity(${MemberName});
    }
    public ${ContextName}Entity GetEntityWith${ComponentName}(${KeyType} ${MemberName}) {
        var entity = ((${IndexType}<${ContextName}Entity, ${KeyType}>)GetEntityIndex(Contexts.${IndexName})).GetEntity(${MemberName});
        if(entity == null){
			throw new Entitas.MissingPrimaryEntityException<${KeyType}>(${MemberName}, Contexts.${IndexName});
		}
		return entity;
    }";

        const string CUSTOM_METHOD_TEMPLATE =
            @"    public ${ReturnType} ${MethodName}(${methodArgs}) {
        return ((${IndexType})(GetEntityIndex(Contexts.${IndexName}))).${MethodName}(${args});
    }
";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var entityIndexData = data
                .OfType<EntityIndexData>()
                .OrderBy(d => d.GetEntityIndexName())
                .ToArray();

            return entityIndexData.Length == 0
                ? new CodeGenFile[0]
                : generateEntityIndices(entityIndexData);
        }

        CodeGenFile[] generateEntityIndices(EntityIndexData[] data) {

			var files = new List<CodeGenFile>();

            var indexConstants = string.Join("\n", data
                .Select(d => INDEX_CONSTANTS_TEMPLATE
                    .Replace("${IndexName}", d.GetHasMultiple()
                        ? d.GetEntityIndexName() + d.GetMemberName().UppercaseFirst()
                        : d.GetEntityIndexName()))
                .ToArray());

            var addIndices = string.Join("\n\n", data
                .Select(generateAddMethods)
                .ToArray());

            var getIndices = data
                .SelectMany(generateGetMethods)
				.GroupBy(kv => kv.Key);

			foreach(var key in getIndices) {
				var contextFileContent = CONTEXT_CLASS_TEMPLATE
					.Replace("${ContextName}", key.Key)
					.Replace("${getIndices}", string.Join("\n\n", key.Select(x => x.Value).ToArray()));
				files.Add(new CodeGenFile(
					key.Key + Path.DirectorySeparatorChar + key.Key + "Context.cs",
					contextFileContent,
					GetType().FullName
				));
			}

            var fileContent = CLASS_TEMPLATE
                .Replace("${indexConstants}", indexConstants)
                .Replace("${addIndices}", addIndices);

			files.Add(new CodeGenFile(
                "Contexts.cs",
                fileContent,
                GetType().FullName
            ));

            return files.ToArray();
        }

        string generateAddMethods(EntityIndexData data) {
            return string.Join("\n", data.GetContextNames()
                .Aggregate(new List<string>(), (addMethods, contextName) => {
                    addMethods.Add(generateAddMethod(data, contextName));
                    return addMethods;
                }).ToArray());
        }

        string generateAddMethod(EntityIndexData data, string contextName) {
            return data.IsCustom()
                ? generateCustomMethods(data)
                : generateMethods(data, contextName);
        }

        string generateCustomMethods(EntityIndexData data) {
            return ADD_CUSTOM_INDEX_TEMPLATE
                .Replace("${contextName}", data.GetContextNames()[0].LowercaseFirst())
                .Replace("${IndexType}", data.GetEntityIndexType());
        }

        string generateMethods(EntityIndexData data, string contextName) {
            return ADD_INDEX_TEMPLATE
                .Replace("${contextName}", contextName.LowercaseFirst())
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetHasMultiple()
                    ? data.GetEntityIndexName() + data.GetMemberName().UppercaseFirst()
                    : data.GetEntityIndexName())
                .Replace("${Matcher}", data.GetComponentName())
				.Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${KeyType}", data.GetKeyType())
                .Replace("${ComponentType}", data.GetComponentType())
                .Replace("${MemberName}", data.GetMemberName())
                .Replace("${componentName}", data.GetComponentType()
                    .ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces)
                    .LowercaseFirst()
                    .AddPrefixIfIsKeyword());
        }

        IEnumerable<KeyValuePair<string, string>> generateGetMethods(EntityIndexData data) {
            return data.GetContextNames()
                .Aggregate(new List<KeyValuePair<string, string>>(), (getMethods, contextName) =>
                {
                    getMethods.Add(new KeyValuePair<string, string>(contextName, generateGetMethod(data, contextName)));
                    return getMethods;
                });
        }

        string generateGetMethod(EntityIndexData data, string contextName) {
            var template = "";
            if (data.GetEntityIndexType() == "Entitas.EntityIndex") {
                template = GET_INDEX_TEMPLATE;
            } else if (data.GetEntityIndexType() == "Entitas.PrimaryEntityIndex") {
                template = GET_PRIMARY_INDEX_TEMPLATE;
            } else {
                return getCustomMethods(data);
            }

            return template
                .Replace("${ContextName}", contextName)
                .Replace("${IndexName}", data.GetHasMultiple()
                    ? data.GetEntityIndexName() + data.GetMemberName().UppercaseFirst()
                    : data.GetEntityIndexName())
				.Replace("${IndexType}", data.GetEntityIndexType())
                .Replace("${KeyType}", data.GetKeyType())
                .Replace("${ComponentName}", data.GetComponentName())
                .Replace("${MemberName}", data.GetMemberName());
        }

        string getCustomMethods(EntityIndexData data) {
            return string.Join("\n", data.GetCustomMethods()
                .Select(m => CUSTOM_METHOD_TEMPLATE
                    .Replace("${ReturnType}", m.returnType)
                    .Replace("${MethodName}", m.methodName)
                    .Replace("${ContextName}", data.GetContextNames()[0])
                    .Replace("${methodArgs}", string.Join(", ", m.parameters.Select(p => p.type + " " + p.name).ToArray()))
                    .Replace("${IndexType}", data.GetEntityIndexType())
                    .Replace("${IndexName}", data.GetHasMultiple()
                        ? data.GetEntityIndexName() + data.GetMemberName().UppercaseFirst()
                        : data.GetEntityIndexName())
                    .Replace("${args}", string.Join(", ", m.parameters.Select(p => p.name).ToArray()))).ToArray());
        }
    }
}
