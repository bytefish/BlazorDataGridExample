// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BlazorDataGridExample.Localization;
using BlazorDataGridExample.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.Fast.Components.FluentUI;
using WideWorldImportersService;

namespace BlazorDataGridExample.Components
{
    public partial class FilterSelector
    {
        /// <summary>
        /// Text used on aria-label attribute.
        /// </summary>
        [Parameter]
        public virtual string? Title { get; set; }

        /// <summary>
        /// If true, will disable the list of items.
        /// </summary>
        [Parameter]
        public virtual bool Disabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the content to be rendered inside the component.
        /// In this case list of FluentOptions
        /// </summary>
        [Parameter]
        public virtual RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// All selectable Filter Operators.
        /// </summary>
        [Parameter]
        public required FilterOperatorEnum[] FilterOperators { get; set; }

        /// <summary>
        /// The FilterOperator.
        /// </summary>
        [Parameter]
        public FilterOperatorEnum? FilterOperator { get ; set; }

        /// <summary>
        /// Invoked, when the Filter Operator has changed.
        /// </summary>
        [Parameter]
        public EventCallback<FilterOperatorEnum?> FilterOperatorChanged { get; set; }

        /// <summary>
        /// Available FilterOperator Options.
        /// </summary>
        private Option<FilterOperatorEnum>[]? _filterOperatorOptions { get; set; }

        /// <summary>
        /// Value as String.
        /// </summary>
        private string? _filterOperatorAsString { get; set; }

        /// <summary>
        /// Localizer.
        /// </summary>
        [Inject]
        public IStringLocalizer<SharedResource> Loc { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _filterOperatorOptions = FilterOperators
                .Select(x => new Option<FilterOperatorEnum> { Text = x, Value = x, Selected = x == FilterOperator })
                .ToArray();
        }

        private void SelectedOptionChanged(Option<FilterOperatorEnum> option)
        {
            FilterOperator = option?.Value;
            FilterOperatorChanged.InvokeAsync(FilterOperator);
        }
    }
}
