﻿using System.Diagnostics;
using System.Text.Json;
using UniVerse.Models;
using UniVerse.Services.SubjectService;

namespace UniVerse.Services.SubjectServices
{
    public class SubjectService : ISubjectService
    {
        HttpClient _client;
        //base api URL 
        internal string baseURL = "https://localhost:7050/api/";
        JsonSerializerOptions _serializerOptions;

        public List<SubjectWithLecturerModel> Subjects { get; private set; }

        public SubjectService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        //get subjects
        public async Task<List<SubjectWithLecturerModel>> GetSubjectsAsync()
        {
            Subjects = new List<SubjectWithLecturerModel>();
            Uri uri = new(string.Format(baseURL + "Subjects"));

            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Subjects = JsonSerializer.Deserialize<List<SubjectWithLecturerModel>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Subjects;
        }

        //Get subjects by id
        public async Task<SubjectModel> GetSubjectByIdAsync(int id)
        {
            SubjectModel subject = new();
            Uri studentUri = new(string.Format(baseURL + "Subjects/{0}", id));

            try
            {
                HttpResponseMessage response = await _client.GetAsync(studentUri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    subject = JsonSerializer.Deserialize<SubjectModel>(content, _serializerOptions);
                    Debug.WriteLine($"Name: {subject.subject_name}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return subject;
        }
    }
}