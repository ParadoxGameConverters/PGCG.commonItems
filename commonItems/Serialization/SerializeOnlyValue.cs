﻿using System;

namespace commonItems.Serialization;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SerializeOnlyValue : Attribute { }