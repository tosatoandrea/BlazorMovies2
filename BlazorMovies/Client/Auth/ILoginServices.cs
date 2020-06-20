﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Auth
{
    public interface ILoginServices
    {
        Task Login(string token);
        Task Logout();
    }
}
