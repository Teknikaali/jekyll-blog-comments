// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1062:Validate arguments of public methods",
    Justification = "Adding null checks to every test method with parameters is just adding unnecessary noise")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Globalization",
    "CA1303:Do not pass literals as localized parameters",
    Justification = "Test methods won't use localization unless otherwise specified")]