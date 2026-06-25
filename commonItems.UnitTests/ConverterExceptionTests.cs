using commonItems.Exceptions;
using System;
using Xunit;

namespace commonItems.UnitTests;

public sealed class ConverterExceptionTests {
	[Fact]
	public void MessageConstructorSetsMessage() {
		var exception = new ConverterException("boom");

		Assert.Equal("boom", exception.Message);
	}

	[Fact]
	public void MessageAndInnerExceptionConstructorSetsProperties() {
		var innerException = new InvalidOperationException("inner");

		var exception = new ConverterException("boom", innerException);

		Assert.Equal("boom", exception.Message);
		Assert.Same(innerException, exception.InnerException);
	}

	[Fact]
	public void ParameterlessConstructorUsesDefaultExceptionMessage() {
		var exception = new ConverterException();

		Assert.Equal("Exception of type 'commonItems.Exceptions.ConverterException' was thrown.", exception.Message);
	}
}