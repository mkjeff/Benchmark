using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApp1
{
    public static class FastActivator
    {
        public static T Create<T>() where T : new()
        {
            return FastActivatorImpl<T>.Create();
        }

        private static readonly ConcurrentDictionary<Type, Func<object>> FactoryDictionary
            = new ConcurrentDictionary<Type, Func<object>>();

        private static readonly MethodInfo GenerateValueTypeMethod = typeof(DynamicModuleLambdaCompiler)
            .GetMethod(nameof(DynamicModuleLambdaCompiler.GenerateValueTypeFactory));

        private static readonly MethodInfo GenerateTMethod = typeof(DynamicModuleLambdaCompiler)
            .GetMethod(nameof(DynamicModuleLambdaCompiler.GenerateFactory));

        public static object CreateInstance(Type type) =>
            FactoryDictionary.GetOrAdd(type,
                key => (Func<object>)(
                    key.IsValueType
                        ? GenerateValueTypeMethod.Invoke(null, new object[] { key })
                        : GenerateTMethod.MakeGenericMethod(key).Invoke(null, null))
            )();

        private static class FastActivatorImpl<T> where T : new()
        {
            public static readonly Func<T> Create =
                DynamicModuleLambdaCompiler.GenerateFactory<T>();
        }
    }

    public static class FastActivator<T> where T : new()
    {
        /// <summary>
        /// Extremely fast generic factory method that returns an instance
        /// of the type <typeparam name="T"/>.
        /// </summary>
        public static readonly Func<T> Create =
            DynamicModuleLambdaCompiler.GenerateFactory<T>();
    }

    public static class DynamicModuleLambdaCompiler
    {
        public static Func<T> GenerateFactory<T>() where T : new()
        {
            Expression<Func<T>> expr = () => new T();
            NewExpression newExpr = (NewExpression)expr.Body;

            var method = new DynamicMethod(
                name: "lambda",
                returnType: newExpr.Type,
                parameterTypes: new Type[0],
                m: typeof(DynamicModuleLambdaCompiler).Module,
                skipVisibility: true);

            ILGenerator ilGen = method.GetILGenerator();
            // Constructor for value types could be null
            if (newExpr.Constructor != null)
            {
                ilGen.Emit(OpCodes.Newobj, newExpr.Constructor);
            }
            else
            {
                LocalBuilder temp = ilGen.DeclareLocal(newExpr.Type);
                ilGen.Emit(OpCodes.Ldloca, temp);
                ilGen.Emit(OpCodes.Initobj, newExpr.Type);
                ilGen.Emit(OpCodes.Ldloc, temp);
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T>)method.CreateDelegate(typeof(Func<T>));
        }

        public static Func<object> GenerateValueTypeFactory(Type valueType)
        {
            var method = new DynamicMethod(
                name: "lambda",
                returnType: typeof(object),
                parameterTypes: new Type[0],
                m: typeof(DynamicModuleLambdaCompiler).Module,
                skipVisibility: true);

            ILGenerator ilGen = method.GetILGenerator();

            LocalBuilder temp = ilGen.DeclareLocal(valueType);
            ilGen.Emit(OpCodes.Ldloca, temp);
            ilGen.Emit(OpCodes.Initobj, valueType);
            ilGen.Emit(OpCodes.Ldloc, temp);
            ilGen.Emit(OpCodes.Box, valueType);

            ilGen.Emit(OpCodes.Ret);

            return (Func<object>)method.CreateDelegate(typeof(Func<object>));
        }
    }
}