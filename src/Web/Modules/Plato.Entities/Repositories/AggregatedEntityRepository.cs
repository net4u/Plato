﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Metrics;

namespace Plato.Entities.Repositories
{
    
    public class AggregatedEntityRepository : IAggregatedEntityRepository
    {

        public IDbHelper DbHelper { get; }

        public AggregatedEntityRepository(IDbHelper dbHelper)
        {
            DbHelper = dbHelper;
        }
        
        // ----------------
        // Grouped by date
        // ----------------

        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(string groupBy, DateTimeOffset start, DateTimeOffset end)
        {
            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(Id) AS [Count], 
                    MAX({groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_Entities
                WHERE 
                    {groupBy} >= '{start}' AND {groupBy} <= '{end}'
                GROUP BY 
                    YEAR({groupBy}),
                    MONTH({groupBy}), 
                    DAY({groupBy})
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{groupBy}"] = groupBy,
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern()
            };

            // Execute and return results
            return await DbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<DateTimeOffset>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<DateTimeOffset>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });

        }
        
        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(string groupBy, DateTimeOffset start, DateTimeOffset end, int featureId)
        {

            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(Id) AS [Count], 
                    MAX({groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_Entities e
                WHERE (
                    ({groupBy} >= '{start}' AND {groupBy} <= '{end}') AND
                    (e.FeatureId = {featureId})
                )
                GROUP BY 
                    YEAR({groupBy}),
                    MONTH({groupBy}), 
                    DAY({groupBy})
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{groupBy}"] = groupBy,
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern(),
                ["{featureId}"] = featureId.ToString()
            };

            // Execute and return results
            return await DbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<DateTimeOffset>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<DateTimeOffset>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });
            
        }
        
    }

}
