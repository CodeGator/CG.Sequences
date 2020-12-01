using System;

namespace CG.Sequences
{
    /// <summary>
    /// This interface represents a custom formatter object that uses a 
    /// mask to specify how to convert a value to a string.
    /// </summary>
    public interface IMaskedFormatter : IFormatProvider, ICustomFormatter
    {

    }
}
