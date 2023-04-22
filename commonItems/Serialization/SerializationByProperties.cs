using System;

namespace commonItems.Serialization; 

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]

public class SerializationByPropertiesAttribute : Attribute { }