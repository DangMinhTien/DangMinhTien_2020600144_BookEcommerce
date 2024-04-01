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

            return slug;
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
    }
}
