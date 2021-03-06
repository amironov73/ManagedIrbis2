﻿using System;
using System.Collections.Generic;
using System.Text;

using AM;

using BenchmarkDotNet.Attributes;

// ReSharper disable NotAccessedField.Local

namespace Benchmarks.AM
{
    public class FastNumberBenchmark
    {
        private List<string> _lines;
        private int _data;
        private string _data2;

        [GlobalSetup]
        public void Setup()
        {
            _lines = new List<string>(100000);
            for (int i = 0; i < _lines.Capacity; i++)
            {
                _lines.Add(i.ToInvariantString());
            }
        }

        [Benchmark]
        public void Int32_Parse()
        {
            foreach (var line in _lines)
            {
                _data = int.Parse(line);
            }
        }

        [Benchmark]
        public void FastNumber_ParseInt32_String()
        {
            foreach (var line in _lines)
            {
                _data = FastNumber.ParseInt32(line);
            }
        }

        [Benchmark]
        public unsafe void FastNumber_ParseInt32_Pointer()
        {
            foreach (var line in _lines)
            {
                fixed (char* pointer = line)
                {
                    _data = FastNumber.ParseInt32(pointer, line.Length);
                }
            }
        }

        [Benchmark]
        public void Int32_ToString()
        {
            for (int i = 0; i < 10000; i++)
            {
                _data2 = i.ToInvariantString();
            }
        }

        [Benchmark]
        public void FastNumber_Int32ToString()
        {
            for (int i = 0; i < 10000; i++)
            {
                _data2 = FastNumber.Int32ToString(i);
            }
        }
    }
}
