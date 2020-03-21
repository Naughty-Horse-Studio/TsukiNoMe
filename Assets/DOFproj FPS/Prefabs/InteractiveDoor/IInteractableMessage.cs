  /// <summary>
    /// Interface for an interactable target that can display a message.
    /// </summary>
    public interface IInteractableMessage
    {
        /// <summary>
        /// Returns the message that should be displayed when the object can be interacted with.
        /// </summary>
        /// <returns>The message that should be displayed when the object can be interacted with.</returns>
        string AbilityMessage();
    }
