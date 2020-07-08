# How to access resources in CFET2

![](GettingStartedWithCFET2/2017-09-14-09-25-29.png)

The above image shows how data access happens in CFET2. There are2 types of access local and remote. in the image, the local access call stack is marked as red, the remote is green. the arrows shows how the request is propagated to the thing that has the resource you requested. See [How to Make a CFET2 Communication Module](#how-to-make-a-cfet2-communication-module) for details. Now let just look how to compose a request to a CFET2 resource.

## Accessing using Hub or accessing using plain old HTTP

When you are in a Thing that is mounted to a CFET App, you have access to the hub object, you can use it to access resources in this cfet app or in a remote cfet app. But when you are not, you will have to use a HTTP client to get the resource. Do not mix this 2. Using http to access resource is extremely simple, we could not find a reason to provide a library as you http client of all sorts.

Down below we first introduced how to using the hub way, by this way we can get you familiar with some concept. Then we will show you how to using http client.

## Actions
When you access the resource inside a thing, you use method in hub:
```csharp
       public ISample TryGetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict);
       public ISample TryGetResourceSampleWithUri(string requestUri, params object[] inputs);
       public ISample TryInvokeSampleResourceWithUri(string requestUri, Dictionary<string, bject> inputDict);
       public ISample TryInvokeSampleResourceWithUri(string requestUri, params object[] nputs);
       public ISample TrySetResourceSampleWithUri(string requestUri, Dictionary<string, bject> inputDict);
       public ISample TrySetResourceSampleWithUri(string requestUri, params object[] inputs);
```

The above is obsolete, us

```csharp
public ISample TryAccessResourceSampleWithUri(ResourceRequest request)
```
When you are requesting resource in a thing you can access the hub as a property `MyHub`. Then you can use access method on the hub.

```csharp
public ResourceRequest(string uri,AccessAction action, object[] inputarray, 
            Dictionary<string, object> inputdict, Dictionary<string, string> extraRequest, 
            bool usingdict = false)
```

Before making access request you have to make a request object. It is simple, as show above, most time you will just need the `uri` parameter and the action. For the action you have `public enum AccessAction { get,set,invoke};` to choose.

When you making a request it is like using a remote remote procedure call to a function of a object in a control system. The URI not only specifies the location of the function or property but it also can have parameter in it just like restful APIs (shown later). If -in rare case- you need complex parameters you can put them in a dictionary or in an array, just link when you making HTTP request and put some parameters in the request body or form.

## Using Only the URI path

When you don't have complex type as parameter you can just use the **URI path**. 

For example you have a status implementation like:
```csharp
        [Cfet2Status]
        public int Temp(int channel, string unit)
```
Assume it belongs to a thing name `tempSensor`, and mounted at `/building1/`.

The path to the resource is `/building1/tempsensor/temp`. Note here the path when accessing resource is not case sensitive. Suppose the sensor have many channels and a parameter to specify the unit as C or F. 

Like you want the temperature of the channel 3 in Celsius you can simplly:
```csharp
    MyHub.TryAccessResourceSampleWithUri(new ResourceRequest(
        "/building1/tempsensor/temp/3/c", 
        AccessAction.get,
        null,null,null
        ));
```
Simple right? Just give the path and put the parameters at last, note that the must match the order and the number of the function that implement the resource. if the resource is implemented by property than no parameter is needed.

Using array input
```csharp
         var sample= TryGetResourceSampleWithUri("/building1/tempsensor", new int[]{5,103} );
```
Now you now how input are mapped to the parameters in the resource implementation. Not that when using array input the input must be in the order of the parameters in the method signature. For the above if you use ` new int[]{103,5} `, you would get floor 103, room 5. For dictionary input they are mapped using the string key. The key must match the parameter names in the method signature.

The above is for get, for invoke it's exactly the same.

Oh, I forgot, for the status implemented by property no input is needed. property index is not supported.

For config and set, if it is implemented by method, the parameter and the input are mapped the same way.

But if it is implemented by property, then you cannot use the dictionary input, you can use the array input, and the array contains only one element. Example: 
```csharp
        [Cfet2Config]
        public int Config1 { get; set; } = 1;
        ...................
        var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor", new int[]{5} );
```

### Convert
The CFET2 will handles the type conversion. 

```csharp
var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor", new int[]{5} );

var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor", new string[]{"5"} );
```
The above is the same. It even works on POCO object. When type is not matching, CFET2 will serialize you input into JSON and tye de-serialize them into the target type.

### Using Route
You can embedded input in the route. This means that the `requestUri` does not need to be a path is can be a URI that contains the input.

1. Route Parameters

You can embed you input into URI path, like:
```csharp
        [Cfet2Status]
        public int Temp(int unit,int floor,int section,int room)
```
Assume it belongs to a thing name `tempSensor`, and mounted at `/building1/`.

The path to the resource is `/building1/tempsensor`. If you need unit 1, floor2, section 3, room 4.
```csharp
        var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor/1/2/3/4");
        var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor/1/2", new int[]{3,4} );
```
The above 2 lines are the same. You can put some input in the path some in the input array. The order is the key to get everything mapped right. CFET2 will think `"/building1/tempsensor/1/2/3/4"` is a path to a resource, but if it can not find not it goes upward and think the last segment of the path is a input, like:
```
path="/building1/tempsensor/1/2/3"
input={4}
```
It will go up until it get to a resource. it you have a resource, which happens to have a path of "/building1/tempsensor/1" then if will try to get the resource with:
```
path="/building1/tempsensor/1"
input={2,3,4}
```
That not what you want so proper naming is essential.

The input in the URI path is called **Route Input**. You can use route input with array input, since some input may not be able to be presented as string, like POCO. But you can not use is with dictionary input, since the order of the route input can not be clearly presented. Like
```csharp
        var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor/1/2",new Dictionary<string,object>
         {
             {"section",5},
             {"room", 103}
         });
```
This will fail. When dictionary input are presented, CFET2 will consider the path has no route input. So `"/building1/tempsensor/1/2"` is just a path to a resource. it will try to get that resource with input of `         {
             {"section",5},
             {"room", 103}
         }
`. This is not what you want.

2. Query Parameters

You can also use query string in the requested URI. The query will be converted into key value pairs. This is called query parameters. Take the same example that in the above:

```csharp
        //the order does not matter in query parameters
        var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor?unit=1&section=2&room=4&floor=3");
        var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor?unit=1&room=4&floor=3",
            new Dictionary<string,object>
            {
                {"section",5},
                {"room", 103}
            });
```

You can see the query parameter must contains all the parameters of the config, all it must be completed with a dictionary input as shown in the second line. If there are overlap in dictionary input and query input the dictionary input will overwrite the query input.

The query input cannot used with route parameter, that's to say the path in the URI is the requested path to the resource. Query parameters can not used with array input neither.
```csharp
        //this is WRONG!!!
        var sample= MyHub.TrySetResourceSampleWithUri("/building1/tempsensor?unit=1&room=4&floor=3",
            new int[]{3,5});
```