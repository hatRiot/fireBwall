using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fireBwall.Configuration;

namespace fireBwall.Configuration.Testing
{
    [TestFixture]
    public class ThemeConfigurationTests
    {
        [Test]
        public void CreatingAndSavingATheme()
        {
            Program.Setup();
            ThemeConfiguration.Instance.CreateDefaultThemes();
            ThemeConfiguration.Instance.Save();
            Assert.IsTrue(ThemeConfiguration.Instance.Schemes.Count > 0);
        }

        private bool changed = false;

        public void Changed()
        {
            changed = true;
        }

        [Test]
        public void ChangeThemes()
        {
            Program.Setup();
            ThemeConfiguration.Instance.CreateDefaultThemes();
            changed = false;
            ThemeConfiguration.Instance.ThemeChanged += Changed;
            ThemeConfiguration.Instance.ChangeTheme("Light");
            Assert.IsTrue("Light".Equals(GeneralConfiguration.Instance.CurrentTheme));
            Assert.IsTrue(changed);
        }

        [Test]
        public void DontChangeFakeTheme()
        {
            Program.Setup();
            ThemeConfiguration.Instance.CreateDefaultThemes();
            changed = false;
            ThemeConfiguration.Instance.ThemeChanged += Changed;
            ThemeConfiguration.Instance.ChangeTheme("Poo");
            Assert.IsTrue(!"Poo".Equals(GeneralConfiguration.Instance.CurrentTheme));
            Assert.IsTrue(!changed);
        }
    }
}
