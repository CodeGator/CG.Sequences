using CG.Business.Models;
using CG.Business.Repositories;

namespace CG.Sequences.Repositories
{
    /// <summary>
    /// This interface represents an object for reading and writing
    /// numeric sequences.
    /// </summary>
    /// <typeparam name="TModel">The model type associated with the repository.</typeparam>
    /// <typeparam name="TKey">The key type associated with the model.</typeparam>
    public interface ISequenceRepository<TModel, TKey> : ICrudRepository<TModel, TKey>
        where TModel : class, IModel<TKey>
        where TKey : new()
    {
        
    }
}
