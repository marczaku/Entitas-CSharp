//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial interface IMultipleEventsStandardEventEntity {

    MultipleEventsStandardEventComponent multipleEventsStandardEvent { get; }
    bool hasMultipleEventsStandardEvent { get; }

    void AddMultipleEventsStandardEvent(string newValue);
    void ReplaceMultipleEventsStandardEvent(string newValue);
    void RemoveMultipleEventsStandardEvent();
}
