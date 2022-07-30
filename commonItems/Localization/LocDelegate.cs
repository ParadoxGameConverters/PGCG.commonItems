namespace commonItems.Localization;

public delegate string? LocDelegate(string? baseLoc, string languageName);
public delegate string? TwoArgLocDelegate(string? baseLoc, string? modifyingLoc, string languageName);
