namespace Entitas {
	public abstract class DynamicIdEntity<TEntityId> : IdEntity<TEntityId>, IDynamicEntity {
		public void AddComponent(System.Type type, IComponent component) {
			AddComponent(this.componentIndexer.GetComponentIndex(type), component);
		}

		public void ReplaceComponent(System.Type type, IComponent component) {
			ReplaceComponent(this.componentIndexer.GetComponentIndex(type), component);
		}

		public void RemoveComponent(System.Type type) {
			RemoveComponent(this.componentIndexer.GetComponentIndex(type));
		}

		public bool HasComponent(System.Type type) {
			return HasComponent(this.componentIndexer.GetComponentIndex(type));
		}

		public IComponent GetComponent(System.Type type) {
			return GetComponent(this.componentIndexer.GetComponentIndex(type));
		}

		public void AddComponent<T>(T component) where T : IComponent {
			AddComponent(this.componentIndexer.GetComponentIndex(typeof(T)), component);
		}

		public void ReplaceComponent<T>(T component) where T : IComponent {
			ReplaceComponent(this.componentIndexer.GetComponentIndex(typeof(T)), component);
		}

		public void RemoveComponent<T>() where T : IComponent {
			RemoveComponent(this.componentIndexer.GetComponentIndex(typeof(T)));
		}

		public bool HasComponent<T>() where T : IComponent {
			return HasComponent(this.componentIndexer.GetComponentIndex(typeof(T)));
		}

		public T GetComponent<T>() where T : IComponent {
			return (T) GetComponent(this.componentIndexer.GetComponentIndex(typeof(T)));
		}

		public IComponentIndexer componentIndexer { get; set; }
	}
}