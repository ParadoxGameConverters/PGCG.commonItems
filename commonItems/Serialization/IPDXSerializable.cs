namespace commonItems.Serialization {
	public interface IPDXSerializable {
		public string Serialize() {
			return PDXSerializer.Serialize(this);
		}
	}
}
