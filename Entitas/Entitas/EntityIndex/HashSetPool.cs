using System.Collections.Generic;

namespace Entitas {

    public static class HashSetPool<T> {
		public static readonly HashSet<T> instance = new HashSet<T>();
		static Queue<HashSet<T>> pool = new Queue<HashSet<T>>();

		public static HashSet<T> GetInstance() {
			if(pool.Count > 0)
				return pool.Dequeue();
			else
				return new HashSet<T>();
		}

		public static void ReturnInstance(HashSet<T> instance) {
			pool.Enqueue(instance);
		}
	}

	public static class ResizingArray<T> {
		static T[] instance = new T[12];

		public static T[] GetInstance(int size) {
			var length = instance.Length;
			while (length < size) {
				length *= 2;
				if (length >= size)
					System.Array.Resize<T>(ref instance, length);
			}
			return instance;
		}
	}
}
