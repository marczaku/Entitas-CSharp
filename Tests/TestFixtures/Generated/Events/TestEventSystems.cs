//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.EventSystemsGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed class TestEventSystems : Feature {

    public TestEventSystems(Contexts contexts) {
        Add(new AnyBaseEventSystem(contexts)); // priority: 0
        Add(new TestAnyEventToGenerateEventSystem(contexts)); // priority: 0
        Add(new AnyFlagEventRemovedEventSystem(contexts)); // priority: 0
        Add(new AnyMixedEventEventSystem(contexts)); // priority: 0
        Add(new MixedEventEventSystem(contexts)); // priority: 0
        Add(new TestAnyMultipleContextStandardEventEventSystem(contexts)); // priority: 0
        Add(new AnyStandardEventEventSystem(contexts)); // priority: 0
        Add(new AnyContextComponentEventEventSystem(contexts)); // priority: 0
        Add(new FlagEntityEventEventSystem(contexts)); // priority: 1
        Add(new TestAnyMultipleEventsStandardEventEventSystem(contexts)); // priority: 1
        Add(new StandardEntityEventRemovedEventSystem(contexts)); // priority: 1
        Add(new TestMultipleEventsStandardEventRemovedEventSystem(contexts)); // priority: 2
    }
}
