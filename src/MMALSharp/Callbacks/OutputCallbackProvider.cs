﻿using System.Collections.Generic;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Provides a facility to retrieve callback handlers for Output and Control ports.
    /// </summary>
    public static class OutputCallbackProvider
    {
        /// <summary>
        /// The list of active callback handlers.
        /// </summary>
        public static Dictionary<MMALPortBase, ICallbackHandler> WorkingHandlers { get; private set; } = new Dictionary<MMALPortBase, ICallbackHandler>();

        /// <summary>
        /// Register a new callback handler with a given port.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void RegisterCallback(ICallbackHandler handler)
        {
            if (WorkingHandlers.ContainsKey(handler.WorkingPort))
            {
                WorkingHandlers[handler.WorkingPort] = handler;
            }
            else
            {
                WorkingHandlers.Add(handler.WorkingPort, handler);
            }
        }

        /// <summary>
        /// Finds and returns a <see cref="ICallbackHandler"/> for a given port. If no handler is registered, a 
        /// <see cref="DefaultCallbackHandler"/> will be returned.
        /// </summary>
        /// <param name="port">The port we are retrieving the callback handler on.</param>
        /// <returns>A <see cref="ICallbackHandler"/> for a given port. If no handler is registered, a 
        /// <see cref="DefaultCallbackHandler"/> will be returned.</returns>
        public static ICallbackHandler FindCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                return WorkingHandlers[port];
            }

            return new DefaultCallbackHandler(port);
        }

        /// <summary>
        /// Remove a callback handler for a given port.
        /// </summary>
        /// <param name="port">The port we are removing the callback handler on.</param>
        public static void RemoveCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                WorkingHandlers.Remove(port);
            }
        }
    }
}
