using System;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    public static class CompiledExpressionMethod
    {
        public static T Create<T>() where T : new()
        {
            return FastActivatorImpl<T>.NewFunction();
        }

        private static class FastActivatorImpl<T> where T : new()
        {
            // Compiler translates 'new T()' into Expression.New()
            private static readonly Expression<Func<T>> NewExpression = () => new T();

            // Compiling expression into the delegate
            public static readonly Func<T> NewFunction = NewExpression.Compile();
        }
    }
}