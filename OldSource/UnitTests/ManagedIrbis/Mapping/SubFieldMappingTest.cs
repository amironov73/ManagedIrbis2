using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using ManagedIrbis;
using ManagedIrbis.Mapping;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis.Mapping
{
    [TestClass]
    public class SubFieldMappingTest
    {
        class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public void SubFieldMapping_CreateBackwardConverter_1()
        {
            SubFieldMapping<Person> mapping = new SubFieldMapping<Person>
            {
                Code = 'a',
                Property = typeof(Person).GetProperty("Name")
            };
            RecordField field = new RecordField
                (
                    700,
                    new SubField('a', "Not set")
                );
            string expected = "Старик Хоттабыч";
            Person source = new Person
            {
                Name = expected,
                Age = 8942
            };
            Action<Person, RecordField> converter
                = MappingUtility.CreateBackwardConverter(mapping);
            converter(source, field);
            Assert.AreEqual(expected, field.GetFirstSubFieldValue('a'));
        }

        [TestMethod]
        public void SubFieldMapping_CreateBackwardConverter_2()
        {
            SubFieldMapping<Person> mapping = new SubFieldMapping<Person>
            {
                Code = 'z',
                Property = typeof(Person).GetProperty("Age")
            };
            RecordField field = new RecordField
                (
                    700,
                    new SubField('z', "Not set")
                );
            string expected = "8942";
            Person source = new Person
            {
                Name = "Старик Хоттабыч",
                Age = 8942
            };
            Action<Person, RecordField> converter
                = MappingUtility.CreateBackwardConverter(mapping);
            converter(source, field);
            Assert.AreEqual(expected, field.GetFirstSubFieldValue('z'));
        }

        [TestMethod]
        public void SubFieldMapping_CreateForwardConverter_1()
        {
            SubFieldMapping<Person> mapping = new SubFieldMapping<Person>
            {
                Code = 'a',
                Property = typeof(Person).GetProperty("Name")
            };

            string expected = "Старик Хоттабыч";
            RecordField field = new RecordField
                (
                    700,
                    new SubField('a', expected),
                    new SubField('z', "8942")
                );
            Person target = new Person { Name = "Not set" };
            Action<RecordField, Person> converter
                = MappingUtility.CreateForwardConverter(mapping);
            converter(field, target);
            Assert.AreEqual(expected, target.Name);
        }

        [TestMethod]
        public void SubFieldMapping_CreateForwardConverter_2()
        {
            SubFieldMapping<Person> mapping = new SubFieldMapping<Person>
            {
                Code = 'z',
                Property = typeof(Person).GetProperty("Age")
            };

            RecordField field = new RecordField
                (
                    700,
                    new SubField('a', "Старик Хоттабыч"),
                    new SubField('z', "8942")
                );
            Person target = new Person { Age = -1 };
            Action<RecordField, Person> action
                = MappingUtility.CreateForwardConverter(mapping);
            action(field, target);
            Assert.AreEqual(8942, target.Age);
        }
    }
}
