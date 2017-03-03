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
        public Guid PureCsCodeSequentialGuid() => Identifier.NewSequentialGuid();

        [Benchmark]
        public Guid UuidCreateSequential() => SQLGuidUtil.NewSequentialId();

        [Benchmark]
        public Guid FastUuidCreateSequential() => SQLGuidUtil.FastSequentialGuid();
    }

    public class SQLGuidUtil
    {

        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);

        public static Guid NewSequentialId()
        {
            const int RPC_S_OK = 0;
            Guid guid;
            int hr = UuidCreateSequential(out guid);
            if (hr != RPC_S_OK)
                throw new ApplicationException("UuidCreateSequential failed: " + hr);
            var s = guid.ToByteArray();
            var t = new byte[16];
            // reverse 0~3
            t[3] = s[0];
            t[2] = s[1];
            t[1] = s[2];
            t[0] = s[3];
            // reverse 4~5
            t[5] = s[4];
            t[4] = s[5];
            // reverse 6~7
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
        static extern int SuppressSecuritySequentialGuid(out Guid guid);

        public static Guid SuppressSecuritySequentialGuid()
        {
            const int RPC_S_OK = 0;
            Guid guid;
            int hr = SuppressSecuritySequentialGuid(out guid);
            if (hr != RPC_S_OK)
                throw new ApplicationException("UuidCreateSequential failed: " + hr);
            var s = guid.ToByteArray();
            var t = new byte[16];
            // reverse 0~3
            t[3] = s[0];
            t[2] = s[1];
            t[1] = s[2];
            t[0] = s[3];
            // reverse 4~5
            t[5] = s[4];
            t[4] = s[5];
            // reverse 6~7
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
        static extern int FastUuidCreateSequential(out Guid guid);

        public static Guid FastSequentialGuid()
        {
            const int RPC_S_OK = 0;
            Guid g;
            int hr = FastUuidCreateSequential(out g);
            if (hr != RPC_S_OK)
                throw new ApplicationException("UuidCreateSequential failed: " + hr);
            var s = g.ToByteArray();

            // reverse 0~3
            byte tmp = s[3];
            s[3] = s[0];
            s[0] = tmp;

            tmp = s[2];
            s[2] = s[1];
            s[1] = tmp;

            // reverse 4~5
            tmp = s[5];
            s[5] = s[4];
            s[4] = tmp;

            // reverse 6~7
            tmp = s[7];
            s[7] = s[6];
            s[6] = tmp;

            return new Guid(s);
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
