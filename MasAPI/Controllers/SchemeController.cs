using MasAPI.Models;
using System.Text.Json;
using static MasAPI.MasAPIServer;
using static MasAPI.RequestParser;

namespace MasAPI.Controllers
{
    internal class SchemeController
    {
        private static string MasRoot => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mas");

        internal static async Task<ApiResponse> GetScheme(ApiRequest request)
        {
            var scheme = new Scheme();
            scheme.LoadScheme(MasRoot);
            var response = new ApiResponse { StatusCode = 200, ContentType = "application/json", Body = JsonSerializer.Serialize(scheme, JsonSerializerOptions.Default), };
            return await Task.FromResult(response);
        }
    }
}
