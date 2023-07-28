using BlazorDataGridExample.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorDataGridExample.Components
{
    public partial class BooleanFilter
    {
        /// <summary>
        /// The Property Name.
        /// </summary>
        [Parameter]
        public required string PropertyName { get; set; }

        /// <summary>
        /// The current FilterState.
        /// </summary>
        [Parameter]
        public required FilterState FilterState { get; set; }

        /// <summary>
        /// Filter Options available for the BooleanFilter.
        /// </summary>
        private FilterOperatorEnum[] filterOperatorOptions = new[]
        {
            FilterOperatorEnum.IsNull,
            FilterOperatorEnum.IsNotNull,
            FilterOperatorEnum.Yes,
            FilterOperatorEnum.No,
        };

        string? filterValue;

        FilterOperatorEnum? filterOperator;

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
