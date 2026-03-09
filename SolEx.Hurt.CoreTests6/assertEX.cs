
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using Xunit;

namespace Xunit
{
    public static class AssertEx
    {
        public static void PropertyValuesAreEquals(object actual, object expected)
        {
            PropertyInfo[] properties = expected.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object expectedValue = property.Get(expected);
                object actualValue = property.Get(actual);

                if (actualValue is IList)
                    AssertListsAreEquals(property, (IList)actualValue, (IList)expectedValue);
                else if (!Equals(expectedValue, actualValue))
                    throw new Exception(string.Format("Property {0}.{1} does not match. Expected: {2} but was: {3}", property.DeclaringType.Name, property.Name, expectedValue, actualValue));
            }
        }

        private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
        {
            if (actualList.Count != expectedList.Count)
                throw new Exception(string.Format("Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements", property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count));

            for (int i = 0; i < actualList.Count; i++)
                if (!Equals(actualList[i], expectedList[i]))
                    throw new Exception(string.Format("Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}", property.PropertyType.Name, property.Name, expectedList[i], actualList[i]));
        }
    }
}
