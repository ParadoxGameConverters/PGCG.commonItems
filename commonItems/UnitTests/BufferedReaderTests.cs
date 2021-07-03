using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class BufferedReaderTests {
        [Fact]
        public void BufferedReaderReadsCorrectly() {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
            var reader = new BufferedReader(stream);
            var chars = new char[2];
            reader.Read(chars);
            Assert.Equal("12", new string(chars));
            Assert.Equal("345", reader.ReadToEnd());
        }
        [Fact]
        public void BufferedReaderAllowsPushBackOfOneChar()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
            var reader = new BufferedReader(stream);
            var chars = new char[2];
            reader.Read(chars); // in stream: 345
            reader.PushBack('2'); // in stream: 2345
            Assert.Equal("2345", reader.ReadToEnd());
        }
        [Fact]
        public void BufferedReaderThrowsExceptionOnPushBackOfMultipleCharsInARow() {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
            var reader = new BufferedReader(stream);
            var chars = new char[2];
            reader.Read(chars); // in stream: 345
            reader.PushBack('2');
            Assert.Throws<InvalidOperationException>(()=>reader.PushBack('1'));
        }
    }
}
