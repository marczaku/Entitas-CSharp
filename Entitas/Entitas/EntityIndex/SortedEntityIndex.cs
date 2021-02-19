using System;
using System.Collections;
using System.Collections.Generic;

namespace Entitas {
	public class SortedEntityIndex<TEntity, TKey> : AbstractEntityIndex<TEntity, TKey> where TEntity : class, IEntity where TKey : IComparable {
		public class SortedMultiValue : IEnumerable<TEntity> {
			SortedDictionary<TKey, HashSet<TEntity>> _data;

			public SortedMultiValue() {
				_data = new SortedDictionary<TKey, HashSet<TEntity>>();
			}

			public void Clear() {
				_data.Clear();
			}

			public void Add(TKey key, TEntity value) {
				HashSet<TEntity> items;
				if (!_data.TryGetValue(key, out items)) {
					items = new HashSet<TEntity>();
					_data.Add(key, items);
				}

				items.Add(value);
			}

			public IEnumerable<TEntity> Get(TKey key) {
				HashSet<TEntity> items;
				if (_data.TryGetValue(key, out items)) {
					return items;
				}

				throw new KeyNotFoundException();
			}
			
			public bool TryGet(TKey key, out HashSet<TEntity> entities) {
				return _data.TryGetValue(key, out entities);
			}

			public void Remove(TKey key, TEntity entity) {
				HashSet<TEntity> entities;
				if (TryGet(key, out entities)) {
					entities.Remove(entity);
				}
			}

			public IEnumerator<TEntity> GetEnumerator() {
				return CreateEnumerable().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return CreateEnumerable().GetEnumerator();
			}

			IEnumerable<TEntity> CreateEnumerable() {
				foreach (var values in _data.Values) {
					foreach (var value in values) {
						yield return value;
					}
				}
			}
		}

		readonly SortedMultiValue _index;

		public SortedEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey) : base(name, group, getKey) {
			_index = new SortedMultiValue();
			Activate();
		}

		public SortedEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys) : base(name, group, getKeys) {
			_index = new SortedMultiValue();
			Activate();
		}

		public override void Activate() {
			base.Activate();
			indexEntities(_group);
		}

		public IEnumerable<TEntity> GetEntities() {
			return _index;
		}

		public override string ToString() {
			return "SortedEntityIndex(" + name + ")";
		}

		protected override void clear() {
			foreach (var entity in _index) {
				var safeAerc = entity.aerc as SafeAERC;
				if (safeAerc != null) {
					if (safeAerc.owners.Contains(this)) {
						entity.Release(this);
					}
				} else {
					entity.Release(this);
				}
			}

			_index.Clear();
		}

		protected override void addEntity(TKey key, TEntity entity) {
			_index.Add(key, entity);

			var safeAerc = entity.aerc as SafeAERC;
			if (safeAerc != null) {
				if (!safeAerc.owners.Contains(this)) {
					entity.Retain(this);
				}
			} else {
				entity.Retain(this);
			}
		}

		protected override void removeEntity(TKey key, TEntity entity) {
			this._index.Remove(key, entity);
			var safeAerc = entity.aerc as SafeAERC;
			if (safeAerc != null) {
				if (safeAerc.owners.Contains(this)) {
					entity.Release(this);
				}
			} else {
				entity.Release(this);
			}
		}
	}
}