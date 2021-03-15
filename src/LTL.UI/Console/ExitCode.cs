namespace LTL.UI.Console
{
    public enum ExitCode : int
    {
        /// <summary>
        /// Successful log
        /// </summary>
        Success = 0,

        /// <summary>
        /// There was a validation error
        /// </summary>
        ValidationError = 1,

        /// <summary>
        /// There was another type of error present
        /// </summary>
        GeneralError = 2,
    }
}
