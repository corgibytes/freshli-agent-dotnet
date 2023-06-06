using System.Collections.Generic;
using System.IO;

namespace Corgibytes.Freshli.Lib.Test
{
    public class Fixtures
    {
        public static string Path(params string[] values)
        {

            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var components = new List<string>()
            {
                Directory.GetParent(assemblyPath)!.ToString(),
                "Fixtures"
            };
            components.AddRange(values);

            return System.IO.Path.Combine(components.ToArray());
        }
    }
}
