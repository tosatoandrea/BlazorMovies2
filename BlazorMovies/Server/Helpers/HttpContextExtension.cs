using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazorMovies.Server.Helpers
{
    public static class HttpContextExtension
    {
        public static async Task InsertPaginationParametersInResponse<T>(this HttpContext httpContext, IQueryable<T> query, 
            int recordPerPage)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            double count = await query.CountAsync();
            double totalAmountOfPages = Math.Ceiling(count / recordPerPage);

            httpContext.Response.Headers.Add("totalAmountOfPages", totalAmountOfPages.ToString());
        }
    }
}
