using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.Helpers
{
    public static class Helpers
    {
        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true;

            Type type = typeof(T);

            if (!type.IsValueType) return true;
            if (Nullable.GetUnderlyingType(type) != null) return true;

            return false;
        }
    }
}
