using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fireBwall.Configuration;

namespace fireBwall.Configuration.Testing
{
    [TestFixture]
    public class GeneralConfigurationTests
    {
        private void Setup()
        {
            ConfigurationManagement.Instance.ConfigurationPath = "temp";
        }

        [Test]
        public void TestEmptyLoading()
        {
            Assert.IsTrue(GeneralConfiguration.Instance.Load());
        }

        [Test]
        public void TestSavingAndLoading()
        {
            Setup();
            Assert.IsTrue(GeneralConfiguration.Instance.Load());
            GeneralConfiguration.Instance.PreferredLanguage = "English";
            Assert.IsTrue(GeneralConfiguration.Instance.Save());
            Assert.IsTrue("English".Equals(GeneralConfiguration.Instance.PreferredLanguage));
        }
    }
}
