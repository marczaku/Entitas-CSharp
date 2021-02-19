using System;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class CharTypeDrawer : ITypeDrawer {

        public bool HandlesType(Type type) {
            return type == typeof(char);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target, IEntity entity) {
            var str = EditorGUILayout.TextField(memberName, ((char)value).ToString());
            return str.Length > 0 ? str[0] : default(char);
        }
    }
}
