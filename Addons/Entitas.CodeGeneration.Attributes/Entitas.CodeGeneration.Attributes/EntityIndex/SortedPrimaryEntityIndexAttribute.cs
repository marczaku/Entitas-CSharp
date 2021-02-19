using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class SortedEntityIndexAttribute : AbstractEntityIndexAttribute {

        public SortedEntityIndexAttribute() : base(EntityIndexType.SortedEntityIndex) {
        }
    }
}
