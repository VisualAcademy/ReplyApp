// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace ReplyApp.SingleSignOn
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            { 
                new ApiResource("ReplyApp.Apis", "ReplyApp.Apis")
            };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            { 
                new Client 
                { 
                    ClientId = "ReplyApp.Models.Tests",
                    AllowedScopes = { "ReplyApp.Apis" },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    { 
                        new Secret("ReplyApp.Apis".Sha256())
                    }
                }
            };        
    }
}
