using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ManagedApp
{
    class Program
    {
        private static class MixedLibrary
        {
            public const string LibName = nameof(MixedLibrary) + ".dll";

            [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
            public static extern int NativeEntryPoint(int i);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                CallPInvoke();
            }
            else
            {
                switch(args[0])
                {
                    case "pinvoke":
                        CallPInvoke();
                        return;
                    case "reference":
                        UseAssembly();
                        return;
                    case "loadmanaged":
                        ExplicitLoadManaged();
                        return;
                    case "loadnative":
                        ExplicitLoadNative();
                        return;
                }
            }
        }

        // P/Invoke - goes through InMemoryAssemblyLoader
        static void CallPInvoke()
        {
            Console.WriteLine($"=== {nameof(CallPInvoke)} ===");
            int res = MixedLibrary.NativeEntryPoint(10);
            Console.WriteLine($"NativeEntryPoint returned: {res}");
        }

        // Regular assembly reference - does not go through InMemoryAssemblyLoader
        static void UseAssembly()
        {
            Console.WriteLine($"=== {nameof(UseAssembly)} ===");
            int res = global::MixedLibrary.Test.Run();
            Console.WriteLine($"  Test.Run returned: {res}");
        }

        // Explicit managed assembly load- does not go through InMemoryAssemblyLoader
        static void ExplicitLoadManaged()
        {
            Console.WriteLine($"=== {nameof(ExplicitLoadManaged)} ===");
            var asm = Assembly.Load(nameof(MixedLibrary));
            object res = asm.GetType($"{nameof(MixedLibrary)}.{nameof(global::MixedLibrary.Test)}")
                .GetMethod(nameof(global::MixedLibrary.Test.Run))
                .Invoke(null, null);
            Console.WriteLine($"Test.Run returned: {res}");
        }

        // Explicit native library load and call entry point - goes throguh InMemoryAssemblyLoader
        static unsafe void ExplicitLoadNative()
        {
            Console.WriteLine($"=== {nameof(ExplicitLoadNative)} ===");
            IntPtr lib = NativeLibrary.Load(MixedLibrary.LibName);
            IntPtr entryPoint = NativeLibrary.GetExport(lib, nameof(MixedLibrary.NativeEntryPoint));
            delegate* unmanaged[Stdcall]<int, int> func = (delegate* unmanaged[Stdcall]<int, int>)entryPoint;

            int res = func(1);
            Console.WriteLine($"NativeEntryPoint returned: {res}");
        }
    }
}
