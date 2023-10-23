using System.Runtime.CompilerServices;

namespace Basis
{
    public static class TestFunc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TestLog<T>(this T obj)
        {
            Note.note.Log(obj);
            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TestWarning<T>(this T obj)
        {
            Note.note.Warning(obj);
            return obj;
        }
    }
}

