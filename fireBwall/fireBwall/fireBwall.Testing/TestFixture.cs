using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace fireBwall.Testing
{
    [TestFixture]
    public class TestFixture1
    {
        [Test]
        public void TestTrue()
        {
            Assert.IsTrue(true);
        }
    }
}
