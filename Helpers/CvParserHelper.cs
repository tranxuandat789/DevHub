using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DevHub.Helpers
{
    public class CvParserHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public CvParserHelper(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? configuration["Gemini__ApiKey"] ?? "";
        }

        public async Task<string> ParseCvToJsonAsync(string physicalPath)
        {
            if (!File.Exists(physicalPath))
            {
                return "{\"error\": \"Không tìm thấy file CV trên máy chủ.\"}";
            }

            var extension = Path.GetExtension(physicalPath).ToLowerInvariant();
            string rawText = "";

            if (extension == ".docx")
            {
                rawText = ExtractTextFromDocx(physicalPath);
            }
            else
            {
                return "{\"error\": \"Định dạng file không được hỗ trợ để trích xuất text. Tính năng thử nghiệm hiện chỉ hỗ trợ .docx\"}";
            }

            if (string.IsNullOrWhiteSpace(rawText))
            {
                return "{\"error\": \"Không thể trích xuất nội dung từ CV.\"}";
            }

            // Gọi Gemini API
            return await CallGeminiApiAsync(rawText);
        }

        private string ExtractTextFromDocx(string path)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, false))
                {
                    var body = wordDoc.MainDocumentPart?.Document.Body;
                    if (body != null)
                    {
                        foreach (var paragraph in body.Elements<Paragraph>())
                        {
                            sb.AppendLine(paragraph.InnerText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc file docx: " + ex.Message);
            }
            return sb.ToString();
        }

        private async Task<string> CallGeminiApiAsync(string rawText)
        {
            try
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

                var prompt = "Hãy đóng vai một chuyên gia nhân sự. Trích xuất thông tin từ đoạn văn bản CV sau đây và trả về định dạng JSON thuần túy (chỉ JSON, không bao gồm code block markdown ```json hay bất kỳ văn bản nào khác). Cấu trúc JSON cần có các trường chính: 'Thông tin cá nhân', 'Học vấn', 'Kinh nghiệm', 'Kỹ năng', 'Chứng chỉ', 'Khác' (nếu có). NẾU TRƯỜNG NÀO KHÔNG CÓ THÔNG TIN THÌ ĐỂ TRỐNG (null).\n\nĐoạn văn bản:\n" + rawText;

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("X-goog-api-key", _apiKey);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseJson);
                    
                    var root = doc.RootElement;
                    if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        var parts = candidates[0].GetProperty("content").GetProperty("parts");
                        if (parts.GetArrayLength() > 0)
                        {
                            var textResult = parts[0].GetProperty("text").GetString();
                            
                            if (textResult != null)
                            {
                                textResult = textResult.Trim();
                                if (textResult.StartsWith("```json"))
                                {
                                    textResult = textResult.Substring(7);
                                }
                                if (textResult.StartsWith("```"))
                                {
                                    textResult = textResult.Substring(3);
                                }
                                if (textResult.EndsWith("```"))
                                {
                                    textResult = textResult.Substring(0, textResult.Length - 3);
                                }
                                return textResult.Trim();
                            }
                        }
                    }
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    return "{\"error\": \"Hệ thống đọc cv bằng AI hiện không khả dụng. Vui lòng thử lại sau!\"}";
                }
            }
            catch (Exception)
            {
                return "{\"error\": \"Hệ thống đọc cv bằng AI hiện không khả dụng. Vui lòng thử lại sau!\"}";
            }
            
            return "{\"error\": \"Hệ thống đọc cv bằng AI hiện không khả dụng. Vui lòng thử lại sau!\"}";
        }
    }
}
