using System.Linq;

namespace Entitas.CodeGenerator {

    public class UnityContextsGenerator : ICodeGenerator {

        public string name { get { return "UnityContexts"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return false; } }
        public bool runInDryMode { get { return true; } }

        const string CONTEXTS_TEMPLATE =
@"using Entitas;
using System.Linq;

namespace StormChaser.SurvivalCity.Unity {
    public partial class UnityContexts : IContexts {
        public static UnityContexts SharedInstance {
            get {
                if (_sharedInstance == null) {
                    _sharedInstance = new UnityContexts();
                }

                return _sharedInstance;
            }
            set { _sharedInstance = value; }
        }

        static UnityContexts _sharedInstance;

        public static void CreateContextObserver(IContext context) {
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            if (UnityEngine.Application.isPlaying) {
                var observer = new Entitas.VisualDebugging.Unity.ContextObserver(context);
                UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
            }
#endif
        }

${contextProperties}
        public Contexts Contexts { get; set; }

        public IContext[] allContexts { get { return new IContext[] { ${contextList} }.Concat(this.Contexts.allContexts).ToArray(); } }

        public UnityContexts() {
            this.Contexts = new Contexts();
            ${contextAssignments}

            foreach (var context in this.allContexts) {
                CreateContextObserver(context);
            }

            var postConstructors = System.Linq.Enumerable.Where(
                GetType().GetMethods(),
                method => System.Attribute.IsDefined(method, typeof(Entitas.CodeGenerator.Attributes.PostConstructorAttribute))
            );

            foreach (var postConstructor in postConstructors) {
                postConstructor.Invoke(this, null);
            }
        }

        public void Reset() {
            var contexts = this.allContexts;
            for (int i = 0; i < contexts.Length; i++) {
                contexts[i].Reset();
            }
        }
    }
}";

        const string CONTEXT_PROPERTY_TEMPLATE = @"        public ${ContextName}Context ${ContextName} { get; set; }";
        const string CONTEXT_LIST_TEMPLATE = @"this.${ContextName}";
        const string CONTEXT_ASSIGNMENT_TEMPLATE = @"           this.${ContextName} = new ${ContextName}Context();";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var contextNames = data
                .OfType<ContextData>()
                .Select(d => d.GetContextName())
                .OrderBy(contextName => contextName)
                .ToArray();

            return new[] { new CodeGenFile(
                "UnityContexts.cs",
                generateContextsClass(contextNames),
                GetType().FullName)
            };
        }

        string generateContextsClass(string[] contextNames) {
            var contextProperties = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_PROPERTY_TEMPLATE
                        .Replace("${ContextName}", contextName)
                        .Replace("${contextName}", contextName.LowercaseFirst())
                       ).ToArray());

            var contextList = string.Join(", ", contextNames
                .Select(contextName => CONTEXT_LIST_TEMPLATE
                        .Replace("${ContextName}", contextName)
                        .Replace("${contextName}", contextName.LowercaseFirst())
                       ).ToArray());

            var contextAssignments = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_ASSIGNMENT_TEMPLATE
                        .Replace("${ContextName}", contextName)
                        .Replace("${contextName}", contextName.LowercaseFirst())
                       ).ToArray());

            return CONTEXTS_TEMPLATE
                .Replace("${contextProperties}", contextProperties)
                .Replace("${contextList}", contextList)
                .Replace("${contextAssignments}", contextAssignments);
        }
    }
}
