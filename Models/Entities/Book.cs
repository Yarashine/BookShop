using System;

namespace Models.Entities;

public class Book : Entity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null;
    public Guid AuthorId { get; set; } 
    public virtual User Author { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public string? Series { get; set; }
    public DateTime DateOfPublication { get; set; }
    public int? Price { get; set; } = null;
    public int Likes { get; set; } = 0;
    public StateType State { get; set; } = StateType.IsExisted; 
    public List<BookStatus> Status { get; set; } = [];
    public virtual List<User> AddedInLibrary { get; set; } = [];
    public virtual List<User> AddedInFavorites { get; set; } = [];
    public virtual List<User> BoughtBooks { get; set; } = [];
    public virtual EBook EBook { get; set; } = null!;
    public virtual BookImage? Cover { get; set; } = null;
    public virtual List<Tag> Tags { get; set; } = [];
    public virtual List<Genre> Genres { get; set; } = [];
    public virtual List<Comment> Comments { get; set; } = [];


}
