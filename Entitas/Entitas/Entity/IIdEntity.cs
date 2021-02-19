namespace Entitas {
	public interface IIdEntity<TEntityId> : IEntity {
		TEntityId id { get; }
	}
}