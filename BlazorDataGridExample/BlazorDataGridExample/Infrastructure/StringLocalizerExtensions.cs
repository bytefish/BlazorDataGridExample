﻿using Microsoft.Extensions.Localization;

namespace BlazorDataGridExample.Infrastructure
{
    public static class StringLocalizerExtensions
    {
        public static string TranslateEnum<TResource, TEnum>(this IStringLocalizer<TResource> localizer, TEnum enumValue)
        {
            var key = $"{typeof(TEnum).Name}_{enumValue}";

            var res = localizer.GetString(key);

            return res;
        }
    }
}
