﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniVerse.Models;

namespace UniVerse.Services
{
    public class RestService: IRestService
    {
        HttpClient _client;

        //base api URL 
        internal string baseURL = "https://localhost:7050/api/";
        JsonSerializerOptions _serializerOptions;
        public List <Person> Lecturers { get; private set; }
        public List<Person> Students { get; private set; }
        public AuthenticatedUser AuthenticatedUser { get; private set; }

        public RestService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task RefreshDataAsync()
        {
            await LoadLecturersAsync();
            await LoadStudentsAsync();
        }

        public async Task LoadLecturersAsync()
        {
            Uri lecturersUri = new (string.Format(baseURL + "People/Lecturers"));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(lecturersUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Lecturers = JsonSerializer.Deserialize<List<Person>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        public async Task LoadStudentsAsync()
        {
            Uri studentsUri = new(string.Format(baseURL + "People/Students"));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(studentsUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Students = JsonSerializer.Deserialize<List<Person>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }


        // authenticate users
        public async Task<AuthenticatedUser> PostDataAsync(string email, string password)
        {
            AuthenticatedUser AuthenticatedUser = null;
            Uri uri = new (string.Format(baseURL + "People/auth"));
            var requestData = new
            {
                email,
                password
            };

            var json = JsonSerializer.Serialize(requestData, _serializerOptions);
            StringContent stringContent = new(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage res = await _client.PostAsync(uri, stringContent);
                if(res.IsSuccessStatusCode)
                {
                   string responseContent = await res.Content.ReadAsStringAsync();
                   AuthenticatedUser = JsonSerializer.Deserialize<AuthenticatedUser>(responseContent, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return AuthenticatedUser;
        }

        Task<List<Person>> IRestService.RefreshDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}
