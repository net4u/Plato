﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Plato.Internal.Abstractions;

namespace Plato.Categories.Models
{
    public interface ICategory
    {

        int Id { get; set; }

        int ParentId { get; set; }

        int FeatureId { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Alias { get; set; }

        string IconCss { get; set; }

        string ForeColor { get; set; }

        string BackColor { get; set; }

        int SortOrder { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        IEnumerable<CategoryData> Data { get; set; } 

        IDictionary<Type, ISerializable> MetaData { get; }

        IList<ICategory> Children { get; set; }

        int Depth { get; set; }

        void AddOrUpdate<T>(T obj) where T : class;

        void AddOrUpdate(Type type, ISerializable obj);

        T GetOrCreate<T>() where T : class;

        void PopulateModel(IDataReader dr);

    }

}
