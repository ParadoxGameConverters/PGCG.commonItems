using System;

namespace commonItems.Serialization; 

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class NonSerialized : Attribute { }