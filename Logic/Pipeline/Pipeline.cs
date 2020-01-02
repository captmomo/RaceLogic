using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using maxbl4.Race.Logic.Checkpoints;
using maxbl4.Race.Logic.RoundTiming;

namespace maxbl4.Race.Logic.Pipeline
{
    public class Pipeline: IDisposable
    {
        private readonly IFinishCriteria finishCriteria;
        readonly CompositeDisposable disposable;
        private ITrackOfCheckpoints track;
        readonly Subject<List<RoundPosition>> sequence = new Subject<List<RoundPosition>>();
        public IObservable<List<RoundPosition>> Sequence => sequence;

        public Pipeline(IEnumerable<IObservable<Checkpoint>> checkpointProviders,
            IFinishCriteria finishCriteria,
            ICheckpointAggregator checkpointAggregator = null)
        {
            this.finishCriteria = finishCriteria;
            disposable = new CompositeDisposable(checkpointProviders.Select(x =>
            {
                if (checkpointAggregator == null)
                    return x.Subscribe(OnCheckpoint);
                return x.Subscribe(checkpointAggregator);
            }));
            if (checkpointAggregator != null)
                disposable.Add(checkpointAggregator.Subscribe(OnCheckpoint));
        }

        public void StartRound(DateTime roundStartTime)
        {
            track = TrackOfCheckpointsFactory.Create(roundStartTime, finishCriteria);
        }

        public void StopRound()
        {
            track?.ForceFinish();
        }

        void OnCheckpoint(Checkpoint cp)
        {
            if (track == null)
                StartRound(cp.Timestamp);
            track.Append(cp);
            sequence.OnNext(track.Rating);
        }

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}