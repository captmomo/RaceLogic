﻿using System;
using maxbl4.Race.Logic.Checkpoints;
using maxbl4.Race.Logic.EventModel.Storage.Identifier;
using maxbl4.Race.Logic.EventStorage.Storage.Model;
using maxbl4.Race.Logic.EventStorage.Storage.Traits;

namespace maxbl4.Race.Logic.EventModel.Storage.Model
{
    public class DropCheckpointDto: IHasId<DropCheckpointDto>, IHasTimestamp, IHasSeed
    {
        public Id<DropCheckpointDto> Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsSeed { get; set; }
        
        public Id<CheckpointDto> TargetId { get; set; }
    }
}