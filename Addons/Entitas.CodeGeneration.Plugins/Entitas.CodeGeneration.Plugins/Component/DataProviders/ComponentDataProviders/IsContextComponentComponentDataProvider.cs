using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class IsContextComponentComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var isContextComponent = Attribute
                .GetCustomAttributes(type)
                .OfType<ContextComponentAttribute>()
                .Any();

            data.IsContextComponent(isContextComponent);
        }
    }

    public static class IsContextComponentComponentDataExtension {

        public const string COMPONENT_IS_UNIQUE = "Component.ContextComponent";

        public static bool IsContextComponent(this ComponentData data) {
            return (bool)data[COMPONENT_IS_UNIQUE];
        }

        public static void IsContextComponent(this ComponentData data, bool isContextComponent) {
            data[COMPONENT_IS_UNIQUE] = isContextComponent;
        }
    }
}
