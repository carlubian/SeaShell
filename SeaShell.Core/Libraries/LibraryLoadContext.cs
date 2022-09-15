using System;
using System.Reflection;
using System.Runtime.Loader;

namespace SeaShell.Core.Libraries
{
    class LibraryLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public LibraryLoadContext(string path) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(path);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
