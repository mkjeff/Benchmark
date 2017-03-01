using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
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

        [Benchmark]
        public Guid PureCsCodeSequentialGuid() => Identifier.NewSequentialGuid();
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

    public class Identifier
    {
        private static long _lastTimeStamp = DateTime.UtcNow.Ticks;
        public static DateTimeOffset NewDateTimeOffset()
        {
            const long increment = TimeSpan.TicksPerSecond / 300; // SQL Server is accurate to 1/300th of a second

            long original, newValue;

            do
            {
                original = _lastTimeStamp;
                var now = DateTime.UtcNow.Ticks;
                newValue = Math.Max(now, original + increment);
            }
            while (Interlocked.CompareExchange(ref _lastTimeStamp, newValue, original) != original);

            return new DateTimeOffset(newValue, TimeSpan.Zero);
        }


        public static Guid NewSequentialGuid()
        {
            var now = NewDateTimeOffset();

            var days = new TimeSpan(now.Ticks - new DateTime(1900, 1, 1).Ticks).Days;
            var msecs = now.TimeOfDay.TotalMilliseconds * 1000 / 300; // SQL Server is accurate to 1/300th of a second

            var daysBytes = BitConverter.GetBytes(days);
            var msecsBytes = BitConverter.GetBytes(msecs);

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysBytes);
            Array.Reverse(msecsBytes);

            // Get sequential guid
            var guidBytes = Guid.NewGuid().ToByteArray();
            Array.Copy(daysBytes, daysBytes.Length - 2, guidBytes, guidBytes.Length - 6, 2);
            Array.Copy(msecsBytes, msecsBytes.Length - 4, guidBytes, guidBytes.Length - 4, 4);

            return new Guid(guidBytes);
        }
    }
}
