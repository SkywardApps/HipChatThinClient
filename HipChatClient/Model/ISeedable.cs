using System;

namespace HipChatClient.Model
{
    /// <summary>
    /// A quick and dirty interface that allows a model object to load data from a 'seed', a dynamic object.
    /// 
    /// This allows some utility methods to be generic, and provides encapsulation around loading data or extending it. 
    /// For example, the User object only contains id and name in a reference, but if you retrieve the user api endpoint
    /// directly it contains a lot more, so it can be 'upgraded' in this way.
    /// </summary>
    public interface ISeedable
    {
        /// <summary>
        /// Load data from the seed value
        /// </summary>
        /// <param name="seed">A dynamic object containing seed data.</param>
        /// <param name="client">The client this data was retrieved from.</param>
        void Seed(dynamic seed, Client client);

        /// <summary>
        /// Standard unique Id
        /// </summary>
        Int64 Id { get; }

    }
}