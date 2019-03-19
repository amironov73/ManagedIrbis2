using System;

using JetBrains.Annotations;

using ManagedIrbis;
using ManagedIrbis.Mapping;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ExpressionIsAlwaysNull

namespace UnitTests.ManagedIrbis.Mapping
{
    [TestClass]
    public class RecordMapperTest
    {
        //[NotNull]
        //private MarcRecord _GetRecord()
        //{
        //    return new MarcRecord()
        //        .AddField(200, new SubField('a', "Заглавие"))
        //        .AddField(300, "123")
        //        .AddField(300, "234")
        //        .AddField(300, "345")
        //        .AddField(910, new SubField('d', "ФКХ"))
        //        .AddField(910, new SubField('d', "АБ"))
        //        .AddField(910, new SubField('d', "ЧЗ"))
        //        .AddField(911, new SubField('e'))
        //        .AddField(911, new SubField('e', "1234"))
        //        .AddField(912, new SubField('a', "12345"))
        //        .AddField(912, new SubField('a', "23456"))
        //        .AddField(913, "20180403")
        //        .AddField(913, "20180404")
        //        .AddField(914, new SubField('a', "20180405"))
        //        .AddField(914, new SubField('a', "20180406"));
        //}

        //[TestMethod]
        //public void RecordMapper_ToBoolean_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.IsTrue(RecordMapper.ToBoolean(record, 300));
        //    Assert.IsFalse(RecordMapper.ToBoolean(record, 200));
        //    Assert.IsFalse(RecordMapper.ToBoolean(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToBoolean_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.IsFalse(RecordMapper.ToBoolean(record, 300, 'a'));
        //    Assert.IsTrue(RecordMapper.ToBoolean(record, 200, 'a'));
        //    Assert.IsFalse(RecordMapper.ToBoolean(record, 200, 'b'));
        //    Assert.IsFalse(RecordMapper.ToBoolean(record, 100, 'a'));
        //}

        //[TestMethod]
        //public void RecordMapper_ToBooleanArray_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    bool[] array = RecordMapper.ToBooleanArray(record, 300);
        //    Assert.AreEqual(3, array.Length);
        //    Assert.IsTrue(array[0]);
        //    Assert.IsTrue(array[1]);
        //    Assert.IsTrue(array[2]);

        //    array = RecordMapper.ToBooleanArray(record, 100);
        //    Assert.AreEqual(0, array.Length);

        //    array = RecordMapper.ToBooleanArray(record, 200);
        //    Assert.AreEqual(1, array.Length);
        //    Assert.IsFalse(array[0]);
        //}

        //[TestMethod]
        //public void RecordMapper_ToBooleanArray_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    bool[] array = RecordMapper.ToBooleanArray(record, 910, 'd');
        //    Assert.AreEqual(3, array.Length);
        //    Assert.IsTrue(array[0]);
        //    Assert.IsTrue(array[1]);
        //    Assert.IsTrue(array[2]);

        //    array = RecordMapper.ToBooleanArray(record, 910, 'e');
        //    Assert.AreEqual(0, array.Length);

        //    array = RecordMapper.ToBooleanArray(record, 100);
        //    Assert.AreEqual(0, array.Length);

        //    array = RecordMapper.ToBooleanArray(record, 200, 'a');
        //    Assert.AreEqual(1, array.Length);
        //    Assert.IsTrue(array[0]);

        //    array = RecordMapper.ToBooleanArray(record, 911, 'e');
        //    Assert.AreEqual(2, array.Length);
        //    Assert.IsFalse(array[0]);
        //    Assert.IsTrue(array[1]);
        //}

        //[TestMethod]
        //public void RecordMapper_ToChar_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual('1', RecordMapper.ToChar(record, 300));
        //    Assert.AreEqual('\0', RecordMapper.ToChar(record, 100));
        //    Assert.AreEqual('\0', RecordMapper.ToChar(record, 200));
        //}

        //[TestMethod]
        //public void RecordMapper_ToChar_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual('\0', RecordMapper.ToChar(record, 300, 'a'));
        //    Assert.AreEqual('\0', RecordMapper.ToChar(record, 100, 'a'));
        //    Assert.AreEqual('З', RecordMapper.ToChar(record, 200, 'a'));
        //    Assert.AreEqual('\0', RecordMapper.ToChar(record, 200, 'b'));
        //}

