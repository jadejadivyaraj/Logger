namespace LoggerComponent
{
    internal interface IControlledShutDown
    {
        /// <summary>
        ///     Manages cleanup & Triggers shutdown after finishing the pending operations
        /// </summary>
        /// <param name="forceShutDown">if true, aborts any pending operations and forces shutdown </param>
        void ShutDown(bool forceShutDown = false);
    }
}
