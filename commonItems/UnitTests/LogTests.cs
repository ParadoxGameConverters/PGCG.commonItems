using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace commonItems.UnitTests
{
    public class MyTestClass
    {
        private class LogClass
        {
            private readonly TextWriter outputWriter;
            private readonly int number;
            public LogClass(TextWriter outputWriter, int number)
            {
                this.outputWriter = outputWriter;
                this.number = number;
            }

            public void PrintValue()
            {
                Log.WriteLine(LogLevel.Warning, "Number: " + number);
            }
        }
        

        [Fact]
        public void MyTest()
        {
            const int NumberToPrint = 5;
            var content = new StringBuilder();
            var writer = new StringWriter(content);
            var sut = new LogClass(writer, NumberToPrint);

            sut.PrintValue();

            var actualOutput = content.ToString();
            Assert.Equal("Number: 5\r\n", actualOutput);
        }
    }
    public class LogTests
    {
        [Fact]
        public void ErrorMessagesLogged()
        {
            Log.WriteLine(LogLevel.Error, "Error mesage");
            //Assert.Equal("   [ERROR] Error message\n", log.str());
        }
    }
}
