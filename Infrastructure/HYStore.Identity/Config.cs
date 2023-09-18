// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace HYStore.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalogapi"),
                new ApiScope("basketapi"),
                new ApiScope("catalogapi.read"),
                new ApiScope("catalogapi.write"),
                new ApiScope("hystoregateway")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                //List of Microservices can go here.
                new ApiResource("Catalog","Catalog.API")
                {
                    // Scopes = {"catalogapi.read","catalogapi.write"}
                    Scopes = {"catalogapi"}
                },
                new ApiResource("Basket","Basket.API")
                {
                    Scopes = {"basketapi"}
                },
                new ApiResource("HYStoreGateway","HYStore Gateway")
                {
                    Scopes = {"hystoregateway"}
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                //m2m flow
                new Client
                {
                    ClientName = "Catalog API Client",
                    ClientId = "CatalogApiClient",
                    ClientSecrets = {new Secret("123456".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // AllowedScopes = {"catalogapi.read","catalogapi.write"}
                    AllowedScopes = {"catalogapi"}
                },
                new Client
                {
                    ClientName = "Basket API Client",
                    ClientId = "BasketApiClient",
                    ClientSecrets = {new Secret("1234561".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"basketapi"}
                },
                new Client
                {
                    ClientName = "HYStore Gateway Client",
                    ClientId = "HYStoreGatewayClient",
                    ClientSecrets = {new Secret("12345612".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"hystoregateway","basketapi"}
                }
            };
    }
}