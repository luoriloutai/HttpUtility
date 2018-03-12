using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpUtility
{
    /// <summary>
    /// 多部分数据的文件类，用于能拿到本地文件的绝对路径的情况，通常是本地文件上传
    /// </summary>
    public class MultipartLocalFile
    {
        /// <summary>
        /// 文件的名称，不是上传的本地文件的文件名，而是提交后以什么作为键去取回这个文件，即html中input元素的name属性值，接收方使用该值提取文件：Request.Files[HttpName]
        /// </summary>
        public string HttpName { get; set; }

        /// <summary>
        /// 要上传的本地文件路径
        /// </summary>
        public string FileLocalPath { get; set; }
    }


    /// <summary>
    /// 多部分数据的文件类，用于不能取到本地文件的绝对路径的情况，通常是网页上传
    /// </summary>
    public class MultiPartInputFile
    {
        /// <summary>
        /// 文件名称，用于取回文件的键值
        /// </summary>
        public string HttpName { get; set; }

        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 上传的文件流，一般是通过Html的input上传
        /// </summary>
        public Stream InputFileStream { get; set; }
    }
}
