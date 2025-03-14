using System.Collections.Generic;
using AllerCheck.API.DTOs.FavoriteListDTO;
using AllerCheck.API.DTOs.BlackListDTO;

namespace AllerCheck.API.DTOs.UserDTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string MailAdress { get; set; }
        public string UserPassword { get; set; }
        public List<FavoriteListDto> FavoriteLists { get; set; }
        public List<BlackListDto> BlackLists { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
