namespace commonItems.Serialization {
	public interface IPDXSerializable {
		public string Serialize(string indent) {
			return PDXSerializer.Serialize(this, indent);
		}
	}
}
