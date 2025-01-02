using System;
using System.Collections.Generic;
using System.Linq;

namespace NK.EntityFramework.Common.Models
{
    /// <summary>
    /// Represents the result of a save operation, indicating success or failure, and providing error details if applicable.
    /// </summary>
    public class SaveResult
    {
        /// <summary>
        /// A static instance of <see cref="SaveResult"/> representing a successful save operation.
        /// </summary>
        private static readonly SaveResult _success = new()
        {
            Succeeded = true
        };

        /// <summary>
        /// Gets a value indicating whether the save operation succeeded.
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// Gets a static instance of <see cref="SaveResult"/> representing a successful save operation.
        /// </summary>
        public static SaveResult Success => _success;

        /// <summary>
        /// Returns a string representation of the save result, including error details if the operation failed.
        /// </summary>
        /// <returns>
        /// A string indicating "Succeeded" if the operation was successful,
        /// or a detailed error message if it failed.
        /// </returns>
        public override string ToString()
        {
            if (!Succeeded)
            {
                return $"Failed: {string.Join(", ", Errors)}";
            }

            return "Succeeded";
        }

        /// <summary>
        /// A private collection of error messages associated with a failed save operation.
        /// </summary>
        private readonly List<string> _errors = new();

        /// <summary>
        /// Gets the collection of error messages associated with the save operation.
        /// </summary>
        public IEnumerable<string> Errors => _errors;

        /// <summary>
        /// Creates a <see cref="SaveResult"/> representing a failed save operation,
        /// with the specified error messages.
        /// </summary>
        /// <param name="errors">An array of error messages associated with the failure.</param>
        /// <returns>A new instance of <see cref="SaveResult"/> with failure details.</returns>
        public static SaveResult Failed(params string[] errors)
        {
            var result = new SaveResult { Succeeded = false };
            if (errors != null)
            {
                result._errors.AddRange(errors);
            }
            return result;
        }

        /// <summary>
        /// Creates a <see cref="SaveResult"/> representing a failed save operation,
        /// with the specified error messages.
        /// </summary>
        /// <param name="errors">A list of error messages associated with the failure.</param>
        /// <returns>A new instance of <see cref="SaveResult"/> with failure details.</returns>
        public static SaveResult Failed(List<string>? errors)
        {
            var result = new SaveResult { Succeeded = false };
            if (errors != null)
            {
                result._errors.AddRange(errors);
            }
            return result;
        }
    }
}
