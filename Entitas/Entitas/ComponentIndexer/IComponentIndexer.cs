namespace Entitas {
    
    public delegate void IndexSizeChanged (
        int index
    );
    
    public interface IComponentIndexer {
        event IndexSizeChanged OnIndexSizeChanged;
        int componentIndexSize { get; }
        int GetComponentIndex(System.Type componentType);
    }
}
