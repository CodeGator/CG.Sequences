using CG.Sequences.Properties;
using CG.Validations;
using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace CG.Sequences
{
    /// <summary>
    /// This class is an implementation of <see cref="IMaskedFormatter"/> that 
    /// formats a given value using a mask comprised of literal and token symbols. 
    /// Each token in the mask is used to format the number, according to the
    /// rules for that token. Literal symbols are copied verbatum to the output.
    /// Token symbols can also be escaped by leading the symbol with a '\'.
    /// </summary>
    /// <remarks>
    /// The mask may contain any combination of tokens and literals, to any
    /// length desired. If the value cannot be expressed with a given mask then
    /// a <see cref="FormatException"/> exception is thrown. So, make sure the
    /// mask you use have enough tokens in it to handle the range of values
    /// you'll be trying to format. 
    /// <para />
    /// The mask part of the format string MUST start and end with the '$' symbol 
    /// and may contain the following conversion tokens: 
    /// <list type="bullet">
    /// <listheader>  
    /// <term>Token</term>  
    /// <description>Description</description>  
    /// </listheader>  
    /// <item>  
    /// <term>'D'</term>  
    /// <description>This token denotes a decimal placeholder, which means this
    /// digit will be formatted as a decimal number (0 through 9).</description>  
    /// </item>  
    /// <item>  
    /// <term>'B'</term>  
    /// <description>This token denotes a binary placeholder, which means this
    /// digit will be formatted as a binary number (0 through 1).</description>  
    /// </item>  
    /// <item>  
    /// <term>'O'</term>  
    /// <description>This token denotes an octal placeholder, which means this
    /// digit will be formatted as a octal number (0 through 7).</description>  
    /// </item>
    /// <item>  
    /// <term>'H'</term>  
    /// <description>This token denotes a hex placeholder, which means this
    /// digit will be formatted as a hex number (0 through F).</description>  
    /// </item>
    /// <item>  
    /// <term>'Z'</term>  
    /// <description>This token denotes a base36 placeholder, which means this
    /// digit will be formatted as a printable character (0 through 9, a through Z).
    /// </description>  
    /// </item>
    /// </list>
    /// <para />
    /// Literal symbols may be any legal .NET character but token symbols used as
    /// a literal MUST be escaped with a leading '\' character.
    /// </remarks>
    /// <example>
    /// This example shows how to use the <see cref="MaskedFormatter"/> class to 
    /// format a numeric value into a custom string.
    /// <code>
    /// class TestClass
    /// {
    ///     static void Main()
    ///     {
    ///         var result = String.Format(new MaskedFormatter(), "{0:$HHHH$}", 101);
    ///         Console.WriteLine(result);
    ///         // Outputs the value: 0065
    ///         // (hex conversion with no literals)
    ///         
    ///         result = String.Format(new MaskedFormatter(), "{0:$OOOO$}", 101);
    ///         Console.WriteLine(result);
    ///         // Outputs the value: 0145
    ///         // (octal conversion with no literals)
    ///         
    ///         result = String.Format(new MaskedFormatter(), "{0:$HBOD$}", 201);
    ///         Console.WriteLine(result);
    ///         // Outputs the value: 1041
    ///         // (mixed base conversion with no literals)
    ///         
    ///         result = String.Format(new MaskedFormatter(), "{0:$H_B-O%D=$}", 201);
    ///         Console.WriteLine(result);
    ///         // Outputs the value: 1_0-4%1=
    ///         // (mixed base conversion with embedded literals)
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class MaskedFormatter : IMaskedFormatter
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains the list of valid mask tokens.
        /// </summary>
        private static readonly char[] TOKENS = 
        {
            '$',   // Mask start/end  
            '\\',  // Escape 
            'D',   // Base10
            'B',   // Base2
            'H',   // Base16
            'O',   // Base8
            'Z'    // Base36
        };

        /// <summary>
        /// This field contains the list of base36 replacement symbols
        /// </summary>
        private static readonly char[] BASE36SYMBOLS = 
        {
            '0', '1', '2', '3', '4', '5',
            '6', '7', '8', '9', 'a', 'b',
            'c', 'd', 'e', 'f', 'g', 'h',
            'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z'
        };

        /// <summary>
        /// This field indicates the formatter should throw an exception if
        /// the formatting operation overflows the available digits, in the mask
        /// </summary>
        private readonly bool _throwOnMaskOverflow;

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="MaskedFormatter"/>
        /// class.
        /// </summary>
        /// <param name="throwOnMaskOverflow">True if the formatter should throw
        /// an exception whenever the value exceeds the number of non-literal 
        /// digits in the mask. Empty masks are ignored for this purpose.</param>
        public MaskedFormatter(
            bool throwOnMaskOverflow = false
            )
        {
            // Save the values.
            _throwOnMaskOverflow = throwOnMaskOverflow;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method returns an object that provides formatting services for 
        /// the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format 
        /// object to return.</param>
        /// <returns>An instance of the object specified by formatType, if the 
        /// <see cref="System.IFormatProvider"/> implementation can supply that 
        /// type of object; otherwise, null.</returns>
        public object GetFormat(
            Type formatType
            )
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        // *******************************************************************

        /// <summary>
        /// This method converts the value of a specified object to an equivalent 
        /// string representation using specified format and culture-specific 
        /// formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An object that supplies format information 
        /// about the current instance.</param>
        /// <returns>The string representation of the value of arg, formatted 
        /// as specified by format and formatProvider.</returns>
        public string Format(
            string format,
            object arg,
            IFormatProvider formatProvider
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNullOrEmpty(format, nameof(format))
                .ThrowIfNull(arg, nameof(arg))
                .ThrowIfNull(formatProvider, nameof(formatProvider));

            // Ignore leading/trailing whitespace for the formatting.
            var mask = format.Trim();

            // We'll hold a running value here as we work.
            var value = new BigInteger(0);
            
            // Is the mask valid?
            if (mask.Length > 2 && // MUST have at least 3 tokens.
                mask[0] == TOKENS[0] && // MUST start with a '$' token
                mask[format.Length - 1] == TOKENS[0]) // MUST end with a '$' token
            {
                if (arg is sbyte)
                {
                    value = new BigInteger((sbyte)arg);
                }
                else if (arg is byte)
                {
                    value = new BigInteger((byte)arg);
                }
                else if (arg is short)
                {
                    value = new BigInteger((short)arg);
                }
                else if (arg is int)
                {
                    value = new BigInteger((int)arg);
                }
                else if (arg is long)
                {
                    value = new BigInteger((long)arg);
                }
                else if (arg is ushort)
                {
                    value = new BigInteger((ushort)arg);
                }
                else if (arg is uint)
                {
                    value = new BigInteger((uint)arg);
                }
                else if (arg is ulong)
                {
                    value = new BigInteger((ulong)arg);
                }
                else if (arg is byte[])
                {
                    value = new BigInteger((byte[])arg);
                }
                else if (arg is Guid)
                {
                    value = new BigInteger(((Guid)arg).ToByteArray());
                }
                else
                {
                    // Unknown data type so return default .NET formatting
                    return HandleDefaultFormat(format, arg);
                }

                // Strip out the leading and trailing delimeters.
                mask = mask.Substring(1, format.Length - 2);

                // We'll work with a string-builder as we format.
                var sb = new StringBuilder();

                // The conversion is mask driven so we'll start converting 
                //   backwards from the last mask symbol and we'll keep 
                //   converting until we run out of symbols.
                for (int x = mask.Length - 1; x >= 0; x--)
                {
                    // Figure out how to deal with the mask character.
                    switch (mask[x])
                    {
                        case 'D':
                            sb.Insert(0, string.Format("{0:D}", value % 10));
                            value = value / 10;
                            break;
                        case 'B':
                            sb.Insert(0, string.Format("{0}", value % 2));
                            value = value / 2;
                            break;
                        case 'H':
                            sb.Insert(0, string.Format("{0:X}", value % 16));
                            value = value / 16;
                            break;
                        case 'O':
                            sb.Insert(0, string.Format("{0}", value % 8));
                            value = value / 8;
                            break;
                        case 'Z':
                            sb.Insert(0, string.Format("{0}", BASE36SYMBOLS[Math.Abs((int)(value % 36))]));
                            value = value / 36;
                            break;
                    }
                }

                // Check for numeric overflow ...
                if (value > 0)
                {
                    // Should we throw an exception?
                    if (_throwOnMaskOverflow)
                    {
                        // Panic!
                        throw new FormatException(
                            message: Resources.MaskedFormatter_Overflow
                            );
                    }
                }

                // Loop and deal with any literal mask symbols.
                var escaped = false;
                for (int x = 0; x < mask.Length; x++)
                {
                    switch (mask[x])
                    {
                        case 'D':
                        case 'B':
                        case 'H':
                        case 'O':
                        case 'Z':
                            if (escaped)
                            {
                                sb.Insert(x - 1, mask[x]);
                            }
                            escaped = false;
                            break;
                        case '\\':
                            escaped = true;
                            break;
                        default:
                            sb.Insert(x, mask[x]);
                            break;
                    }
                }

                // Return the formatted value.
                return sb.ToString();
            }
            else
            {
                // Unknown mask symbols so return default .NET formatting.
                return HandleDefaultFormat(format, arg);
            }
        }

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method returns a default format for the specified format and
        /// argument.
        /// </summary>
        /// <param name="format">The format to use for the oeration.</param>
        /// <param name="arg">The argument to be formatted.</param>
        /// <returns>A string representation of the formatted argument.</returns>
        private string HandleDefaultFormat(
            string format, 
            object arg
            )
        {
            if (arg is IFormattable)
            {
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            }
            else if (arg != null)
            {
                return arg.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion
    }
}
