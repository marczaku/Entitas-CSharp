using System;
using System.Collections.Generic;

namespace Entitas {
	public class ComponentIndexer<TEntity> : IComponentIndexer
		where TEntity : class, IEntity {
		Action<int> indexSizeChanged;
		int _componentIndexSize;
		readonly Dictionary<Type, int> componentIndices = new Dictionary<Type, int>();

		public event IndexSizeChanged OnIndexSizeChanged;

		public int componentIndexSize {
			get { return this._componentIndexSize; }
			private set {
				if (this._componentIndexSize == value)
					return;
				this._componentIndexSize = value;
				if (this.OnIndexSizeChanged != null)
					this.OnIndexSizeChanged(value);
			}
		}

		void EnsureCapacity(int minCapacity) {
			if (this._componentIndexSize < minCapacity) {
				this.componentIndexSize = minCapacity + 12;
			}
		}

		public int GetComponentIndex(Type componentType) {
			var index = 0;
			if (!this.componentIndices.TryGetValue(componentType, out index)) {
				index = this.componentIndices.Count;
				this.componentIndices[componentType] = index;
				EnsureCapacity(index + 1);
			}

			return index;
		}
	}
}