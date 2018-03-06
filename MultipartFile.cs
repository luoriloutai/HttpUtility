using System;
using System.Collections.Generic;
using System.Text;

namespace HttpUtility
{
    /// <summary>
    /// 用于多部分数据的文件类
    /// </summary>
    public class MultipartFile
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
}
