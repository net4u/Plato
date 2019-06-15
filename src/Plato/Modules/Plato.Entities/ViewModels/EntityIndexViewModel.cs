﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.ViewModels
{
  
    public class EntityIndexViewModel<TModel> where TModel : class
    {

        public IPagedResults<TModel> Results { get; set; }

        public PagerOptions Pager { get; set; } = new PagerOptions();

        public EntityIndexOptions Options { get; set; } = new EntityIndexOptions();

        public ICollection<SortColumn> SortColumns { get; set; }

        public ICollection<SortOrder> SortOrder { get; set; }

        public ICollection<Filter> Filters { get; set; }

    }

    public class EntityIndexOptions
    {
        
        [DataMember(Name = "search")]
        public string Search { get; set; }

        [DataMember(Name = "feature")]
        public int? FeatureId { get; set; }
        
        [DataMember(Name = "filter")]
        public FilterBy Filter { get; set; } = FilterBy.All;

        [DataMember(Name = "sort")]
        public SortBy Sort { get; set; } = SortBy.Auto;

        [DataMember(Name = "order")]
        public OrderBy Order { get; set; } = OrderBy.Desc;

        [DataMember(Name = "category")]
        public int CategoryId { get; set; } = -1;

        // ---------------

        [JsonIgnore]
        public int[] CategoryIds { get; set; }

        public int CreatedByUserId { get; set; }

        [JsonIgnore]
        public int LabelId { get; set; }

        [JsonIgnore]
        public int TagId { get; set; }
        
        private IDictionary<string, OrderBy> _sortColumns;

        [JsonIgnore]
        public IDictionary<string, OrderBy> SortColumns
        {
            get => _sortColumns ?? (_sortColumns = new Dictionary<string, OrderBy>());
            set => _sortColumns = value;
        }

    }

    public class SortColumn
    {
        public string Text { get; set; }

        public SortBy Value { get; set; }

    }

    public class SortOrder
    {
        public string Text { get; set; }

        public OrderBy Value { get; set; }

    }

    public class Filter
    {
        public string Text { get; set; }

        public FilterBy Value { get; set; }

    }

    public enum SortBy
    {
        Auto = 0,
        Rank = 1,
        LastReply = 2,
        Replies = 3,
        Views = 4,
        Participants = 5,
        Reactions = 6,
        Follows = 7,
        Stars = 8,
        SortOrder = 9,
        Created = 10,
        Modified = 11,
        IsPinned = 12
    }

    public enum FilterBy
    {
        All = 0,
        Started = 1,
        Participated = 2,
        Following = 3,
        Starred = 4,
        Unanswered = 5,
        NoReplies = 6
    }

}
