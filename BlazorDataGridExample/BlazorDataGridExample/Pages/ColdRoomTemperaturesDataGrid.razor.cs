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
    public partial class ColdRoomTemperaturesDataGrid
    {
        /// <summary>
        /// Provides the Data Items.
        /// </summary>
        private GridItemsProvider<ColdRoomTemperature> ColdRoomTemperatureProvider = default!;

        /// <summary>
        /// DataGrid.
        /// </summary>
        private FluentDataGrid<ColdRoomTemperature> DataGrid = default!;

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

        public ColdRoomTemperaturesDataGrid()
        {
            CurrentFiltersChanged = new(EventCallback.Factory.Create<FilterState>(this, RefreshData));
        }

        protected override Task OnInitializedAsync()
        {
            ColdRoomTemperatureProvider = async request =>
            {
                var response = await GetCustomers(request);

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

        private async Task<QueryOperationResponse<ColdRoomTemperature>> GetCustomers(GridItemsProviderRequest<ColdRoomTemperature> request)
        {
            var sorts = ConvertSortColumns(request);
            var filters = FilterState.Filters.Values.ToList();

            var dataServiceQuery = GetDataServiceQuery(sorts, filters, Pagination.CurrentPageIndex, Pagination.ItemsPerPage);

            var result = await dataServiceQuery.ExecuteAsync(request.CancellationToken);

            return (QueryOperationResponse<ColdRoomTemperature>)result;
        }

        private DataServiceQuery<ColdRoomTemperature> GetDataServiceQuery(List<SortColumn> sortColumns, List<FilterDescriptor> filters,  int pageNumber, int pageSize)
        {
            var query = Container.ColdRoomTemperatures
                .Page(pageNumber + 1, pageSize)
                .Filter(filters)
                .SortBy(sortColumns)
                .IncludeCount(true);

            return (DataServiceQuery<ColdRoomTemperature>)query;
        }

        private static List<SortColumn> ConvertSortColumns(GridItemsProviderRequest<ColdRoomTemperature> request)
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
