using BlazorMovies.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Helpers
{
    public static class IHttpServiceExtension
    {
        public static async Task<T> GetHelper<T>(this IHttpService httpService, string url)
        {
            var response = await httpService.Get<T>(url);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public static async Task<PaginatedResponse<T>> GetHelper<T>(this IHttpService httpService, string url, PaginationDTO paginationDTO)
        {
            string newUrl = "";
            if (url.Contains("?"))
            {
                newUrl = $"{url}&page={paginationDTO.Page}&recordPerPage={paginationDTO.RecordPerPage}";
            }
            else
            {
                newUrl = $"{url}?page={paginationDTO.Page}&recordPerPage={paginationDTO.RecordPerPage}";
            }

            var httpResponse = await httpService.Get<T>(newUrl);
            var totalAmountOfPages = int.Parse(httpResponse.HttpResponseMessage.Headers.GetValues("totalAmountOfPages").FirstOrDefault());
            var paginatedResponse = new PaginatedResponse<T>
            {
                Response = httpResponse.Response,
                TotalAmountOfPages = totalAmountOfPages
            };
            return paginatedResponse;
        }
    }
}
