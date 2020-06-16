using System.IO;
using System.Runtime.InteropServices;

namespace WadReader
{
    static class Util
    {
        public static T ReadStuctureFromStream<T>(Stream stream)
        {
            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];
            stream.Read(bytes, 0, bytes.Length);

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T obj = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),typeof(T));
            handle.Free();
            return obj;
        }
    }
}
