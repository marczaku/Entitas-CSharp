namespace Entitas {

    public class MissingPrimaryEntityException<KeyType> : EntitasException {

        public MissingPrimaryEntityException(KeyType key)
            : base("No entity exists for PrimaryIndex key '"+key.ToString()+"' of type: "+typeof(KeyType).Name,
				  "Make sure that an entity with that key exists, or use TryGetEntity instead.") {
        }
    }
}
