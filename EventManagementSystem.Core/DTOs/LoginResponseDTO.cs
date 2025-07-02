namespace EventManagementSystem.API.DTOs.AuthDtos
{
	public class LoginResponseDto
	{
		public string? Email { get; set; }
		public string? AccessToken { get; set; }
		public int ExpiredIn {get; set;}
	}
}
