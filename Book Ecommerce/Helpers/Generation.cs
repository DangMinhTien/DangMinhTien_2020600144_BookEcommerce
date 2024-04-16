using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Book_Ecommerce.Helpers
{
    public class Generation
    {
        public static string GenerationSlug(string slug)
        {
            // Chuyển đổi chuỗi thành chữ thường và loại bỏ các ký tự không mong muốn
            slug = RemoveDiacritics(slug).ToLower();

            // Thay thế khoảng trắng bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"[-\s]+", "-");

            return slug + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        private static string RemoveDiacritics(string s)
        {
            string normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static decimal RandomTransportFee()
        {
            Random random = new Random();
            int minValue = 10000;
            int maxValue = 100000;

            int randomNumber;
            do
            {
                // Tạo số ngẫu nhiên từ minValue đến maxValue
                randomNumber = random.Next(minValue, maxValue + 1);
            } while (randomNumber % 1000 != 0);
            return randomNumber;
        }
    }
}
