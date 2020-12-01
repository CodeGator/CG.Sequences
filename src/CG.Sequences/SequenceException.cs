using CG.Business;
using System;
using System.Runtime.Serialization;

namespace CG.Sequences
{
    /// <summary>
    /// This class represents a sequence related exception.
    /// </summary>
    [Serializable]
    public class SequenceException : BusinessException
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SequenceException"/>
        /// class.
        /// </summary>
        public SequenceException()
        {

        }

        // *******************************************************************

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SequenceException"/>
        /// class.
        /// </summary>
        /// <param name="message">The message to use for the exception.</param>
        /// <param name="innerException">An optional inner exception reference.</param>
        public SequenceException(
            string message,
            Exception innerException
            ) : base(message, innerException)
        {

        }

        // *******************************************************************

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SequenceException"/>
        /// class.
        /// </summary>
        /// <param name="message">The message to use for the exception.</param>
        public SequenceException(
            string message
            ) : base(message)
        {

        }

        // *******************************************************************

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SequenceException"/>
        /// class.
        /// </summary>
        /// <param name="info">The serialization info to use for the exception.</param>
        /// <param name="context">The streaming context to use for the exception.</param>
        public SequenceException(
            SerializationInfo info,
            StreamingContext context
            ) : base(info, context)
        {

        }

        #endregion
    }
}
