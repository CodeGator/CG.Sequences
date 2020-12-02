using CG.Business.Models;
using CG.Sequences.Models;
using CG.Sequences.Stores;
using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CG.Sequences
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method registers the stores required to support numeric 
        /// sequences.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the 
        /// operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/>
        /// parameter, for chaining method calls together.</returns>
        public static IServiceCollection AddSequenceStores(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the stores.
            serviceCollection.TryAddScoped<ISequenceStore<Sequence, int>, SequenceStore<Sequence,int>>();

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method registers the stores required to support numeric 
        /// sequences.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the 
        /// operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/>
        /// parameter, for chaining method calls together.</returns>
        public static IServiceCollection AddSequenceStores<TModel, TKey>(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            ) where TModel : Sequence, IModel<TKey>
              where TKey : new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the stores.
            serviceCollection.TryAddScoped<ISequenceStore<TModel, TKey>, SequenceStore<TModel, TKey>>();

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
