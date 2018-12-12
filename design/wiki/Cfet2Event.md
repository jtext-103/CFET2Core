# How to use CFET2 Event

## Simple Usage
You can publish or subscribe to CFET2 event when the Hub is injected. This is to say you can do these in `TryInit` or `Start` or anything after that. You can not do these in the constructor.


### To publish event
```csharp

[Cfet2Config]
public int Base
{
    get
    {
        return _base;
    }
    set
    {
        if (_base != value)
        {
            _base = value;
            //Example of publish event
            MyHub.EventHub.Publish(GetPathFor("Base"), "changed", _base); 
        }
    }
} 

```
The above code publish a "changed" event when the "Base" config has changed. The `Publish` method take 3 parameters, the first is the resource path that generate the event. You can use `GetPathFor("Base")` in a thing, this returns `"\[the thing' path]\Base"`, which is the resource path. The 2nd parameter is the EventType, it's a string. The last is the payload, here is the new "Base" value.

## To subscribe an event
```csharp

public class EventListener:Thing
{
   
    public override void Start()
    {
        token= MyHub.EventHub.Subscribe(  //the token is used to unsubscribe event
            new EventFilter(@"(\/(\w)+)*\/base",  //the first parameter of EventFilter is the source filter
                            "changed"),          //the 2nd is the event type filter
            handler);  //the handler is an Action<EventArg>
    }
    private void handler(EventArg e)
    {
        //the event arg contains the source and the event type 
        //the e.sample is the event payload wrapped as ISample, most likely a Status
        Console.WriteLine($"{e.Source} has {e.EventType} to {e.Sample}");
    }
}

```
The above code subscribe to an event. To subscribe an event you need 2 things a event filter and a handler. 
The EventFilter has 2 property, Source and EventType. They a regex expression strings (case insensitive). if they matches the source and EventType that the published event has, then the handler will  be called.

Example:  
`"(\/(\w)+)*\/base" matches "/asd/base", "/base", "/asd/qwe/bAse"`  
`"(\w)*Changed" matches "changed", "NotChanged", "StatusChanged"`

## To unsubscribe
```csharp
[Cfet2Method]
public void UnSub()
{
    token.Dispose();
}
```

