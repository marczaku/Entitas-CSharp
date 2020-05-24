using System.Linq;

namespace Entitas {

    public class CombineAllMatcher<TEntity> : IMatcher<TEntity> where TEntity : class, IEntity {
        readonly IMatcher<TEntity>[] matchers;

        public int[] indices { 
            get {
                return this.matchers.SelectMany(matcher => matcher.indices).Distinct().ToArray();
            } 
        }

        public CombineAllMatcher(params IMatcher<TEntity>[] matchers) {
            this.matchers = matchers;
        }

        public bool Matches(TEntity entity) {
            foreach (var matcher in this.matchers) {
                if (!matcher.Matches(entity))
                    return false;
            }
            return true;
        }
    }
    
    public class CombineAnyMatcher<TEntity> : IMatcher<TEntity> where TEntity : class, IEntity {
        readonly IMatcher<TEntity>[] matchers;

        public int[] indices { 
            get {
                return this.matchers.SelectMany(matcher => matcher.indices).Distinct().ToArray();
            } 
        }

        public CombineAnyMatcher(params IMatcher<TEntity>[] matchers) {
            this.matchers = matchers;
        }

        public bool Matches(TEntity entity) {
            foreach (var matcher in this.matchers) {
                if (matcher.Matches(entity))
                    return true;
            }
            return false;
        }
    }
}
