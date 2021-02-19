namespace Entitas {
	public interface IIdContext<TEntity, TEntityId> : IContext<TEntity> where TEntity : class, IEntity {
		TEntity GetEntityWithId(TEntityId id);
		bool HasEntityWithId(TEntityId id);
		TEntity TryGetEntityWithId(TEntityId id);
	}
}