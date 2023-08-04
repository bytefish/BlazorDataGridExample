using BlazorDataGridExample.Components;
using BlazorDataGridExample.Infrastructure;
using BlazorDataGridExample.Shared.Extensions;
using BlazorDataGridExample.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.OData.Client;
using WideWorldImportersService;
using SortDirection = BlazorDataGridExample.Shared.Models.SortDirection;
using FluentUiSortDirection = Microsoft.Fast.Components.FluentUI.SortDirection;

namespace BlazorDataGridExample.Pages
{
    public partial class VehicleTemperaturesDataGrid
    {
        /// <summary>
        /// Provides the Data Items.
        /// </summary>
        private GridItemsProvider<VehicleTemperature> VehicleTemperatureProvider = default!;

        /// <summary>
        /// DataGrid.
        /// </summary>
        private FluentDataGrid<VehicleTemperature> DataGrid = default!;

        /// <summary>
        /// The current Pagination State.
        /// </summary>
        private readonly PaginationState Pagination = new() { ItemsPerPage = 10 };

        /// <summary>
        /// The current Filter State.
        /// </summary>
        private readonly FilterState FilterState = new();

        /// <summary>
        /// Reacts on Paginator Changes.
        /// </summary>
        private readonly EventCallbackSubscriber<FilterState> CurrentFiltersChanged;

        public VehicleTemperaturesDataGrid()
        {
            CurrentFiltersChanged = new(EventCallback.Factory.Create<FilterState>(this, RefreshData));
        }

        protected override Task OnInitializedAsync()
        {
            VehicleTemperatureProvider = async request =>
            {
                var response = await GetVehicleTemperatures(request);

                return GridItemsProviderResult.From(items: response.ToList(), totalItemCount: (int)response.Count);
            };
            
            return base.OnInitializedAsync();
        }

        /// <inheritdoc />
        protected override Task OnParametersSetAsync()
        {
            // The associated filter state may have been added/removed/replaced
            CurrentFiltersChanged.SubscribeOrMove(FilterState.CurrentFiltersChanged);

            return Task.CompletedTask;
        }

        private Task RefreshData()
        {
            return DataGrid.RefreshDataAsync();
        }

        private async Task<QueryOperationResponse<VehicleTemperature>> GetVehicleTemperatures(GridItemsProviderRequest<VehicleTemperature> request)
        {
            var sorts = ConvertSortColumns(request);
            var filters = FilterState.Filters.Values.ToList();

            var dataServiceQuery = GetDataServiceQuery(sorts, filters, Pagination.CurrentPageIndex, Pagination.ItemsPerPage);

            var result = await dataServiceQuery.ExecuteAsync(request.CancellationToken);

            return (QueryOperationResponse<VehicleTemperature>)result;
        }

        private DataServiceQuery<VehicleTemperature> GetDataServiceQuery(List<SortColumn> sortColumns, List<FilterDescriptor> filters,  int pageNumber, int pageSize)
        {
            var query = Container.VehicleTemperatures
                .Page(pageNumber + 1, pageSize)
                .Filter(filters)
                .SortBy(sortColumns)
                .IncludeCount(true);

            return (DataServiceQuery<VehicleTemperature>)query;
        }

        private static List<SortColumn> ConvertSortColumns(GridItemsProviderRequest<VehicleTemperature> request)
        {
            var sortByProperties = request.GetSortByProperties();

            return ConvertSortColumns(sortByProperties);
        }

        private static List<SortColumn> ConvertSortColumns(IReadOnlyCollection<SortedProperty>? source)
        {
            if(source == null)
            {
                return new();
            }

            return source
                .Select(x => ConvertSortColumn(x))
                .ToList();
        }

        private static SortColumn ConvertSortColumn(SortedProperty source)
        {
            var sortDirection = ConvertSortDirection(source.Direction);

            return new SortColumn
            {
                PropertyName = source.PropertyName,
                SortDirection = sortDirection
            };
        }

        private static SortDirection ConvertSortDirection(FluentUiSortDirection source)
        {
            if (source == FluentUiSortDirection.Ascending)
            {
                return SortDirection.Ascending;
            }

            return SortDirection.Descending;
        }

    }
}
