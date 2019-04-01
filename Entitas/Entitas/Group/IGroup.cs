using System;
using System.Collections;
using System.Collections.Generic;

namespace Entitas {

    public delegate void GroupChanged<TEntity>(
        IGroup<TEntity> group, TEntity entity, int index, IComponent component
    ) where TEntity : class, IEntity;

    public delegate void GroupUpdated<TEntity>(
        IGroup<TEntity> group, TEntity entity, int index,
        IComponent previousComponent, IComponent newComponent
    ) where TEntity : class, IEntity;

    public interface IGroup {

        int count { get; }

        void RemoveAllEventHandlers();
    }

	public struct LimitedEnumerator<TEntity> : IEnumerator<TEntity> {

			int limit;
			int currentIndex;
			TEntity[] array;

			public LimitedEnumerator(int limit, TEntity[] array){
				currentIndex = -1;
				this.limit = limit;
				this.array = array;
			}

			TEntity IEnumerator<TEntity>.Current {
				get {
					return array[currentIndex];
				}
			}

			object IEnumerator.Current {
				get {
					return array[currentIndex];
				}
			}

			void IDisposable.Dispose() {
				array = null;
				currentIndex = 0;
			}

			bool IEnumerator.MoveNext() {
				currentIndex++;
				return currentIndex < limit;
			}

			void IEnumerator.Reset() {
				currentIndex = -1;
			}
		}

		public struct GetEntitiesResult<TEntity> {
			public TEntity[] entities;
			public int count;

            LimitedEnumerator<TEntity> GetEnumerator() {
				return new LimitedEnumerator<TEntity>(count, entities);
			}
		}

    public interface IGroup<TEntity> : IGroup where TEntity : class, IEntity {

        event GroupChanged<TEntity> OnEntityAdded;
        event GroupChanged<TEntity> OnEntityRemoved;
        event GroupUpdated<TEntity> OnEntityUpdated;

        IMatcher<TEntity> matcher { get; }

        void HandleEntitySilently(TEntity entity);
        void HandleEntity(TEntity entity, int index, IComponent component);

        GroupChanged<TEntity> HandleEntity(TEntity entity);

        void UpdateEntity(TEntity entity, int index, IComponent previousComponent, IComponent newComponent);

        bool ContainsEntity(TEntity entity);

        TEntity[] GetEntities();
        TEntity GetSingleEntity();
		TEntity GetFirstEntity();

        HashSet<TEntity>.Enumerator GetEnumerator ();

		GetEntitiesResult<TEntity> GetEntitiesCached();
    }
}
