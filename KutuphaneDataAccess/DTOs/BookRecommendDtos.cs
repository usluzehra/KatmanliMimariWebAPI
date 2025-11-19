using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneDataAccess.DTOs
{
    // record:“veri tutmak için kullanılan, immutable (değişmez) ve değer eşitliği (value equality) odaklı bir sınıf türü”
    public class BookCandidate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int CountOfPage { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }

    };

    public record RecommendRequest(string Query, int? MaxResults =5);

    public record BookRecommendationDto(int Id, string Title, string Reason);

    public record RecommendationResponse(string Summary, List<BookRecommendationDto> Items);
}
