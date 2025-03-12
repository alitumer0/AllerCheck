namespace AllerCheck.API.DTOs.ContentDTO
{
    public class ContentDto
    {
        public int ContentId { get; set; }
        public string ContentName { get; set; }
        public int RiskStatusId { get; set; }
        public string RiskStatusName { get; set; }
        public string ContentInfo { get; set; }
    }
} 