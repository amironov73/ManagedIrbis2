using System.IO;

using AM.Text;

using ManagedIrbis;
using ManagedIrbis.Menus;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis.Menus
{
    [TestClass]
    public class OrgMnuTest
        : Common.CommonUnitTest
    {
        [TestMethod]
        public void OrgMnu_Construction_1()
        {
            OrgMnu mnu = new OrgMnu();
            Assert.IsNotNull(mnu.Country);
            Assert.IsNotNull(mnu.Organization);
            Assert.IsNotNull(mnu.Currency);
            Assert.IsNotNull(mnu.Volume);
            Assert.IsNotNull(mnu.Position);
            Assert.IsNotNull(mnu.Language);
            Assert.IsNotNull(mnu.Check);
            Assert.IsNotNull(mnu.Technology);
            Assert.IsNotNull(mnu.AuthorSign);
            Assert.IsNotNull(mnu.ExtendedAuthors);
            Assert.IsNotNull(mnu.Sigla);
        }

        [TestMethod]
        public void OrgMnu_Construction_2()
        {
            string fileName = Path.Combine(TestDataPath, "ORG.MNU");
            MenuFile menu = MenuFile.ParseLocalFile(fileName, IrbisEncoding.Ansi);
            OrgMnu mnu = new OrgMnu(menu);
            Assert.AreEqual("RU", mnu.Country);
            Assert.AreEqual("НТБ ИрНИТУ", mnu.Organization);
            Assert.AreEqual(" р.", mnu.Currency);
            Assert.AreEqual(" с", mnu.Volume);
            Assert.AreEqual("стр.", mnu.Position);
            Assert.AreEqual("rus", mnu.Language);
            Assert.AreEqual("0", mnu.Check);
            Assert.AreEqual("0", mnu.Technology);
            Assert.AreEqual("1", mnu.AuthorSign);
            Assert.AreEqual("0", mnu.ExtendedAuthors);
            Assert.AreEqual("10010033", mnu.Sigla);
        }

        [TestMethod]
        public void OrgMnu_ApplyToMenu_1()
        {
            OrgMnu mnu = new OrgMnu();
            MenuFile menu = new MenuFile();
            mnu.ApplyToMenu(menu);
            string expected = "1\nRU\n2\nГПНТБ России\n3\n р.\n4\n с\n5\nстр.\n6\nrus\n7\n0\n8\n0\n9\n0\nA\n0\nS\n10010033\n*****\n";
            string actual = menu.ToText().DosToUnix();
            Assert.AreEqual(expected, actual);
        }
    }
}
