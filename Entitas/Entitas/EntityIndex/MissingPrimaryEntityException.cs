namespace Entitas {

    public class MissingPrimaryEntityException<KeyType> : EntitasException {

        public MissingPrimaryEntityException(KeyType key, string entityIndexName)
            : base("No entity exists for PrimaryIndex key '"+key.ToString()+"' of type: "+typeof(KeyType).Name+" for EntityIndex id: "+entityIndexName,
				  "Make sure that an entity with that key exists, or use TryGetEntity instead.") {
        }
    }
}
