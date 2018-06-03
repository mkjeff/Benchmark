<Query Kind="Program">
  <NuGetReference>BenchmarkDotNet</NuGetReference>
  <NuGetReference>System.Threading.Tasks.Extensions</NuGetReference>
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>BenchmarkDotNet.Attributes.Jobs</Namespace>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
  <Namespace>BenchmarkDotNet.Configs</Namespace>
  <Namespace>BenchmarkDotNet.Jobs</Namespace>
  <Namespace>BenchmarkDotNet.Diagnosers</Namespace>
</Query>

async Task Main()
{
	BenchmarkRunner.Run<CacheMiss>();
	BenchmarkRunner.Run<CacheHit>();
}

static void Foo(int key){
	
}

[MemoryDiagnoser]
public class CacheHit
{
	private CacheManager _cache;
	private readonly int _cacheExistKey = 100;
	private static async Task<string> GetValueByKey(int key)
	{
		Foo(key);
		await Task.Yield();		
		return string.Empty;
	}
	
	private static readonly Func<int,Task<string>> cacheGetter = GetValueByKey;

	[GlobalSetup]
	public void Setup(){
		_cache = new CacheManager();
		_cache.GetOrCreateObject(_cacheExistKey, TimeSpan.MaxValue, ()=>string.Empty);
	}
	
