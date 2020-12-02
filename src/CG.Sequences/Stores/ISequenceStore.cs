using CG.Business.Models;
using CG.Business.Stores;
using CG.Sequences.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Sequences.Stores
{
    /// <summary>
    /// This interface represents an object that manages the generation  of numeric
    /// sequences.
    /// </summary>
    /// <typeparam name="TModel">The type of associated model.</typeparam>
    /// <typeparam name="TKey">The type of key associated with the model.</typeparam>
    public interface ISequenceStore<TModel, TKey> : 
        ICrudStore<TModel, TKey>
        where TModel : Sequence, IModel<TKey>
        where TKey : new()
    {
        /// <summary>
        /// This method generates an array of counts for the specified <typeparamref name="TModel"/>
        /// object.
        /// </summary>
        /// <param name="sequence">The sequence to use for the operation.</param>
        /// <param name="count">The number of counts to generate.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that returns an array of formatted counts.</returns>
        Task<string[]> NextAsync(
            TModel sequence,
            int count,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method resets the value of the specified <typeparamref name="TModel"/>
        /// object.
        /// </summary>
        /// <param name="sequence">The sequence to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that returns the newly updated <typeparamref name="TModel"/>
        /// object.</returns>
        Task<TModel> ResetAsync(
            TModel sequence,
            CancellationToken cancellationToken = default
            );
    }
}
