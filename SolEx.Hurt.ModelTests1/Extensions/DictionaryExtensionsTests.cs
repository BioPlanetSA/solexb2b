using System.Collections.Generic;
using Xunit;
namespace System.Tests
{
    public class DictionaryExtensionsTests
    {
        [Fact(DisplayName = "Testy sprawdzania poprawności łaczenia słowników")]
        public void AddRangeTest()
        {
           Dictionary<int,int> slow1 = new Dictionary<int, int>();
            slow1.Add(1,1);
            slow1.Add(2,2);
            Dictionary<int,int> slow2 = new Dictionary<int, int>();
            slow2.Add(3,3);
            slow2.Add(4,4);
            slow1.AddRange(slow2);
            Assert.True(slow1.Count==4);
            Assert.True(slow1.ContainsKey(3));
            Assert.True(slow1.ContainsKey(4));

            Dictionary<int, int> slow3 = new Dictionary<int, int>();
            slow3.Add(3,4);
            slow3.Add(5,5);

            slow1.AddRange(slow3);
            Assert.True(slow1.Count == 5);
            Assert.True(slow1.ContainsKey(5));
            Assert.True(slow1[3]==3);
        }
    }
}
