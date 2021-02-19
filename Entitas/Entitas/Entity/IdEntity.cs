namespace Entitas {
	public abstract class IdEntity<TEntityId> : Entity, IIdEntity<TEntityId> {
		public TEntityId id {
			get; protected set;
		}
	}
}