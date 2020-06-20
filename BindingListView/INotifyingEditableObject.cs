using System;
using System.ComponentModel;

namespace Equin.ApplicationFramework
{
    /// <summary>
    /// Extends <see cref="System.ComponentModel.IEditableObject"/> by providing events to raise during edit state changes.
    /// </summary>
    internal interface INotifyingEditableObject : IEditableObject
    {
        /// <summary>
        /// An edit has started on the object.
        /// </summary>
        /// <remarks>
        /// This event should be raised from BeginEdit().
        /// </remarks>
        event EventHandler EditBegun;
        /// <summary>
        /// The editing of the object was cancelled.
        /// </summary>
        /// <remarks>
        /// This event should be raised from CancelEdit().
        /// </remarks>
        event EventHandler EditCancelled;
        /// <summary>
        /// The editing of the object was ended.
        /// </summary>
        /// <remarks>
        /// This event should be raised from EndEdit().
        /// </remarks>
        event EventHandler EditEnded;
    }
}
