using commonItems.Serialization;
using System;
using Xunit;

namespace commonItems.UnitTests.Serialization;

public sealed class SerializationSupportTests {
	private sealed class DefaultSerializable : IPDXSerializable;

	private sealed class SerializedNameContainer {
		[SerializedName("serialized_value")]
		public int Value { get; init; }
	}

	[Fact]
	public void DefaultInterfaceSerializeThrowsUntilImplemented() {
		IPDXSerializable serializable = new DefaultSerializable();

		var exception = Assert.Throws<NotImplementedException>(() => serializable.Serialize(string.Empty, withBraces: true));

		Assert.Equal("Serialize method must be implemented.", exception.Message);
	}

	[Fact]
	public void SerializedNameStoresConfiguredName() {
		var attribute = new SerializedName("serialized_value");

		Assert.Equal("serialized_value", attribute.Name);
	}

	[Fact]
	public void SerializedNameCanBeRetrievedFromPropertyMetadata() {
		var property = typeof(SerializedNameContainer).GetProperty(nameof(SerializedNameContainer.Value));

		var attribute = Assert.IsType<SerializedName>(Attribute.GetCustomAttribute(property!, typeof(SerializedName))!);

		Assert.Equal("serialized_value", attribute.Name);
	}
}