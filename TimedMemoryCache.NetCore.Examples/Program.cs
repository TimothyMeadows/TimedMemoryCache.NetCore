using System;

namespace TimedMemoryCache.NetCore.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
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

            var json = cache.Save<string>();
            cache.Load(json);

            Console.ReadKey();
        }
    }
}
