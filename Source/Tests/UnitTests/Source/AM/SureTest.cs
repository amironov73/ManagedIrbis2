using System;
using System.IO;
using System.Runtime.InteropServices;

using AM;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable NotResolvedInText
// ReSharper disable ConvertNullableToShortForm

namespace UnitTests.AM
{
    [TestClass]
    public class SureTest
        : Common.CommonUnitTest
    {
        enum PetType
        {
            Cat,
            Dog,
            Fish
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOleVariantTypeException))]
        public void Sure_AssertState_1()
        {
            Sure.AssertState(false, "message");
        }

        [TestMethod]
        public void Sure_AssertState_2()
        {
            Sure.AssertState(true, "message");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_Defined_1()
        {
            Sure.Defined((PetType)10, "petType");
        }

        [TestMethod]
        public void Sure_Defined_2()
        {
            Sure.Defined(PetType.Cat, "petType");
            Sure.Defined(PetType.Dog, "petType");
            Sure.Defined(PetType.Fish, "petType");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Sure_FileExists_1()
        {
            Sure.FileExists("NoSuchFile.ogo", "fileName");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Sure_FileExists_2()
        {
            Sure.FileExists(null, "fileName");
        }

        [TestMethod]
        public void Sure_FileExists_3()
        {
            string fileName = Path.Combine(TestDataPath, "canary.xml");
            Sure.FileExists(fileName, nameof(fileName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_InRange_1()
        {
            Sure.InRange(1, 2, 3, "argument");
        }

        [TestMethod]
        public void Sure_InRange_2()
        {
            Sure.InRange(2, 1, 3, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_InRange_3()
        {
            Sure.InRange(1.0, 2.0, 3.0, "argument");
        }

        [TestMethod]
        public void Sure_InRange_4()
        {
            Sure.InRange(2.0, 1.0, 3.0, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_InRange_5()
        {
            Sure.InRange(1L, 2L, 3L, "argument");
        }

        [TestMethod]
        public void Sure_InRange_6()
        {
            Sure.InRange(2L, 1L, 3L, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_NonNegative_1()
        {
            Sure.NonNegative(-1, "argument");
        }

        [TestMethod]
        public void Sure_NonNegative_2()
        {
            Sure.NonNegative(1, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_NonNegative_3()
        {
            Sure.NonNegative(-1.0, "argument");
        }

        [TestMethod]
        public void Sure_NonNegative_4()
        {
            Sure.NonNegative(1.0, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_NonNegative_5()
        {
            Sure.NonNegative(-1L, "argument");
        }

        [TestMethod]
        public void Sure_NonNegative_6()
        {
            Sure.NonNegative(1L, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Sure_NotNull_1()
        {
            Nullable<int> argument = null;
            Sure.NotNull(argument, nameof(argument));
        }

        [TestMethod]
        public void Sure_NotNull_2()
        {
            Nullable<int> argument = 1;
            Sure.NotNull(argument, nameof(argument));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Sure_NotNull_3()
        {
            string argument = null;
            Sure.NotNull(argument, nameof(argument));
        }

        [TestMethod]
        public void Sure_NotNull_4()
        {
            string argument = "1";
            Sure.NotNull(argument, nameof(argument));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Sure_NotNullNorEmpty_1()
        {
            string argument = string.Empty;
            Sure.NotNullNorEmpty(argument, nameof(argument));
        }

        [TestMethod]
        public void Sure_NotNullNorEmpty_2()
        {
            string argument = "1";
            Sure.NotNullNorEmpty(argument, nameof(argument));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_Positive_1()
        {
            Sure.Positive(0, "argument");
        }

        [TestMethod]
        public void Sure_Positive_2()
        {
            Sure.Positive(1, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_Positive_3()
        {
            Sure.Positive(0.0, "argument");
        }

        [TestMethod]
        public void Sure_Positive_4()
        {
            Sure.Positive(1.0, "argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Sure_Positive_5()
        {
            Sure.Positive(0L, "argument");
        }

        [TestMethod]
        public void Sure_Positive_6()
        {
            Sure.Positive(1L, "argument");
        }
    }
}
