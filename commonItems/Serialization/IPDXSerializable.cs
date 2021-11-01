namespace commonItems.Serialization {
	public interface IPDXSerializable {
		public string Serialize(string indent) {
			// default implementation
			return PDXSerializer.SerializeMembers(this, indent);
		}
	}
}
