using System;

namespace Entitas {
	public class DynamicIdContext<TEntity, TEntityId> : IdContext<TEntity, TEntityId>, IDynamicContext<TEntity> where TEntity : class, IDynamicEntity {
		public IComponentIndexer componentIndexer { get; private set; }

		public DynamicIdContext(int initialComponentIndexSize, Func<TEntity> entityFactory) : base(initialComponentIndexSize, entityFactory) {
			this.componentIndexer = new ComponentIndexer<TEntity>();
			this.componentIndexer.OnIndexSizeChanged += UpdateTotalComponents;
		}

		public DynamicIdContext(int initialComponentIndexSize, int startCreationIndex, ContextInfo contextInfo, Func<IEntity, IAERC> aercFactory, Func<TEntity> entityFactory) : base(initialComponentIndexSize, startCreationIndex, contextInfo, aercFactory, entityFactory) {
			this.componentIndexer = new ComponentIndexer<TEntity>();
			this.componentIndexer.OnIndexSizeChanged += UpdateTotalComponents;
		}

		protected override void ActivateEntity(TEntity entity) {
			entity.componentIndexer = this.componentIndexer;
			base.ActivateEntity(entity);
		}
	}
}