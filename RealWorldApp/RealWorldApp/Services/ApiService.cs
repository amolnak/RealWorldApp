﻿using Newtonsoft.Json;
using FoodApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Net.Http.Headers;
using UnixTimeStamp;

namespace FoodApp.Services
{
    public static class ApiService
    {
        public static async Task<bool> RegisterUser(string name, string email, string password, string Fname, string Lname, string Pn1, string Pn2, string Address, string Gender, string DOB, Guid City, Guid Region,int age)
        {
            var register = new Users()
            {
                Username = name,
                Email = email,
                Password = password,
                FName = Fname,
                LName = Lname,
                Phone1 = Pn1,
                Phone2 = Pn2,
                Address = Address,
                Gender = Gender,
                Age = age,
                BirthDate = DateTime.Parse(DOB),
                DateRegistered = DateTime.Now,
                CityID = City,
                RegionID = Region,
                Active = "Y"

            };
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(register);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Users/Register", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<bool> Login(string username, string password)
        {
            var login = new Login()
            {
                UserName = username,
                Password = password
            };

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Users/Login", content);
            if (!response.IsSuccessStatusCode) return false;
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Token>(jsonResult);
            Preferences.Set("accessToken", result.access_token);
            Preferences.Set("tokenType", result.token_type);
            Preferences.Set("userId", result.user_Id);
            Preferences.Set("adminID", result.user_Id);
            Preferences.Set("userName", result.user_name);
            Preferences.Set("tokenExpirationTime", result.expiration_Time);
            Preferences.Set("currentTime", UnixTime.GetCurrentTime());
            return true;
        }

        public static async Task<List<Category>> GetCategories()
        {
            try
            {
                await TokenValidator.CheckTokenValidity();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
                var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Categories");
                return JsonConvert.DeserializeObject<List<Category>>(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Product> GetProductById(int productId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Products/" + productId);
            return JsonConvert.DeserializeObject<Product>(response);
        }

        public static async Task<List<ProductByCategory>> GetProductByCategory(int categoryId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Products/ProductsByCategory/" + categoryId);
            return JsonConvert.DeserializeObject<List<ProductByCategory>>(response);
        }

        public static async Task<List<PopularProduct>> GetPopularProducts()
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Products/PopularProducts");
            return JsonConvert.DeserializeObject<List<PopularProduct>>(response);
        }

        public static async Task<bool> AddItemsInCart(AddToCart addToCart)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(addToCart);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/ShoppingCartItems", content);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<CartSubTotal> GetCartSubTotal(Guid userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/ShoppingCartItems/SubTotal/" + userId);
            return JsonConvert.DeserializeObject<CartSubTotal>(response);
        }

        public static async Task<List<ShoppingCartItem>> GetShoppingCartItems(Guid userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/ShoppingCartItems/" + userId);
            return JsonConvert.DeserializeObject<List<ShoppingCartItem>>(response);
        }

        public static async Task<TotalCartItem> GetTotalCartItems(Guid userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/ShoppingCartItems/TotalItems/" + userId);
            return JsonConvert.DeserializeObject<TotalCartItem>(response);
        }

        public static async Task<bool> ClearShoppingCart(Guid userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrl + "api/ShoppingCartItems/" + userId);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<OrderResponse> PlaceOrder(Order order)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/Orders", content);
            var jsonResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OrderResponse>(jsonResult);
        }

        public static async Task<List<OrderByUser>> GetOrdersByUser(Guid userId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Orders/OrdersByUser/" + userId);
            return JsonConvert.DeserializeObject<List<OrderByUser>>(response);
        }

        public static async Task<List<Order>> GetOrderDetails(int orderId)
        {
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Orders/OrderDetails/" + orderId);
            return JsonConvert.DeserializeObject<List<Order>>(response);
        }
        //amol naik
        public static async Task<List<Region>> GetRegions()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Region/GetRegion");
            return JsonConvert.DeserializeObject<List<Region>>(response);
        }

        public static async Task<List<City>> GetCitys()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/City/GetCity");
            return JsonConvert.DeserializeObject<List<City>>(response);
        }

        //public static async Task<List<Category>> GetCategories()
        //{
        //    await TokenValidator.CheckTokenValidity();
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
        //    var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/Categories");
        //    return JsonConvert.DeserializeObject<List<Category>>(response);
        //}
    }

    public static class TokenValidator
    {
        public static async Task CheckTokenValidity()
        {
            var expirationTime = Preferences.Get("tokenExpirationTime", 0);
            Preferences.Set("currentTime", UnixTime.GetCurrentTime());
            var currentTime = Preferences.Get("currentTime", 0);
            if (expirationTime < currentTime)
            {
                var email = Preferences.Get("email", string.Empty);
                var password = Preferences.Get("password", string.Empty);
                await ApiService.Login(email, password);
            }
        }
    }

}
