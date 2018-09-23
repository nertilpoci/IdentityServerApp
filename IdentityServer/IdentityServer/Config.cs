// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("nchat", "N-Chat APi ")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {

            return new Client[] {
                 new Client
            {
                ClientId = "nchatjavascriptclient",
                ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowedScopes = new List<string> { "openid", "profile", "nchat" },
                RedirectUris = new List<string> { "http://localhost:4200/auth-callback","http://chat.devhow.net/auth-callback" },
                PostLogoutRedirectUris = new List<string> { "http://localhost:4200/","http://chat.devhow.net" },
                AllowedCorsOrigins = new List<string> { "http://localhost:4200" ,"http://chat.devhow.net"},
                AllowAccessTokensViaBrowser = true
        }

    };



        }
    }
}