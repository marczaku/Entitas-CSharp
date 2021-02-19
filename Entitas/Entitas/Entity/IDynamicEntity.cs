namespace Entitas {
	public interface IDynamicEntity : IEntity {
		IComponentIndexer componentIndexer { get; set; }
	}
}