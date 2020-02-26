namespace Entitas {

    public class EntityDoesNotExistException : EntitasException {

        public EntityDoesNotExistException(string entityId)
            : base(entityId + "\nEntity does not exist!",
                "The entity has already been destroyed. " +
                "You cannot get destroyed entities.") {
        }
    }
}
