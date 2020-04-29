// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Microsoft.Extensions.Logging;
using System;

namespace Tests.Common
{
    public class TestLogger<T> : TestLogger, ILogger<T>
    {
        public static ILogger<T> Create()
        {
            return new TestLogger<T>();
        }
    }

    public class TestLogger : ILogger, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }
}