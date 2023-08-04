﻿using BlazorDataGridExample.Shared.Models;
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

        protected DateTime? _startDateTime { get; set; }

        protected DateTime? _endDateTime { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetFilterValues();
        }

        private bool IsStartDateTimeDisabled()
        {
            return _filterOperator == FilterOperatorEnum.None
                || _filterOperator == FilterOperatorEnum.IsNull
                || _filterOperator == FilterOperatorEnum.IsNotNull;
        }

        private bool IsEndDateTimeDisabled()
        {
            return (_filterOperator != FilterOperatorEnum.BetweenInclusive && _filterOperator != FilterOperatorEnum.BetweenExclusive);
        }

        private void SetFilterValues()
        {
            if (!FilterState.Filters.TryGetValue(PropertyName, out var filterDescriptor))
            {
                _filterOperator = FilterOperatorEnum.None;
                _startDateTime = null;
                _endDateTime = null;

                return;
            }

            var dateTimeFilterDescriptor = filterDescriptor as DateTimeFilterDescriptor;

            if (dateTimeFilterDescriptor == null)
            {
                _filterOperator = FilterOperatorEnum.None;
                _startDateTime = null;
                _endDateTime = null;

                return;
            }

            _filterOperator = dateTimeFilterDescriptor.FilterOperator;
            _startDateTime = dateTimeFilterDescriptor.StartDateTime?.DateTime;
            _startDateTime = dateTimeFilterDescriptor.EndDateTime?.DateTime;
        }

        protected virtual Task ApplyFilterAsync()
        {
            var numericFilter = new DateTimeFilterDescriptor
            {
                PropertyName = PropertyName,
                FilterOperator = _filterOperator,
                StartDateTime = _startDateTime,
                EndDateTime = _endDateTime,
            };

            return FilterState.AddFilterAsync(numericFilter);
        }

        protected virtual async Task RemoveFilterAsync()
        {
            _filterOperator = FilterOperatorEnum.None;
            _startDateTime = null;
            _endDateTime = null;

            await FilterState.RemoveFilterAsync(PropertyName);
        }
    }
}
