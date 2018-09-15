using System;
using System.Collections.Generic;

namespace BookLib.Core.Model.Audiobookstore
{
    public class Item
    {
        public string name { get; set; }
    }

    public class ItemListElement
    {
        public int position { get; set; }
        public Item item { get; set; }
    }

    public class Breadcrumb
    {
        public List<ItemListElement> itemListElement { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class ReadBy
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Offers
    {
        public string availability { get; set; }
        public string itemCondition { get; set; }
        public string highPrice { get; set; }
        public string lowPrice { get; set; }
        public string priceCurrency { get; set; }
    }

    public class Publisher
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Brand
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class AggregateRating
    {
        public string ratingValue { get; set; }
        public string bestRating { get; set; }
        public string worstRating { get; set; }
        public string reviewCount { get; set; }
    }

    public class ReviewRating
    {
        public string bestRating { get; set; }
        public string worstRating { get; set; }
        public string ratingValue { get; set; }
    }

    public class Review
    {
        public string author { get; set; }
        public string datePublished { get; set; }
        public string reviewBody { get; set; }
        public ReviewRating reviewRating { get; set; }
    }

    public class MainEntity
    {
        public Author author { get; set; }
        public ReadBy readBy { get; set; }
        public string bookFormat { get; set; }
        public string datePublished { get; set; }
        public string image { get; set; }
        public string isbn { get; set; }
        public string gtin13 { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string timeRequired { get; set; }
        public string genre { get; set; }
        public string category { get; set; }
        public string abridged { get; set; }
        public Offers offers { get; set; }
        public Publisher publisher { get; set; }
        public Brand brand { get; set; }
        public AggregateRating aggregateRating { get; set; }
        public List<Review> review { get; set; }
    }

    public class RootObject
    {
        public Breadcrumb breadcrumb { get; set; }
        public MainEntity mainEntity { get; set; }
    }
}
