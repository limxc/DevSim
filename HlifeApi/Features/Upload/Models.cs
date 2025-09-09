using Newtonsoft.Json;

namespace Upload
{
    internal sealed class Request
    {
        /// <see cref="Query.Archive.Guid" />
        public string Hzguid { get; set; }

        /// <see cref="Query.Archive.CardId" />
        public string CardId { get; set; }

        /// <see cref="Query.Archive.SampleId" />
        public string SampleId { get; set; }

        /// <summary>
        /// 设备名
        /// </summary>
        public string DevicName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DevicType { get; set; }

        /// <summary>
        /// 报告单名
        /// </summary>
        public string FrxName { get; set; }

        /// <summary>
        /// 男/女
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public byte[] Pdfdata { get; set; }

        public static Request Mock()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Files", "upload-bmd.json");
            var json = File.ReadAllText(jsonPath);
            var req = JsonConvert.DeserializeObject<Request>(json);
            return req;
        }
    }

    internal sealed class Response
    {
        public bool State { get; set; }
        public string Exception { get; set; }
    }
}