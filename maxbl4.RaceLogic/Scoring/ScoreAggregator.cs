﻿using System;
using System.Collections.Generic;
using System.Linq;
using maxbl4.RaceLogic.Extensions;

namespace maxbl4.RaceLogic.Scoring
{
    
    //TODO: Port tests from PositionAggregator
    public class ScoreAggregator
    {
        public List<AggRoundScore> Aggregate(List<List<RoundScore>> rounds)
        {
            var rating = new Dictionary<string,AggRoundScore>();
            for (var i = 0; i < rounds.Count; i++)
            {
                var round = rounds[i];
                foreach (var position in round)
                {
                    rating.UpdateOrAdd(position.RiderId,
                        x => x.AddScore(position, i), new AggRoundScore(position.RiderId));
                }
            }
            var maxPoints = rating.Values.Count(x => x.Points > 0);
            var result = rating.Values.OrderBy(x => x)
                .Select((x, i) => new AggRoundScore(x, i + 1, Math.Max(0, maxPoints - i), x.Points))
                .ToList();
            
            return result;
        }
    }
}