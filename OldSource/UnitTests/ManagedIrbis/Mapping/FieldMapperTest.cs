using System;

using ManagedIrbis;
using ManagedIrbis.Mapping;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ExpressionIsAlwaysNull

namespace UnitTests.ManagedIrbis.Mapping
{
    [TestClass]
    public class FieldMapperTest
    {
        //[TestMethod]
        //public void FieldMapper_ToBoolean_1()
        //{
        //    RecordField field = new RecordField(300, "Some value");
        //    Assert.IsTrue(FieldMapper.ToBoolean(field));
        //    field = new RecordField(200);
        //    Assert.IsFalse(FieldMapper.ToBoolean(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToChar_1()
        //{
        //    RecordField field = new RecordField(300, "Some value");
        //    Assert.AreEqual('S', FieldMapper.ToChar(field));
        //    field = new RecordField(300);
        //    Assert.AreEqual('\0', FieldMapper.ToChar(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToDateTime_1()
        //{
        //    RecordField field = new RecordField(300, "20180321");
        //    Assert.AreEqual(new DateTime(2018, 3, 21), FieldMapper.ToDateTime(field));
        //    field = new RecordField(300);
        //    Assert.AreEqual(DateTime.MinValue, FieldMapper.ToDateTime(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToDecimal_1()
        //{
        //    RecordField field = new RecordField(300, "123.456");
        //    Assert.AreEqual(123.456m, FieldMapper.ToDecimal(field));
        //    field = new RecordField(300, "Wrong");
        //    Assert.AreEqual(0.0m, FieldMapper.ToDecimal(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToDouble_1()
        //{
        //    RecordField field = new RecordField(300, "123.456");
        //    Assert.AreEqual(123.456, FieldMapper.ToDouble(field));
        //    field = new RecordField(300, "Wrong");
        //    Assert.AreEqual(0.0, FieldMapper.ToDouble(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToSingle_1()
        //{
        //    RecordField field = new RecordField(300, "123.456");
        //    Assert.AreEqual(123.456f, FieldMapper.ToSingle(field));
        //    field = new RecordField(300, "Wrong");
        //    Assert.AreEqual(0.0f, FieldMapper.ToDouble(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToInt16_1()
        //{
        //    RecordField field = new RecordField(300, "123");
        //    Assert.AreEqual((short)123, FieldMapper.ToInt16(field));
        //    field = new RecordField(300, "Wrong");
        //    Assert.AreEqual((short)0, FieldMapper.ToInt16(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToInt32_1()
        //{
        //    RecordField field = new RecordField(300, "123");
        //    Assert.AreEqual(123, FieldMapper.ToInt32(field));
        //    field = new RecordField(300, "Wrong");
        //    Assert.AreEqual(0, FieldMapper.ToInt32(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToInt64_1()
        //{
        //    RecordField field = new RecordField(300, "123");
        //    Assert.AreEqual(123L, FieldMapper.ToInt64(field));
        //    field = new RecordField(300, "Wrong");
        //    Assert.AreEqual(0L, FieldMapper.ToInt64(field));
        //}

        //[TestMethod]
        //public void FieldMapper_ToString_1()
        //{
        //    string value = "Some value";
        //    RecordField field = new RecordField(300, value);
        //    Assert.AreSame(value, FieldMapper.ToString(field));

        //    value = null;
        //    field = new RecordField(300, value);
        //    Assert.AreSame(value, FieldMapper.ToString(field));
        //}
    }
}
