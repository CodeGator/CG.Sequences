using CG.Business.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace CG.Sequences.Models
{
    /// <summary>
    /// This class represents a numeric sequence.
    /// </summary>
    public class Sequence : ModelBase<int>
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the name of the sequence.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// This property contains the last known value of the sequence.
        /// </summary>
        public BigInteger LastValue { get; set; }

        /// <summary>
        /// This property contains a mask, for formatting the count values
        /// from the sequence.
        /// </summary>
        public string Mask { get; set; }

        /// <summary>
        /// This property indicates whether or not the sequence should
        /// throw an exception if the counter overflows the mask.
        /// </summary>
        public bool ThrowOnMaskOverflow { get; set; }

        /// <summary>
        /// This property indicates whether or not the sequence should
        /// throw an exception if the sequence count overflows.
        /// </summary>
        public bool ThrowOnOverflow { get; set; }

        /// <summary>
        /// This property contains the date when the sequence was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// This property contains the name of the person who created the 
        /// sequence.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// This property contains the date when the sequence was last 
        /// updated.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// This property contains the name of the person who last updated
        /// the sequence.
        /// </summary>
        public string UpdatedBy { get; set; }

        #endregion
    }
}
