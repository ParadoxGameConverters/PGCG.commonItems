﻿using System;
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
            var read = reader.Read(2);
            Assert.Equal("12", read);
            Assert.Equal("345", reader.ReadToEnd());
        }
        [Fact]
        public void BufferedReaderAllowsPushBackOfOneChar()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
            var reader = new BufferedReader(stream);
            reader.Read(); // in stream: 2345
            reader.Read(); // in stream: 345
            reader.PushBack('2'); // in stream: 2345
            Assert.Equal("2345", reader.ReadToEnd());
        }
        [Fact]
        public void BufferedReaderThrowsExceptionOnPushBackOfMultipleCharsInARow() {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("12345"));
            var reader = new BufferedReader(stream);
            reader.Read(2); // in stream: 345
            reader.PushBack('2'); // in stream: 2345
            Assert.Throws<InvalidOperationException>(()=>reader.PushBack('1'));
        }
    }
}