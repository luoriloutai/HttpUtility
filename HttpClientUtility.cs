﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpUtility
{
    public class HttpClientUtility
    {
        private HttpClient client;

        // 私有构造函数，阻止外部创建对象
        private HttpClientUtility()
        {
            if (client == null)
            {
                client = new HttpClient();
            }
        }


        /// <summary>
        /// 创建HttpClientUtility的实例
        /// </summary>
        /// <returns></returns>
        public static HttpClientUtility Create()
        {
            return new HttpClientUtility();
        }


        /// <summary>
        /// 添加请求头部
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public void AddRequestHeader(string name, string value)
        {
            if (client != null)
            {
                client.DefaultRequestHeaders.Add(name, value);
            }
        }


        /// <summary>
        /// 移除指定名称的请求头部
        /// </summary>
        /// <param name="name"></param>
        public void RemoveRequestHeader(string name)
        {
            if (client != null)
            {
                client.DefaultRequestHeaders.Remove(name);
            }
        }


        /// <summary>
        /// 提交多部分内容（包含文本字段和文件）表单内容。用于提交文本和文件混合的情况。该方法支持只包含文本内容或只包含文件的情况，不需要的部分赋值为null即可，但不能两部分都为null
        /// </summary>
        /// <param name="requestUrl">请求Url</param>
        /// <param name="formTextContents">文本字段内容。如果不需要提交文本，则该参数赋为null即可，但文本与文件必须存在一个，不能同时为null</param>
        /// <param name="formFiles">文件。如果不需要提交文件，则该参数赋为null，但文本与文件必须存在一个，不能同时为null</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostMultipartContentAsync(string requestUrl, List<MultipartText> formTextContents, List<MultipartLocalFile> formFiles)
        {
            if (formTextContents == null && formFiles == null)
            {
                throw new Exception("文本内容与文件必须存在一个");
            }

            using (var multipartContent = new MultipartFormDataContent())
            {
                // 向请求添加文本内容
                if (formTextContents != null)
                {
                    foreach (var textContent in formTextContents)
                    {
                        multipartContent.Add(new StringContent(textContent.Content, textContent.Encoding, textContent.MediaType));
                    }
                }

                // 在文件上传到服务器端还未成功返回的时候，流不可以关闭，
                // 因此定义集合收集所有打开的文件流，待操作完毕后关闭它们
                var fileStreams = new List<Stream>();

                // 向请求添加文件
                if (formFiles != null)
                {
                    foreach (var file in formFiles)
                    {
                        var stream = File.Open(file.FileLocalPath, FileMode.Open);
                        var streamContent = new StreamContent(stream);
                        multipartContent.Add(streamContent, file.HttpName, Path.GetFileName(file.FileLocalPath));
                        fileStreams.Add(stream);
                    }
                }

                if (multipartContent.Count() == 0)
                {
                    throw new ArgumentException("没有检测到上传的文件");
                }
                var resp = await client.PostAsync(requestUrl, multipartContent);
                var msg = resp.EnsureSuccessStatusCode();

                // 关闭打开的文件流
                foreach (var s in fileStreams)
                {
                    s.Close();
                }
                return msg;
            }
        }


        /// <summary>
        /// 提交多部分内容（包含文本字段和文件）表单内容。用于提交文本和文件混合的情况。该方法支持只包含文本内容或只包含文件的情况，不需要的部分赋值为null即可，但不能两部分都为null
        /// </summary>
        /// <param name="requestUrl">请求Url</param>
        /// <param name="formTextContents">文本字段内容。如果不需要提交文本，则该参数赋为null即可，但文本与文件必须存在一个，不能同时为null</param>
        /// <param name="formInputFiles">包含文件流的文件内容。所需数据从HttpContext.Current.Request.Files中可以获取</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostMultipartContentAsync(string requestUrl, List<MultipartText> formTextContents, List<MultiPartInputFile> formInputFiles)
        {
            if (formTextContents == null && formInputFiles == null)
            {
                throw new Exception("文本内容与文件必须存在一个");
            }

            using (var multipartContent = new MultipartFormDataContent())
            {
                // 向请求添加文本内容
                if (formTextContents != null)
                {
                    foreach (var textContent in formTextContents)
                    {
                        multipartContent.Add(new StringContent(textContent.Content, textContent.Encoding, textContent.MediaType));
                    }
                }

                var fileStreams = new List<Stream>();

                // 向请求添加文件
                if (formInputFiles != null)
                {
                    foreach (var file in formInputFiles)
                    {
                        var streamContent = new StreamContent(file.InputFileStream);
                        multipartContent.Add(streamContent, file.HttpName, file.FileName);
                        fileStreams.Add(file.InputFileStream);
                    }
                }

                if (multipartContent.Count() == 0)
                {
                    throw new ArgumentException("没有检测到提交的数据");
                }
                var resp = await client.PostAsync(requestUrl, multipartContent);
                var msg = resp.EnsureSuccessStatusCode();
                foreach (var s in fileStreams)
                {
                    s.Close();
                }
                return msg;
            }
        }


        /// <summary>
        /// 提交只包含文本的表单内容，形式为键值对，类型为application/x-www-form-urlencoded
        /// </summary>
        /// <param name="requestUrl">请求Url</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostFormUrlEncodedContentAsync(string requestUrl, IEnumerable<KeyValuePair<string, string>> content)
        {
            var bodyContent = new FormUrlEncodedContent(content);
            var resp = await client.PostAsync(requestUrl, bodyContent);
            var msg = resp.EnsureSuccessStatusCode();
            return msg;
        }


        /// <summary>
        /// Post提交纯字符串。一般为json字符串，可根据接收方实现方式自定义格式
        /// </summary>
        /// <param name="requestUrl">请求Url</param>
        /// <param name="content">字符串内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="mediaType">媒体类型，默认text/json，根据具体情况设置</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostStringContentAsync(string requestUrl, string content, Encoding encoding, string mediaType = "application/json")
        {
            var bodyContent = new StringContent(content, encoding, mediaType);
            var resp = await client.PostAsync(requestUrl, bodyContent);
            var msg = resp.EnsureSuccessStatusCode();
            return msg;
        }


        /// <summary>
        /// 以Json格式Post一个对象
        /// </summary>
        /// <param name="requestUrl">请求Url</param>
        /// <param name="obj">提交的对象</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostObjectInJsonFormatAsync(string requestUrl, object data, Encoding encoding)
        {
            var jsonContent = JsonConvert.SerializeObject(data);
            var bodyContent = new StringContent(jsonContent, encoding, "application/json");
            var resp = await client.PostAsync(requestUrl, bodyContent);
            var msg = resp.EnsureSuccessStatusCode();
            return msg;
        }


        /// <summary>
        /// Post请求，发送一个字符串
        /// </summary>
        /// <param name="requestUrl">请求Url</param>
        /// <param name="data">提交的字符串数据</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostStringAsync(string requestUrl, string data, Encoding encoding)
        {
            var bodyContent = new StringContent(data, encoding, "text/plain");
            var resp = await client.PostAsync(requestUrl, bodyContent);
            var msg = resp.EnsureSuccessStatusCode();
            return msg;
        }


        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="requestUrl">请求Url</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            var resp = await client.GetAsync(requestUrl);
            var msg = resp.EnsureSuccessStatusCode();
            return msg;
        }


        /// <summary>
        /// 下载小文件
        /// </summary>
        /// <param name="sourceUrl">资源地址</param>
        /// <param name="saveDir">保存到的目录</param>
        /// <param name="saveFileName">将要保存的文件名</param>
        /// <returns></returns>
        public async Task DownloadFile(string sourceUrl, string saveDir, string saveFileName = "")
        {
            var resp = await GetAsync(sourceUrl);
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException("请求资源出错");
            }
            if (string.IsNullOrWhiteSpace(saveFileName))
            {
                saveFileName = Path.GetFileNameWithoutExtension(sourceUrl);
            }
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            var filePath = saveDir + "/" + saveFileName + Path.GetExtension(sourceUrl);
            var fileStream = new FileStream(filePath, FileMode.Create);
            var downloadStream = await resp.Content.ReadAsStreamAsync();

            downloadStream.CopyTo(fileStream); // 用下面注释的方式也可以

            //var buffer = new byte[10240];  // 10K大小缓存
            //var readBytes = 0;
            //var readPosition = 0;
            //do
            //{
            //    readBytes = downloadStream.Read(buffer, 0, buffer.Length);
            //    fileStream.Write(buffer, 0, readBytes);
            //    readPosition += readBytes;
            //} while (readPosition < downloadStream.Length);


            fileStream.Close();
            downloadStream.Close();
        }
    }
}
