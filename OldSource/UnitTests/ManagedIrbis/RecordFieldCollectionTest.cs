using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM;
using AM.Runtime;

using ManagedIrbis;

namespace UnitTests.ManagedIrbis
{
    [TestClass]
    public class RecordFieldCollectionTest
    {
        [TestMethod]
        public void RecordFieldCollection_Construction_1()
        {
            RecordFieldCollection collection =
                new RecordFieldCollection
                {
                    new RecordField(),
                    new RecordField("200"),
                    new RecordField("300", "Value")
                };
            Assert.AreEqual(3, collection.Count);
        }

        private void _TestSerialization
            (
                params RecordField[] subFields
            )
        {
            RecordFieldCollection collection1 = new RecordFieldCollection();
            collection1.AddRange(subFields);

            byte[] bytes = collection1.SaveToMemory();

            RecordFieldCollection collection2 = bytes
                    .RestoreObjectFromMemory<RecordFieldCollection>();

            Assert.AreEqual(collection1.Count, collection2.Count);

            for (int i = 0; i < collection1.Count; i++)
            {
                Assert.AreEqual
                    (
                        0,
                        RecordField.Compare
                        (
                            collection1[i],
                            collection2[i]
                        )
                    );
            }
        }

        [TestMethod]
        public void RecordFieldCollection_Serialization_1()
        {
            _TestSerialization();

            _TestSerialization
                (
                    new RecordField(),
                    new RecordField("200"),
                    new RecordField("300", "Value")
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RecordFieldCollection_NotNull_1()
        {
            RecordFieldCollection collection =
                new RecordFieldCollection
                {
                    new RecordField(),
                    null,
                    new RecordField("200")
                };
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ReadOnlyException))]
        public void RecordFieldCollection_ReadOnly_1()
        {
            RecordFieldCollection collection =
                new RecordFieldCollection
                {
                    new RecordField(),
                    new RecordField("200"),
                    new RecordField("300", "Value")
                }.AsReadOnly();

            collection.Add(new RecordField());
        }
    }
}
