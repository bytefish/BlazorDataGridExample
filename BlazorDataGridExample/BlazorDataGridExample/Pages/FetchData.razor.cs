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
    public partial class FetchData
    {
        /// <summary>
        /// Provides the Data Items.
        /// </summary>
        private GridItemsProvider<Customer> CustomerProvider = default!;

        /// <summary>
        /// DataGrid.
        /// </summary>
        private FluentDataGrid<Customer> CustomerDataGrid = default!;

        /// <summary>
        /// The current Pagination State.
        /// </summary>
        private readonly PaginationState Pagination = new() { ItemsPerPage = 10 };

        /// <summary>
        /// The current Filter State.
        /// </summary>
        private readonly FilterState Filters = new();

        /// <summary>
        /// Reacts on Paginator Changes.
        /// </summary>
        private readonly EventCallbackSubscriber<FilterState> CurrentFiltersChanged;

        public FetchData()
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
            CurrentFiltersChanged.SubscribeOrMove(Filters.CurrentFiltersChanged);

            return Task.CompletedTask;
        }

        private Task RefreshData()
        {
            return CustomerDataGrid.RefreshDataAsync();
        }

        private async Task<QueryOperationResponse<Customer>> GetCustomers(GridItemsProviderRequest<Customer> request)
        {
            var sortColumns = ConvertSortColumns(request);

            var dataServiceQuery = GetDataServiceQuery(sortColumns, Pagination.CurrentPageIndex, Pagination.ItemsPerPage);

            var result = await dataServiceQuery.ExecuteAsync(request.CancellationToken);

            return (QueryOperationResponse<Customer>)result;
        }

        private DataServiceQuery<Customer> GetDataServiceQuery(SortColumn[] sortColumns, int pageNumber, int pageSize)
        {
            var query = Container.Customers.Expand(x => x.LastEditedByNavigation)
                    .Page(pageNumber + 1, pageSize)
                .SortBy(sortColumns)
                .IncludeCount(true);

            return (DataServiceQuery<Customer>)query;
        }

        private static SortColumn[] ConvertSortColumns(GridItemsProviderRequest<Customer> request)
        {
            var sortByProperties = request.GetSortByProperties();

            return ConvertSortColumns(sortByProperties);
        }

        private static SortColumn[] ConvertSortColumns(IReadOnlyCollection<SortedProperty>? source)
        {
            if(source == null)
            {
                return Array.Empty<SortColumn>();
            }

            return source
                .Select(x => ConvertSortColumn(x))
                .ToArray();
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
