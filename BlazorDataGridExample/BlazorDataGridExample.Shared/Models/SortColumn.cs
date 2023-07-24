// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;

namespace BlazorDataGridExample.Shared.Models
{
    /// <summary>
    /// A SortColumn in a Filter.
    /// </summary>
    public sealed class SortColumn
    {
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public required string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        public required SortDirection SortDirection
        {
            get; set; ]
    }
    }
