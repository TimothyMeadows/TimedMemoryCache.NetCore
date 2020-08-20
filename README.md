# TimedMemoryCache.NetCore
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![nuget](https://img.shields.io/nuget/v/TimedMemoryCache.NetCore.svg)](https://www.nuget.org/packages/TimedMemoryCache.NetCore/)

Implementation of a parallel thread-safe in-memory caching system with save, and load support suited for 'state' programming and easy timeout support for time sensitive caching.

# Install

From a command prompt
```bash
dotnet add package TimedMemoryCache.NetCore
```

```bash
Install-Package TimedMemoryCache.NetCore
```

You can also search for package via your nuget ui / website:

https://www.nuget.org/packages/TimedMemoryCache.NetCore/

# Examples

You can find more examples in the github examples project.

```csharp
// You can populate the cache at initialization time, it will inherit the "default" timeout set in the constructor.
var cache = new TimedMemoryCache(10)
{
  ["caw"] = "first caw!"
};

// You can also access an entry directly by key. However you will need to cast from dynamic using this method.
var caw1 = (string)cache["caw"];

// Will inherit the "default" timeout set in the constructor.
cache["caw2"] = "second caw!";

// You can also override the default timeout and set your own using the Write method!
cache.Write("caw3", "third caw!", 20);

// You can use the OnTimeout event to catch the key, and value of what is removed. You can re-add it back if you want using source, or a stored cache variable.
cache.OnTimeout += (source, key, value) =>
{
  Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Removed '{key}'");
};
```

# Constructor

Timeout is in seconds. Set's the default timeout for all writes that do not specify there own timeout.

```csharp
TimedMemoryCache(long timeout)
```

# Methods

Write a dynamic value to cache with default timeout without returning anything
```csharp
void Write(string key, dynamic value)
```

Write a dynamic value to cache with own timeout without returning anything
```csharp
void Write(string key, dynamic value, long timeout)
```

Write a T value to cache with default timeout returning the T value from cache
```csharp
T Write<T>(string key, T value)
```

Write a T value to cache with own timeout returning the T value from cache
```csharp
T Write<T>(string key, T value, long timeout)
```

Read a value from cache returning as T
```csharp
T Read<T>(string key)
```

Delete an entry from cache without returning anything
```csharp
void Delete(string key)
```

Delete an entry from cache returning that value as T
```csharp
T Delete<T>(string key)
```

Serialize all entries in cache marked as serializable. If you specify T as byte[] binary serialization is used. If you specify T as string json serialization is used.
```csharp
T Save<T>()
```

Load serialized entries into cache. If you specify T as byte[] binary serialization is used. If you specify T as string json serialization is used.
```csharp
void Load<T>(T data, bool clear = true)
```
