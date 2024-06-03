using System;

namespace Models.Entities;

public class Book : Entity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null;
    public Guid AuthorId { get; set; } 
    public User Author { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public string? Series { get; set; }
    public DateTime DateOfPublication { get; set; }
    public int? Price { get; set; } = null;
    public int Likes { get; set; } = 0;
    public StateType State { get; set; } = StateType.IsExisted; 
    public List<BookStatus> Status { get; set; } = [];
    public List<User> AddedInLibrary { get; set; } = [];
    public List<User> AddedInFavorites { get; set; } = [];
    public List<User> BoughtBooks { get; set; } = [];
    public EBook EBook { get; set; } = null!;
    public BookImage? Cover { get; set; } = null;
    public List<Tag> Tags { get; set; } = [];
    public List<Genre> Genres { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];


}
