using BlazorMovies.Client.Helpers;
using BlazorMovies.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Repository
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly string baseUrl = "api/accounts";
        private readonly IHttpService httpService;

        public AccountsRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<UserToken> Register(UserInfoDTO userInfo)
        {
            var httpResponse = await httpService.Post<UserInfoDTO, UserToken>($"{baseUrl}/create", userInfo);
            if (!httpResponse.Success)
            {
                throw new ApplicationException(await httpResponse.GetBody());
            }
            return httpResponse.Response;
        }

        public async Task<UserToken> Login(UserInfoDTO userInfo)
        {
            var httpResponse = await httpService.Post<UserInfoDTO, UserToken>($"{baseUrl}/login", userInfo);
            if (!httpResponse.Success)
            {
                throw new ApplicationException(await httpResponse.GetBody());
            }
            return httpResponse.Response;
        }

    }
}
