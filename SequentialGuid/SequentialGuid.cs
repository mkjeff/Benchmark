using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using BenchmarkDotNet.Attributes;
using RT.Comb;

namespace SequentialGuid
{
	[MemoryDiagnoser]
	public class SequentialGuidBenchmark
    {
		[Benchmark]
		public Guid NewGuid() => Guid.NewGuid();

		[Benchmark]
        public Guid PureCsCodeSequentialGuid() => Identifier.NewSequentialGuid();

		[Benchmark]
		public Guid FastUuidCreateSequential() => SQLGuidUtil.FastSequentialGuid();

		[Benchmark]
		public Guid CombLegacy() => Provider.Legacy.Create();

		[Benchmark]
		public Guid CombSql() => Provider.Sql.Create();

		//[Benchmark]
		//public Guid CombPostgreSql() => Provider.PostgreSql.Create();
	}

    public class SQLGuidUtil
    {
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

    public static class Identifier
    {
		private static readonly long MinDateTimeTicks = new DateTime(1900, 1, 1).Ticks;

		private static long _lastTimeStamp = DateTime.UtcNow.Ticks;
        private static DateTime NewDateTimeOffset()
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

            return new DateTime(newValue, DateTimeKind.Utc);
        }

		public static Guid NewSequentialGuid()
		{
			var now = NewDateTimeOffset();

			var days = (ushort)(new TimeSpan(now.Ticks - MinDateTimeTicks).Days);
			var msecs = (int)(now.TimeOfDay.TotalMilliseconds / 1000 / 300); // SQL Server is accurate to 1/300th of a second

			Span<byte> guidBytes = stackalloc byte[16];
			Guid.NewGuid().TryWriteBytes(guidBytes);

			BinaryPrimitives.WriteUInt16BigEndian(guidBytes.Slice(10, 2), days);
			BinaryPrimitives.WriteInt32BigEndian(guidBytes.Slice(12, 4), msecs);
			return new Guid(guidBytes);
		}
	}
}
