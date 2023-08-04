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
        /// Filter Options available for the DateTimeFilter.
        /// </summary>
        private readonly FilterOperatorEnum[] filterOperatorOptions = new[]
        {
            FilterOperatorEnum.IsNull,
            FilterOperatorEnum.IsNotNull,
            FilterOperatorEnum.All,
            FilterOperatorEnum.Yes,
            FilterOperatorEnum.No,
        };

        protected FilterOperatorEnum _filterOperator { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetFilterValues();
        }

        private void SetFilterValues()
        {
            if (!FilterState.Filters.TryGetValue(PropertyName, out var filterDescriptor))
            {
                _filterOperator = FilterOperatorEnum.None;

                return;
            }

            var dateTimeFilterDescriptor = filterDescriptor as DateTimeFilterDescriptor;

            if (dateTimeFilterDescriptor == null)
            {
                _filterOperator = FilterOperatorEnum.None;

                return;
            }

            _filterOperator = dateTimeFilterDescriptor.FilterOperator;
        }

        protected virtual Task ApplyFilterAsync()
        {
            var numericFilter = new BooleanFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = _filterOperator,
            };

            return FilterState.AddFilterAsync(numericFilter);
        }

        protected virtual async Task RemoveFilterAsync()
        {
            _filterOperator = FilterOperatorEnum.None;

            await FilterState.RemoveFilterAsync(PropertyName);
        }
    }
}
