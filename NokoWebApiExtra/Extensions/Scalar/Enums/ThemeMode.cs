using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace NokoWebApiExtra.Extensions.Scalar.Enums;

/// <summary>
/// Represents the theme modes available in the application.
/// </summary>
[EnumExtensions]
public enum ThemeMode
{
    /// <summary>
    /// Light mode.
    /// </summary>
    [Description("light")]
    Light,

    /// <summary>
    /// Dark mode.
    /// </summary>
    [Description("dark")]
    Dark
}