        //[TestMethod]
        //public void RecordMapper_ToCharArray_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    char[] array = RecordMapper.ToCharArray(record, 300);
        //    Assert.AreEqual(3, array.Length);
        //    Assert.AreEqual('1', array[0]);
        //    Assert.AreEqual('2', array[1]);
        //    Assert.AreEqual('3', array[2]);

        //    array = RecordMapper.ToCharArray(record, 200);
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual('\0', array[0]);

        //    array = RecordMapper.ToCharArray(record, 100);
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToCharArray_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    char[] array = RecordMapper.ToCharArray(record, 200, 'a');
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual('З', array[0]);

        //    array = RecordMapper.ToCharArray(record, 300, 'a');
        //    Assert.AreEqual(0, array.Length);

        //    array = RecordMapper.ToCharArray(record, 100, 'a');
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToDateTime_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(new DateTime(2018, 04, 03), RecordMapper.ToDateTime(record, 913));
        //    Assert.AreEqual(DateTime.MinValue, RecordMapper.ToDateTime(record, 300));
        //    Assert.AreEqual(DateTime.MinValue, RecordMapper.ToDateTime(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToDecimal_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(123m, RecordMapper.ToDecimal(record, 300));
        //    Assert.AreEqual(0m, RecordMapper.ToDecimal(record, 200));
        //    Assert.AreEqual(0m, RecordMapper.ToDecimal(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToDouble_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(123.0, RecordMapper.ToDouble(record, 300));
        //    Assert.AreEqual(0.0, RecordMapper.ToDouble(record, 200));
        //    Assert.AreEqual(0.0, RecordMapper.ToDouble(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToSingle_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(123.0f, RecordMapper.ToSingle(record, 300));
        //    Assert.AreEqual(0.0f, RecordMapper.ToSingle(record, 200));
        //    Assert.AreEqual(0.0f, RecordMapper.ToSingle(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt16_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(123, RecordMapper.ToInt16(record, 300));

        //    Assert.AreEqual(0, RecordMapper.ToInt16(record, 200));
        //    Assert.AreEqual(0, RecordMapper.ToInt16(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt16_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(12345, RecordMapper.ToInt16(record, 912, 'a'));

        //    Assert.AreEqual(0, RecordMapper.ToInt16(record, 200, 'a'));
        //    Assert.AreEqual(0, RecordMapper.ToInt16(record, 200, 'b'));

        //    Assert.AreEqual(0, RecordMapper.ToInt16(record, 100, 'a'));
        //    Assert.AreEqual(0, RecordMapper.ToInt16(record, 910, 'a'));
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt16Array_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    short[] array = RecordMapper.ToInt16Array(record, 300);
        //    Assert.AreEqual(3, array.Length);
        //    Assert.AreEqual(123, array[0]);
        //    Assert.AreEqual(234, array[1]);
        //    Assert.AreEqual(345, array[2]);

        //    array = RecordMapper.ToInt16Array(record, 200);
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual(0, array[0]);

        //    array = RecordMapper.ToInt16Array(record, 100);
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt16Array_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    short[] array = RecordMapper.ToInt16Array(record, 912, 'a');
        //    Assert.AreEqual(2, array.Length);
        //    Assert.AreEqual(12345, array[0]);
        //    Assert.AreEqual(23456, array[1]);

        //    array = RecordMapper.ToInt16Array(record, 200, 'a');
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual(0, array[0]);

        //    array = RecordMapper.ToInt16Array(record, 100, 'a');
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt32_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(123, RecordMapper.ToInt32(record, 300));

        //    Assert.AreEqual(0, RecordMapper.ToInt32(record, 200));
        //    Assert.AreEqual(0, RecordMapper.ToInt32(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt32_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(12345, RecordMapper.ToInt32(record, 912, 'a'));

        //    Assert.AreEqual(0, RecordMapper.ToInt32(record, 200, 'a'));
        //    Assert.AreEqual(0, RecordMapper.ToInt32(record, 200, 'b'));

        //    Assert.AreEqual(0, RecordMapper.ToInt32(record, 100, 'a'));
        //    Assert.AreEqual(0, RecordMapper.ToInt32(record, 910, 'a'));
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt32Array_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    int[] array = RecordMapper.ToInt32Array(record, 300);
        //    Assert.AreEqual(3, array.Length);
        //    Assert.AreEqual(123, array[0]);
        //    Assert.AreEqual(234, array[1]);
        //    Assert.AreEqual(345, array[2]);

