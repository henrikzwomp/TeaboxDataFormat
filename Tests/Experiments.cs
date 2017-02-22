using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using TeaboxDataFormat.IO;

/// <summary>
/// Space for developing new features
/// </summary>

namespace Tests
{
    

    [TestFixture]
    public class Experiments
    {
        
        
        /*[Test, Explicit]
        public void CodeForIdentifingCorrectlyMarkedProperties()
        {
            var foo = new FileRowItem { };
            foreach (var prop in foo.GetType().GetProperties())
            {
                Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));

                var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), false);

                if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                    Console.WriteLine("TeaboxDataAttribute: Yes");
                else
                    Console.WriteLine("TeaboxDataAttribute: No");

                if (prop.PropertyType == typeof(string))
                    Console.WriteLine("Type: string");
                if (prop.PropertyType == typeof(int))
                    Console.WriteLine("Type: int");
                if (prop.PropertyType == typeof(DateTime))
                    Console.WriteLine("Type: DateTime");
            }
        }*/

    }

}


