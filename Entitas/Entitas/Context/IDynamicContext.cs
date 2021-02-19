namespace Entitas {
	public interface IDynamicContext<TEntity> : IContext<TEntity> where TEntity : class, IEntity {
		IComponentIndexer componentIndexer { get; }
	}
}