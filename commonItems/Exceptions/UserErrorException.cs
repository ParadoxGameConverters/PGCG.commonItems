using System;

namespace commonItems.Exceptions;

public class UserErrorException : ConverterException {
    public UserErrorException(string message) : base(message) { }

    public UserErrorException(string? message, Exception? innerException) : base(message, innerException) { }

    public UserErrorException() { }
}