using BlazorDataGridExample.Components;
using BlazorDataGridExample.Infrastructure;
using BlazorDataGridExample.Shared.Extensions;
using BlazorDataGridExample.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.OData.Client;
using System.Net.NetworkInformation;
using WideWorldImportersService;
using SortDirection = BlazorDataGridExample.Shared.Models.SortDirection;
using FluentUiSortDirection = Microsoft.Fast.Components.FluentUI.SortDirection;

namespace BlazorDataGridExample.Pages
{
    public partial class CustomerDataGrid
    {
        /// <summary>
        /// Provides the Data Items.
        /// </summary>
        private GridItemsProvider<Customer> CustomerProvider = default!;

        /// <summary>
        /// DataGrid.
        /// </summary>
        private FluentDataGrid<Customer> DataGrid = default!;

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

        public CustomerDataGrid()
        {
            CurrentFiltersChanged = new(EventCallback.Factory.Create<FilterState>(this, RefreshData));
        }

        protected override Task OnInitializedAsync()
        {
            CustomerProvider = async request =>
            {
                var response = await GetCustomers(request);

                return GridItemsProviderResult.From(
                    items: response.ToList(),
                    totalItemCount: (int)response.Count);
            };
            
            return base.OnInitializedAsync();
        }

        /// <inheritdoc />
        protected override Task OnParametersSetAsync()
        {
            // The associated pagination state may have been added/removed/replaced
            CurrentFiltersChanged.SubscribeOrMove(FilterState.CurrentFiltersChanged);

            return Task.CompletedTask;
        }

        private Task RefreshData()
        {
            return Task.CompletedTask;
            //return DataGrid.RefreshDataAsync();
        }

        private async Task<QueryOperationResponse<Customer>> GetCustomers(GridItemsProviderRequest<Customer> request)
        {
            var sorts = ConvertSortColumns(request);
            var filters = FilterState.Filters.Values.ToList();

            var dataServiceQuery = GetDataServiceQuery(sorts, filters, Pagination.CurrentPageIndex, Pagination.ItemsPerPage);

            var result = await dataServiceQuery.ExecuteAsync(request.CancellationToken);

            return (QueryOperationResponse<Customer>)result;
        }

        private DataServiceQuery<Customer> GetDataServiceQuery(List<SortColumn> sortColumns, List<FilterDescriptor> filters,  int pageNumber, int pageSize)
        {
            var query = Container.Customers.Expand(x => x.LastEditedByNavigation)
                .Page(pageNumber + 1, pageSize)
                .Filter(filters)
                .SortBy(sortColumns)
                .IncludeCount(true);

            return (DataServiceQuery<Customer>)query;
        }

        private static List<SortColumn> ConvertSortColumns(GridItemsProviderRequest<Customer> request)
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
