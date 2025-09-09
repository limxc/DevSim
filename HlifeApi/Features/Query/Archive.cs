using Bogus;

namespace Query
{
    internal sealed class Archive
    {
        public static Faker<Archive> Mock = new Faker<Archive>("zh_CN")
                .RuleFor(a => a.Guid, f => f.Person.Random.Guid().ToString("N"))      // 12 位：8 数字 + 4 字母
                .RuleFor(a => a.CardId, f => f.Random.Number(1, 999999).ToString("000000")) // 6 位数字
                .RuleFor(a => a.SampleId, f => f.Random.Number(1, 99999999).ToString("00000000")) // 8 位数字
                .RuleFor(a => a.Idnumber, f =>
                {
                    string addr = f.Address.ZipCode();                                      // 6 位地址码
                    string date = f.Date.Between(new DateTime(1980, 1, 1), new DateTime(2005, 12, 31)).ToString("yyyyMMdd");
                    string seq = f.Random.Number(0, 999).ToString("000");
                    int genderBit = f.Random.Bool() ? 1 : 0;                              // 1 男 0 女
                    seq = seq[..2] + (((seq[2] - '0') / 2 * 2) + genderBit);                 // 末位奇男偶女

                    string body = addr + date + seq;
                    int sum = 0;
                    for (int i = 0; i < 17; i++) sum += (body[i] - '0') * "7990586421379105842"[i] - '0';
                    char last = "10X98765432"[sum % 11];
                    string id = body + last;
                    return id;
                })                         // 18 位合法身份证
                .RuleFor(a => a.Birthday, f => f.Person.DateOfBirth.ToString("yyyy-MM-dd")) // 1990-10-12
                .RuleFor(a => a.Name, f => f.Person.LastName + f.Person.FirstName) // 中文姓名
                .RuleFor(a => a.Sex, f => f.Person.Gender == Bogus.DataSets.Name.Gender.Male ? "男" : "女")
                .RuleFor(a => a.Phone, f => f.Phone.PhoneNumber("13#########"));    // 11 位手机号

        /// <summary>
        /// 12位
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 6位
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// 8位
        /// </summary>
        public string SampleId { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Idnumber { get; set; }

        /// <summary>
        /// 生日, 1990-10-12
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 男/女
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    }
}