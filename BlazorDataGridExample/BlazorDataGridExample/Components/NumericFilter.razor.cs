using BlazorDataGridExample.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorDataGridExample.Components
{
    public partial class NumericFilter<TItem>
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

        [Parameter]
        public TItem? LowerValue { get; set; }

        [Parameter]
        public TItem? UpperValue { get; set; }

        /// <summary>
        /// Filter Options available for the NumericFilter.
        /// </summary>
        private readonly FilterOperatorEnum[] filterOperatorOptions = new[]
        {
            FilterOperatorEnum.IsNull,
            FilterOperatorEnum.IsNotNull,
            FilterOperatorEnum.IsEqualTo,
            FilterOperatorEnum.IsNotEqualTo,
            FilterOperatorEnum.IsGreaterThan,
            FilterOperatorEnum.IsGreaterThanOrEqualTo,
            FilterOperatorEnum.IsLessThan,
            FilterOperatorEnum.IsLessThanOrEqualTo,
            FilterOperatorEnum.BetweenExclusive,
            FilterOperatorEnum.BetweenInclusive
        };

        private bool IsLowerValueDisabled()
        {
            return _filterOperator == FilterOperatorEnum.IsNull 
                || _filterOperator == FilterOperatorEnum.IsNotNull;
        }

        private bool IsUpperValueDisabled()
        {
            return (_filterOperator != FilterOperatorEnum.BetweenInclusive && _filterOperator != FilterOperatorEnum.BetweenExclusive);
        }

        protected double? _lowerValue { get; set; }

        protected double? _upperValue { get; set; }

        protected FilterOperatorEnum? _filterOperator { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected virtual Task ApplyFilterAsync()
        {
            var numericFilter = new NumericFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = _filterOperator,
                LowerValue = _lowerValue,
                UpperValue = _upperValue,
            };

            return FilterState.AddFilterAsync(numericFilter);
        }

        protected virtual Task RemoveFilterAsync()
        {
            return FilterState.RemoveFilterAsync(PropertyName);
        }
    }
}
