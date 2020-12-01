using CG.Business.Stores;
using CG.Sequences.Models;
using CG.Sequences.Properties;
using CG.Sequences.Repositories;
using CG.Validations;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Sequences.Stores
{
    /// <summary>
    /// This interface represents an object that manages the operation of numeric
    /// sequences.
    /// </summary>
    public class SequenceStore :
        CrudStoreBase<Sequence, int, ISequenceRepository<Sequence, int>>
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SequenceStore"/>
        /// class.
        /// </summary>
        /// <param name="repository">The repository to use with the store.</param>
        public SequenceStore(
            ISequenceRepository<Sequence, int> repository
            ) : base(repository)
        {

        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual async Task<string[]> NextAsync(
            Sequence sequence,
            int count,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(sequence, nameof(sequence))
                .ThrowIfLessThanOrEqualZero(count, nameof(count));

            try
            {
                // Initialize the counts.
                var counts = new string[count];
                for (var x = 0; x < count; x++)
                {
                    // Generate the value.
                    counts[x] = $"{x + sequence.LastValue}";
                }

                // Update the sequence's last value.
                sequence.LastValue += count;

                // Write the changes. (We call the base UpdateAsync method
                //   here so we don't modify the meta-data for sequence
                //   increments.
                var newSequence = await base.UpdateAsync(
                    sequence,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Should we format the counts?
                if (false == string.IsNullOrEmpty(sequence.Mask))
                {
                    // Create the custom formatter.
                    var formatter = new MaskedFormatter();

                    // Loop through the counts.
                    for (var x = 0; x < count; x++)
                    {
                        // Format the value.
                        counts[x] = string.Format(
                            formatter,
                            sequence.Mask,
                            counts[x]
                            );
                    }
                }

                // Return the counts.
                return counts;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new SequenceException(
                    message: string.Format(
                        Resources.SequenceStore_NextAsync,
                        nameof(SequenceStore),
                        count,
                        JsonSerializer.Serialize(sequence)
                        ),
                    innerException : ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Sequence> ResetAsync(
            Sequence sequence,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(sequence, nameof(sequence));

            try
            {
                // Reset the sequence's last value.
                sequence.LastValue = 0;

                // Write the changes.
                var newSequence = await UpdateAsync(
                    sequence,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the updated sequence.
                return newSequence;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new SequenceException(
                    message: string.Format(
                        Resources.SequenceStore_ResetAsync,
                        nameof(SequenceStore),
                        JsonSerializer.Serialize(sequence)
                        ),
                    innerException: ex
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public override Task<Sequence> AddAsync(
            Sequence sequence, 
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(sequence, nameof(sequence));

            // Setup the meta-data for the sequence.
            sequence.CreatedDate = DateTime.Now;
            sequence.CreatedBy = Environment.UserName;
            sequence.UpdatedDate = null;
            sequence.UpdatedBy = "";

            // Give the base class a chance.
            return base.AddAsync(sequence, cancellationToken);
        }

        // *******************************************************************

        /// <inheritdoc />
        public override Task<Sequence> UpdateAsync(
            Sequence sequence, 
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(sequence, nameof(sequence));

            // Setup the meta-data for the sequence.
            sequence.UpdatedDate = DateTime.Now;
            sequence.UpdatedBy = Environment.UserName;

            // Give the base class a chance.
            return base.UpdateAsync(sequence, cancellationToken);
        }

        #endregion
    }
}
