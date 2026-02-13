using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BirthCenter.DataLoader.Models;
using Microsoft.Extensions.Logging;

namespace BirthCenter.DataLoader.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> CreatePatientAsync(GeneratedPatient patient)
        {
            try
            {
                var json = JsonSerializer.Serialize(patient, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/patients", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully created patient: {Family}", patient.Name.Family);
                    return true;
                }

                _logger.LogError("Failed to create patient: {Family}. Status: {StatusCode}",
                    patient.Name.Family, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating patient: {Family}", patient.Name.Family);
                return false;
            }
        }
    }
}