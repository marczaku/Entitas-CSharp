﻿using System;
using System.Collections.Generic;

namespace Entitas {

    public delegate void ContextEntityChanged(IContext context, IEntity entity);
    public delegate void ContextGroupChanged(IContext context, IGroup group);

    public interface IContext {

        event ContextEntityChanged OnEntityCreated;
        event ContextEntityChanged OnEntityWillBeDestroyed;
        event ContextEntityChanged OnEntityDestroyed;

        event ContextGroupChanged OnGroupCreated;

        int totalComponents { get; }

        Stack<IComponent>[] componentPools { get; }
        ContextInfo contextInfo { get; }

        int count { get; }
        int reusableEntitiesCount { get; }
        int retainedEntitiesCount { get; }

        void DestroyAllEntities();

        void AddEntityIndex(IEntityIndex entityIndex);
        IEntityIndex GetEntityIndex(string name);

        void ResetCreationIndex();
        void ClearComponentPool(int index);
        void ClearComponentPools();
        void Reset();
    }

	public interface IIdContext<TEntity, TEntityId> : IContext<TEntity> where TEntity : class, IEntity {
		TEntity GetEntityWithId(TEntityId id);
		bool HasEntityWithId(TEntityId id);
		bool TryGetEntityWithId(TEntityId id, out TEntity entity);
	}

    public interface IContext<TEntity> : IContext where TEntity : class, IEntity {

        TEntity CreateEntity();

        // TODO Obsolete since 0.42.0, April 2017
        [Obsolete("Please use entity.Destroy()")]
        void DestroyEntity(TEntity entity);

        bool HasEntity(TEntity entity);
        TEntity[] GetEntities();
		TEntity[] GetEntities(IMatcher<TEntity> matcher);
		TEntity CloneEntity(IEntity entity,bool replaceExisting = false,params int[] indices);

        IGroup<TEntity> GetGroup(IMatcher<TEntity> matcher);
    }
}
