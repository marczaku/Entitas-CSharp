using System;
using System.Collections.Generic;

namespace Entitas {
	public class IdContext<TEntity, TEntityId> : Context<TEntity>, IIdContext<TEntity, TEntityId> where TEntity : class, IEntity {
		readonly Dictionary<TEntityId, TEntity> entityReferences = new Dictionary<TEntityId, TEntity>();

		public IdContext(int totalComponents, Func<TEntity> entityFactory) : base(totalComponents, entityFactory) {
			
		}

		public IdContext(int totalComponents, int startCreationIndex, ContextInfo contextInfo, Func<IEntity, IAERC> aercFactory, Func<TEntity> entityFactory) : base(totalComponents, startCreationIndex, contextInfo, aercFactory, entityFactory) {
		}

		public TEntity GetEntityWithId(TEntityId id) {
			TEntity result;
			this.entityReferences.TryGetValue(id, out result);
			if (result == null)
				throw new EntityDoesNotExistException(id.ToString());
			return result;
		}

		public bool HasEntityWithId(TEntityId id) {
			return this.entityReferences.ContainsKey(id);
		}

		public TEntity TryGetEntityWithId(TEntityId id) {
			TEntity entity;
			this.entityReferences.TryGetValue(id, out entity);
			return entity;
		}

		protected override void ActivateEntity(TEntity entity) {
			base.ActivateEntity(entity);
			var e = entity as IIdEntity<TEntityId>;
			this.entityReferences[e.id] = entity;
		}

		protected override void DeactivateEntity(TEntity entity) {
			base.DeactivateEntity(entity);
			var e = entity as IIdEntity<TEntityId>;
			this.entityReferences.Remove(e.id);
		}
	}
}