using commonItems.Exceptions;
using System;
using Xunit;

namespace commonItems.UnitTests;

public sealed class UserErrorExceptionTests {
	[Fact]
	public void MessageConstructorSetsMessage() {
		var exception = new UserErrorException("boom");

		Assert.Equal("boom", exception.Message);
		Assert.IsAssignableFrom<ConverterException>(exception);
	}

	[Fact]
	public void MessageAndInnerExceptionConstructorSetsProperties() {
		var innerException = new InvalidOperationException("inner");

		var exception = new UserErrorException("boom", innerException);

		Assert.Equal("boom", exception.Message);
		Assert.Same(innerException, exception.InnerException);
		Assert.IsAssignableFrom<ConverterException>(exception);
	}

	[Fact]
	public void ParameterlessConstructorUsesDefaultExceptionMessage() {
		var exception = new UserErrorException();

		Assert.Equal("Exception of type 'commonItems.Exceptions.UserErrorException' was thrown.", exception.Message);
		Assert.IsAssignableFrom<ConverterException>(exception);
	}
}