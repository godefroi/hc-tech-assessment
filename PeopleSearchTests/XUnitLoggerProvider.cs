using System;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace hc_assessment_tests
{
    public class XUnitLoggerProvider : ILoggerProvider
    {
        private ITestOutputHelper m_output;

        public XUnitLoggerProvider(ITestOutputHelper output)
        {
            m_output = output;
        }

        public void Dispose() { }

        public ILogger CreateLogger(string categoryName) => new XUnitLogger(m_output);

        public class XUnitLogger : ILogger
        {
            private ITestOutputHelper m_output;

            public XUnitLogger(ITestOutputHelper output)
            {
                m_output = output;
            }

            public string Name { get; } = nameof(XUnitLogger);

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if( !IsEnabled(logLevel) )
                    return;

                if( formatter == null )
                    throw new ArgumentNullException(nameof(formatter));

                var message = formatter(state, exception);

                if( string.IsNullOrWhiteSpace(message) && exception == null )
                    return;

                m_output.WriteLine($"{logLevel}: {this.Name}: {message}");

                if( exception != null )
                    m_output.WriteLine(exception.ToString());
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public IDisposable BeginScope<TState>(TState state) => new XUnitScope();
        }

        public class XUnitScope : IDisposable
        {
            public void Dispose() { }
        }
    }
}
