namespace AllerCheck.API.DTOs.BlackListDTO
{
    public class BlackListDto
    {
        public int BlackListId { get; set; }
        public int UserId { get; set; }
        public int ContentId { get; set; }
        public string ContentName { get; set; }
    }
} 