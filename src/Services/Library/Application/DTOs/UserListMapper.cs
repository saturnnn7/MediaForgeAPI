namespace MediaForge.Library.Application.DTOs;

public static class UserListMapper
{
    public static UserListDto ToDto(this UserList list) =>
        new(
            list.Id,
            list.UserId,
            list.Name,
            list.Slug,
            list.Description,
            list.AvatarUrl,
            list.Privacy.ToString(),
            list.IsSystem,
            list.Items.Count,
            list.CreatedAt);

    public static UserListItemDto ToDto(this UserListItem item) =>
        new(item.Id, item.WorkId, item.DisplayOrder, item.AddedAt);

    public static UserListDetailDto ToDetailDto(this UserList list) =>
        new(
            list.Id,
            list.UserId,
            list.Name,
            list.Slug,
            list.Description,
            list.AvatarUrl,
            list.Privacy.ToString(),
            list.IsSystem,
            list.Items.Count,
            list.CreatedAt,
            list.Items.Select(i => i.ToDto()).ToList());
}
