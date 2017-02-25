﻿using System;
using System.Runtime.InteropServices;
using System.Security;
using BenchmarkDotNet.Attributes;

namespace SequentialGuid
{
    public class SequentialGuidBenchmark
    {
        [Benchmark]
        public Guid NewGuid() => Guid.NewGuid();

        [Benchmark]
        public Guid SequentialGuid() => SQLGuidUtil.NewSequentialId();

        [Benchmark]
        public Guid SuppressSecuritySequentialGuid() => SQLGuidUtil.SuppressSecuritySequentialGuid();
    }

    public class SQLGuidUtil
    {
        
        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);

        public static Guid NewSequentialId()
        {
            Guid guid;
            UuidCreateSequential(out guid);
            var s = guid.ToByteArray();
            var t = new byte[16];
            t[3] = s[0];
            t[2] = s[1];
            t[1] = s[2];
            t[0] = s[3];
            t[5] = s[4];
            t[4] = s[5];
            t[7] = s[6];
            t[6] = s[7];
            t[8] = s[8];
            t[9] = s[9];
            t[10] = s[10];
            t[11] = s[11];
            t[12] = s[12];
            t[13] = s[13];
            t[14] = s[14];
            t[15] = s[15];
            return new Guid(t);
        }

        [DllImport("rpcrt4.dll", EntryPoint = "UuidCreateSequential", SetLastError = true), SuppressUnmanagedCodeSecurity]
        static extern int SUuidCreateSequential(out Guid guid);

        public static Guid SuppressSecuritySequentialGuid()
        {
            Guid guid;
            SUuidCreateSequential(out guid);
            var s = guid.ToByteArray();
            var t = new byte[16];
            t[3] = s[0];
            t[2] = s[1];
            t[1] = s[2];
            t[0] = s[3];
            t[5] = s[4];
            t[4] = s[5];
            t[7] = s[6];
            t[6] = s[7];
            t[8] = s[8];
            t[9] = s[9];
            t[10] = s[10];
            t[11] = s[11];
            t[12] = s[12];
            t[13] = s[13];
            t[14] = s[14];
            t[15] = s[15];
            return new Guid(t);
        }
    }
}