        //    array = RecordMapper.ToInt32Array(record, 200);
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual(0, array[0]);

        //    array = RecordMapper.ToInt32Array(record, 100);
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt32Array_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    int[] array = RecordMapper.ToInt32Array(record, 912, 'a');
        //    Assert.AreEqual(2, array.Length);
        //    Assert.AreEqual(12345, array[0]);
        //    Assert.AreEqual(23456, array[1]);

        //    array = RecordMapper.ToInt32Array(record, 200, 'a');
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual(0, array[0]);

        //    array = RecordMapper.ToInt32Array(record, 100, 'a');
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt64_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(123, RecordMapper.ToInt64(record, 300));

        //    Assert.AreEqual(0, RecordMapper.ToInt64(record, 200));
        //    Assert.AreEqual(0, RecordMapper.ToInt64(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt64_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual(12345, RecordMapper.ToInt64(record, 912, 'a'));

        //    Assert.AreEqual(0, RecordMapper.ToInt64(record, 200, 'a'));
        //    Assert.AreEqual(0, RecordMapper.ToInt64(record, 200, 'b'));

        //    Assert.AreEqual(0, RecordMapper.ToInt64(record, 100, 'a'));
        //    Assert.AreEqual(0, RecordMapper.ToInt64(record, 910, 'a'));
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt64Array_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    long[] array = RecordMapper.ToInt64Array(record, 300);
        //    Assert.AreEqual(3, array.Length);
        //    Assert.AreEqual(123, array[0]);
        //    Assert.AreEqual(234, array[1]);
        //    Assert.AreEqual(345, array[2]);

        //    array = RecordMapper.ToInt64Array(record, 200);
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual(0, array[0]);

        //    array = RecordMapper.ToInt64Array(record, 100);
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToInt64Array_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    long[] array = RecordMapper.ToInt64Array(record, 912, 'a');
        //    Assert.AreEqual(2, array.Length);
        //    Assert.AreEqual(12345, array[0]);
        //    Assert.AreEqual(23456, array[1]);

        //    array = RecordMapper.ToInt64Array(record, 200, 'a');
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual(0, array[0]);

        //    array = RecordMapper.ToInt64Array(record, 100, 'a');
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToString_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual("123", RecordMapper.ToString(record, 300));
        //    Assert.IsNull(RecordMapper.ToString(record, 200));
        //    Assert.IsNull(RecordMapper.ToString(record, 100));
        //}

        //[TestMethod]
        //public void RecordMapper_ToString_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    Assert.AreEqual("Заглавие", RecordMapper.ToString(record, 200, 'a'));
        //    Assert.IsNull(RecordMapper.ToString(record, 300, 'a'));
        //    Assert.IsNull(RecordMapper.ToString(record, 100, 'a'));
        //}

        //[TestMethod]
        //public void RecordMapper_ToStringArray_1()
        //{
        //    MarcRecord record = _GetRecord();
        //    string[] array = RecordMapper.ToStringArray(record, 300);
        //    Assert.AreEqual(3, array.Length);
        //    Assert.AreEqual("123", array[0]);
        //    Assert.AreEqual("234", array[1]);
        //    Assert.AreEqual("345", array[2]);

        //    array = RecordMapper.ToStringArray(record, 200);
        //    Assert.AreEqual(1, array.Length);
        //    Assert.IsNull(array[0]);

        //    array = RecordMapper.ToStringArray(record, 100);
        //    Assert.AreEqual(0, array.Length);
        //}

        //[TestMethod]
        //public void RecordMapper_ToStringArray_2()
        //{
        //    MarcRecord record = _GetRecord();
        //    string[] array = RecordMapper.ToStringArray(record, 200, 'a');
        //    Assert.AreEqual(1, array.Length);
        //    Assert.AreEqual("Заглавие", array[0]);

        //    array = RecordMapper.ToStringArray(record, 200, 'b');
        //    Assert.AreEqual(0, array.Length);

        //    array = RecordMapper.ToStringArray(record, 300, 'a');
        //    Assert.AreEqual(0, array.Length);

        //    array = RecordMapper.ToStringArray(record, 100, 'a');
        //    Assert.AreEqual(0, array.Length);
        //}
    }
}
