using System;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class BoolTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type == typeof(bool);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target, IEntity entity) {
            return EditorGUILayout.Toggle(memberName, (bool)value);
        }
    }
}
