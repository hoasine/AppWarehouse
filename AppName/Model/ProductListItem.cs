using System;
using System.Collections.Generic;
using System.Text;

namespace AppName.Model
{
    public class ProductListItem
    {
        public ProductListItem(int id, string name, string type, byte[] image)
        {
            this.ID = id;
            this.DisplayName = name;
            this.Type = type;
            this.Image = image;
        }

        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public byte[] Image { get; set; }
        public string DisplayType { get { return "Barcode: " + Type; } }
    }

    public class ArticleData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public string Section { get; set; }
        public string Author { get; set; }
        public string Avatar { get; set; }
        public string BackgroundImage { get; set; }
        public string Quote { get; set; }
        public string QuoteAuthor { get; set; }
        public string When { get; set; }
        public string Followers { get; set; }
        public string Likes { get; set; }
    }

    public class NavigationItemData
    {
        public string Tittle { get; set; }
        public string MenuID { get; set; }
        public bool IsVisible { get; set; }
        public string Name { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public int Badge { get; set; }
        public int ItemCount { get; set; }
    }

    public class NavigationCategoryData
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