	[Benchmark]
	public string GetOrCreateObjectAsync()
	{
		var key = _cacheExistKey;
		return _cache.GetOrCreateObjectAsync(key, TimeSpan.MaxValue, async () =>
		{
			Foo(key);
			await Task.Yield();
			return string.Empty;
		}).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string WithoutCapturedVariable()
	{
		var key = _cacheExistKey;
		return _cache.GetOrCreateObjectV2Async(key, TimeSpan.MaxValue, GetValueByKey).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string WithoutCapturedVariable_CacheDelegate()
	{
		var key = _cacheExistKey;
		return _cache.GetOrCreateObjectV2Async(key, TimeSpan.MaxValue, cacheGetter).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string ValueTask_GetOrCreateObjectAsync()
	{
		var key = _cacheExistKey;
		return _cache.GetOrCreateObjectV3Async(key, TimeSpan.MaxValue, async () =>
		{
			Foo(key);
			await Task.Yield();
			return string.Empty;
		}).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string ValueTask_WithoutCapturedVariable()
	{
		var key = _cacheExistKey;
		return _cache.GetOrCreateObjectV4Async(key, TimeSpan.MaxValue, GetValueByKey).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string ValueTask_WithoutCapturedVariable_CacheDelegate()
	{
		var key = _cacheExistKey;
		return _cache.GetOrCreateObjectV4Async(key, TimeSpan.MaxValue, cacheGetter).GetAwaiter().GetResult();
	}
}

[MemoryDiagnoser]
public class CacheMiss
{
	private CacheManager _cache;
	private readonly int _cacheMissingKey = 101;

	private static async Task<string> GetValueByKey(int key)
	{
		Foo(key);
		await Task.Yield();
		return string.Empty;
	}
	
	private static readonly Func<int, Task<string>> cacheGetter = GetValueByKey;

	[GlobalSetup]
	public void Setup()
	{
		_cache = new CacheManager();
	}
	
	[IterationCleanup]
	public void Cleanup()
	{
		_cache.Clear();
	}

	[Benchmark]
	public string GetOrCreateObjectAsync()
	{
		var key = _cacheMissingKey;
		return _cache.GetOrCreateObjectAsync(key, TimeSpan.MaxValue, async () =>
		{
			Foo(key);
			await Task.Yield();
			return string.Empty;
		}).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string WithoutCapturedVariable()
	{
		var key = _cacheMissingKey;
		return _cache.GetOrCreateObjectV2Async(key, TimeSpan.MaxValue, GetValueByKey).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string WithoutCapturedVariable_CacheDelegate()
	{
		var key = _cacheMissingKey;
		return _cache.GetOrCreateObjectV2Async(key, TimeSpan.MaxValue, cacheGetter).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string ValueTask_GetOrCreateObjectAsync()
	{
		var key = _cacheMissingKey;
		return _cache.GetOrCreateObjectV3Async(key, TimeSpan.MaxValue, async () =>
		{
			Foo(key);
			await Task.Yield();
			return string.Empty;
		}).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string ValueTask_WithoutCapturedVariable()
	{
		var key = _cacheMissingKey;
		return _cache.GetOrCreateObjectV4Async(key, TimeSpan.MaxValue, GetValueByKey).GetAwaiter().GetResult();
	}

	[Benchmark]
	public string ValueTask_WithoutCapturedVariable_CacheDelegate()
	{
		var key = _cacheMissingKey;
		return _cache.GetOrCreateObjectV4Async(key, TimeSpan.MaxValue, cacheGetter).GetAwaiter().GetResult();
	}
}

public sealed class CacheManager
{
	// one day into the future
	private static readonly TimeSpan DefaultTimeSpan = new TimeSpan((long)24 * 60 * 60 * TimeSpan.TicksPerSecond);

	private static readonly ConcurrentDictionary<string, Tuple<DateTime, object>> _objectByString =
		new ConcurrentDictionary<string, Tuple<DateTime, object>>(StringComparer.OrdinalIgnoreCase);

	public void Clear()
	{
		_objectByString.Clear();
	}

	public Task<T> GetOrCreateObjectAsync<T>(object cacheKey, TimeSpan? expiresIn, Func<Task<T>> getFunc)
	{
		var key = cacheKey.ToString();

		if (_objectByString.TryGetValue(key, out var tuple) && tuple.Item1 >= DateTime.UtcNow)
		{
			return Task.FromResult((T)tuple.Item2);
		}

		return CreateAndCacheAsync(key, expiresIn, getFunc);

		async Task<T> CreateAndCacheAsync(string stringKey, TimeSpan? expires, Func<Task<T>> getter)
		{

			var val = await getter();
			var expirationTime = expires == TimeSpan.MaxValue
				? DateTime.MaxValue
				: DateTime.UtcNow.Add(expires ?? DefaultTimeSpan);
			var tuple2 = Tuple.Create(expirationTime, (object)val);
			_objectByString.AddOrUpdate(stringKey, tuple2, (k, v) => tuple2);
			return val;
		}
	}

	public Task<T> GetOrCreateObjectV2Async<TKey, T>(TKey cacheKey, TimeSpan? expiresIn, Func<TKey, Task<T>> getFunc)
	{
		var key = cacheKey.ToString();

		if (_objectByString.TryGetValue(key, out var tuple) && tuple.Item1 >= DateTime.UtcNow)
		{
			return Task.FromResult((T)tuple.Item2);
		}

		return CreateAndCacheAsync(key, cacheKey, expiresIn, getFunc);

		async Task<T> CreateAndCacheAsync(string stringKey, TKey rawkey, TimeSpan? expires, Func<TKey, Task<T>> getter)
		{
			var val = await getter(rawkey);
			var expirationTime = expires == TimeSpan.MaxValue
				? DateTime.MaxValue
				: DateTime.UtcNow.Add(expires ?? DefaultTimeSpan);
			var tuple2 = Tuple.Create(expirationTime, (object)val);
			_objectByString.AddOrUpdate(stringKey, tuple2, (k, v) => tuple2);
			return val;
		}
	}

	public ValueTask<T> GetOrCreateObjectV3Async<TKey, T>(TKey cacheKey, TimeSpan? expiresIn, Func<Task<T>> getFunc)
	{
		var key = cacheKey.ToString();

		if (_objectByString.TryGetValue(key, out var tuple) && tuple.Item1 >= DateTime.UtcNow)
		{
			return new ValueTask<T>((T)tuple.Item2);
		}

		return new ValueTask<T>(CreateAndCacheAsync(key, expiresIn, getFunc));

		async Task<T> CreateAndCacheAsync(string stringKey, TimeSpan? expires, Func<Task<T>> getter)
		{
			var val = await getter();
			var expirationTime = expires == TimeSpan.MaxValue
				? DateTime.MaxValue
				: DateTime.UtcNow.Add(expires ?? DefaultTimeSpan);
			var tuple2 = Tuple.Create(expirationTime, (object)val);
			_objectByString.AddOrUpdate(stringKey, tuple2, (k, v) => tuple2);
			return val;
		}
	}

	public ValueTask<T> GetOrCreateObjectV4Async<TKey, T>(TKey cacheKey, TimeSpan? expiresIn, Func<TKey, Task<T>> getValueFunc)
	{
		var key = cacheKey.ToString();

		if (_objectByString.TryGetValue(key, out var tuple) && tuple.Item1 >= DateTime.UtcNow)
		{
			return new ValueTask<T>((T)tuple.Item2);
		}

		return new ValueTask<T>(CreateAndCacheAsync(key, cacheKey, expiresIn, getValueFunc));

		async Task<T> CreateAndCacheAsync(string stringKey, TKey rawKey, TimeSpan? expires, Func<TKey, Task<T>> getFunc)
		{
			var val = await getFunc(rawKey);
			var expirationTime = expires == TimeSpan.MaxValue
				? DateTime.MaxValue
				: DateTime.UtcNow.Add(expires ?? DefaultTimeSpan);
			var tuple2 = Tuple.Create(expirationTime, (object)val);
			_objectByString.AddOrUpdate(stringKey, tuple2, (k, v) => tuple2);
			return val;
		}
	}

	public T GetOrCreateObject<Tkey, T>(Tkey cacheKey, TimeSpan? expiresIn, Func<T> getFunc)
	{
		var key = cacheKey.ToString();

		Tuple<DateTime, object> tuple;
		if (!_objectByString.TryGetValue(key, out tuple) || tuple.Item1 < DateTime.UtcNow)
		{
			var val = getFunc();
			var expirationTime = expiresIn == TimeSpan.MaxValue
				? DateTime.MaxValue
				: DateTime.UtcNow.Add(expiresIn ?? DefaultTimeSpan);
			tuple = Tuple.Create(expirationTime, (object)val);
			_objectByString.AddOrUpdate(key, tuple, (k, v) => tuple);
		}
		return (T)tuple.Item2;
	}
}