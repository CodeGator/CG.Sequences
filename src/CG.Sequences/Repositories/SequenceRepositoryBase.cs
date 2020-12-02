using CG.Business.Models;
using CG.Business.Repositories;
using CG.Business.Repositories.Options;
using CG.Sequences.Models;
using Microsoft.Extensions.Options;

namespace CG.Sequences.Repositories
{
    /// <summary>
    /// This class is a default implementation of the <see cref="ISequenceRepository{TModel, TKey}"/> 
    /// interface.
    /// </summary>
    /// <typeparam name="TModel">The model type associated with the repository.</typeparam>
    /// <typeparam name="TKey">The key type associated with the model.</typeparam>
    public abstract class SequenceRepositoryBase<TOptions, TModel, TKey> :
        CrudRepositoryBase<TOptions, TModel, TKey>,
        ISequenceRepository<TModel, TKey>
        where TModel : Sequence, IModel<TKey>
        where TOptions : IOptions<RepositoryOptions>
        where TKey : new()
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SequenceRepositoryBase{TOptions, TModel, TKey}"/>
        /// class.
        /// </summary>
        /// <param name="options">The options to use with the repository.</param>
        protected SequenceRepositoryBase(
            TOptions options
            ) : base(options)
        {

        }

        #endregion
    }
}
