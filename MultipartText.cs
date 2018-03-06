using System;
using System.Collections.Generic;
using System.Text;

namespace HttpUtility
{
    /// <summary>
    /// 用于多部分数据的文本类
    /// </summary>
    public class MultipartText
    {
        /// <summary>
        /// 用于取回数据的标识
        /// </summary>
        public string HttpName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 编码。默认UTF-8
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 媒体类型。表示是什么格式的数据，默认text/plain。对应于Http内容当中的Content-Type
        /// </summary>
        public string MediaType { get; set; } = "text/plain";
    }



}
