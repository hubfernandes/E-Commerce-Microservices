namespace WishlistService.Domain.Dtos
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new();
    }
}
