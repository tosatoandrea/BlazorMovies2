using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Auth
{
    public class DummyAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //await Task.Delay(3000);
            var claims = new List<Claim>
            {
                new Claim("key1", "value1"),
                new Claim(ClaimTypes.Name, "Andrea"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            // se non viene specificato il secondo parametro l'utente risulta anonimo
            // var anonymous = new ClaimsIdentity(claims, "test"); // autorizzato perchè presente il secondo parametro
            var anonymous = new ClaimsIdentity(claims); // anonimo e quindi non autorizzato
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
        }
    }
}
