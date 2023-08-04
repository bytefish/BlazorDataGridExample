﻿using BlazorDataGridExample.Shared.Models;
using Microsoft.Fast.Components.FluentUI;
using SortDirection = BlazorDataGridExample.Shared.Models.SortDirection;
using FluentUiSortDirection = Microsoft.Fast.Components.FluentUI.SortDirection;

namespace BlazorDataGridExample.Infrastructure
{
    /// <summary>
    /// Provides Converters for converting from Fluent UI to Model classes 
    /// and vice versa.
    /// </summary>
    public static class Converters
    {
         public static List<SortColumn> ConvertToSortColumns(IReadOnlyCollection<SortedProperty>? source)
         {
            if (source == null)
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
