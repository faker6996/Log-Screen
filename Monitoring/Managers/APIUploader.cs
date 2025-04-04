using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Monitoring.Utils;
using System.Windows.Forms;
using System.Windows;

namespace Monitoring.Managers
{
    /// <summary>
    /// Defines the response class from the API Check.
    /// </summary>
    public class ApiResponseGetCheck
    {
        [JsonPropertyName("value")]
        public int value { get; set; }
    }

    /// <summary>
    /// Defines the response class from the API Check.
    /// </summary>
    class ApiResponseSetCheck
    {
        [JsonPropertyName("success")]
        public string success { get; set; }
    }

    /// <summary>
    /// Defines the response class from the API Upload.
    /// </summary>
    public class ApiResponseUpload
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("files")]
        public List<FileResponse> Files { get; set; }
    }

    /// <summary>
    /// Class containing file information in the API upload response.
    /// </summary>
    public class FileResponse
    {
        [JsonPropertyName("file")]
        public string File { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// Class for managing file uploads to the API
    /// </summary>
    public class APIUploader
    {
        /// <summary>
        /// Uploads a list of files to the API and deletes successfully uploaded files.
        /// </summary>
        /// <param name="filePaths">List of file paths to upload.</param>
        /// <param name="osUUID">Unique identifier for the folder or operating system.</param>
        /// <param name="apiUrl">API URL.</param>
        /// <param name="authToken">Authentication token.</param>
        public async Task<string> UploadFileAsync(List<string> filePaths, string osUUID, string apiUrl, string authToken)
        {
            try
            {
                string captureDirectory = FileHelper.GetCaptureAddress();

                if (Directory.Exists(captureDirectory))
                {
                    // Find all .jpg files if filePaths is null or empty
                    if (filePaths == null || filePaths.Count == 0)
                    {
                        filePaths = new List<string>(Directory.GetFiles(captureDirectory, Setting.CAPTURE_FILE_EXTENTION));
                    }
                }
                else
                {
                    FileHelper.LogError("The image storage directory does not exist.");
                }

                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    // Add authentication token to the HTTP header
                    client.DefaultRequestHeaders.Add("token", authToken);

                    // Add folder ID (osUUID) to form-data
                    content.Add(new StringContent(osUUID), "folder_id");

                    // List of successfully uploaded files
                    List<string> successfullyUploadedFiles = new List<string>();

                    // Add each file from the upload list to form-data
                    foreach (var filePath in filePaths)
                    {
                        // Check if the file exists
                        if (File.Exists(filePath))
                        {
                            // Create StreamContent to read the file content
                            //var fileStream = new StreamContent(File.OpenRead(filePath));
                            var fileStream = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous));
                            fileStream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                            content.Add(fileStream, "files[]", Path.GetFileName(filePath)); // Attach the file to form-data
                        }
                        else
                        {
                            // Display a message if the file is not found
                            FileHelper.LogError($"File not found: {filePath}");
                        }
                    }

                    // Send a POST request to the API
                    var response = await client.PostAsync(apiUrl, content);

                    // Read the JSON response from the API
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseUpload>(jsonResponse);

                    // Check if the response is successful
                    if (apiResponse != null && apiResponse.Success)
                    {
                        // Iterate through the list of processed files in the API response
                        foreach (var file in apiResponse.Files)
                        {
                            successfullyUploadedFiles.Add(file.File);// Save successfully uploaded file
                        }

                        // Delete successfully uploaded files from the system
                        foreach (var fileName in successfullyUploadedFiles)
                        {
                            // Combine the directory path with the file name
                            string fullFilePath = Path.Combine(captureDirectory, fileName);

                            if (File.Exists(fullFilePath))
                            {
                                File.Delete(fullFilePath);
                            }
                            else
                            {
                                FileHelper.LogError($"File not found: {fullFilePath}");
                            }
                        }

                    }
                    else
                    {
                        FileHelper.LogError("Request failed or no files uploaded.");

                        // Deserialize JSON response
                        var responseData = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                        // Check for invalid token
                        if (responseData != null && responseData.error == Setting.TOKEN_INVALID)
                        {
                            return Setting.TOKEN_INVALID;
                        }
                        else
                        {
                            MessageBoxHelper.ShowMessageServerError(Setting.MESSAGE_SERVER_ERROR, Setting.MESSAGE_TITLE);
                        }
                    }
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Sends a request to the API with dynamic parameters and returns the response result.
        /// </summary>
        /// <param name="osUUID">Folder ID</param>
        /// <param name="apiUrl">API URL</param>
        /// <param name="authToken">Authentication token</param>
        /// <param name="modeCheck">Check mode (get_value)</param>
        /// <returns>An ApiResponseGetCheck object containing the response information</returns>
        public async Task<int> GetCheckTimer(string osUUID, string apiUrl, string authToken, string modeCheck)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string requestUrl = $"{apiUrl}?folder_id={osUUID}&method={modeCheck}";

                    client.DefaultRequestHeaders.Add("token", authToken);
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Cắt bỏ phần trước chuỗi {"value":0}
                    int startIndex = responseContent.IndexOf("{\"value\":0}");
                    if (startIndex != -1)
                    {
                        // Trả về phần còn lại của chuỗi
                        responseContent = responseContent.Substring(startIndex);
                    }

                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return apiResponse.value;
                    }
                    else
                    {
                        FileHelper.LogError($"Error When Call API:{requestUrl} - {response.StatusCode} - {responseContent}");

                        // Check for invalid token
                        if (apiResponse != null && apiResponse.error == Setting.TOKEN_INVALID)
                        {
                            return 401;// lỗi token
                        }
                        else
                        {
                            FileHelper.LogError($"Error When Call API");
                            throw new Exception();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowMessageServerError(Setting.MESSAGE_SERVER_ERROR, Setting.MESSAGE_TITLE);
                FileHelper.LogError($"Error When Call API: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Sends a request to the API with dynamic parameters and returns the response result.
        /// </summary>
        /// <param name="osUUID">Folder ID</param>
        /// <param name="apiUrl">API URL</param>
        /// <param name="authToken">Authentication token</param>
        /// <param name="modeCheck">Check mode (set_value)</param>
        /// <param name="dataValue">Data value</param>
        /// <returns>true if successful</returns>
        public async Task<int> SetCheckTimer(string osUUID, string apiUrl, string authToken, string modeCheck, int dataValue)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string requestUrl = $"{apiUrl}?folder_id={osUUID}&method={modeCheck}";

                    if (modeCheck == Setting.MODE_CHECK_TIMER.SET)
                    {
                        requestUrl += $"&data_value={dataValue}";
                    }

                    client.DefaultRequestHeaders.Add("token", authToken);
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        ApiResponseSetCheck apiResponse = JsonConvert.DeserializeObject<ApiResponseSetCheck>(responseContent);
                        return apiResponse.success == "check.txt set by data_value to 0" ? 1 : 0;
                    }
                    else
                    {
                        FileHelper.LogError($"Error When Call API:{requestUrl} - {response.StatusCode} - {responseContent}");

                        // Deserialize JSON response
                        var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        // Check for invalid token
                        if (responseData != null && responseData.error == Setting.TOKEN_INVALID)
                        {
                            return 401;// lỗi token
                        }
                        else
                        {
                            FileHelper.LogError($"Error When Call API");
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                FileHelper.LogError($"Error When Call API: {ex.Message}");
                MessageBoxHelper.ShowMessageServerError(Setting.MESSAGE_SERVER_ERROR, Setting.MESSAGE_TITLE);
                return -1;
            }
        }

    }
}