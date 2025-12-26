using CheckersApi.Contracts;
using CheckersApi.Validation;

namespace CheckersApi.Engine
{
    public class ChinookAdapter : IEngineAdapter
    {
        private readonly ChinookWorkerPool _pool;

        public ChinookAdapter(ChinookWorkerPool pool)
        {
            _pool = pool;
        }

        public SuggestResponse Suggest(SuggestRequest request, CancellationToken ct)
        {
            var pdn = PdnNormalizer.Normalize(request.State.Position);
            if (!PdnValidator.IsValid(pdn))
                throw new ArgumentException("Invalid PDN format");

            // Якщо ≤ 8 фігур → probe tablebase
            if (PdnUtils.CountPieces(pdn) <= 8)
            {
                // TODO: реалізувати probe у ChinookProcess
                // Поки що повертаємо заглушку
                return new SuggestResponse
                {
                    Engine = "chinook",
                    BestMove = "probe-move",
                    Pv = new[] { "probe-move" },
                    ScoreOrWdl = 0,
                    Depth = 0,
                    Nodes = 0,
                    PositionKey = PdnNormalizer.ToPositionKey(pdn),
                    Info = new SuggestInfo { TablebaseHit = true, TimeMs = 0 }
                };
            }

            var (depth, softMs) = SearchLimitsResolver.Resolve(request.Level);

            var worker = _pool.Next();
            var line = worker.SearchAsync(pdn, depth, ct).Result;

            // TODO: розпарсити line → bestMove, pv, nodes, depth, wdl
            return new SuggestResponse
            {
                Engine = "chinook",
                BestMove = "parsed-move",
                Pv = new[] { "parsed-move" },
                ScoreOrWdl = 0,
                Depth = depth,
                Nodes = 0,
                PositionKey = PdnNormalizer.ToPositionKey(pdn),
                Info = new SuggestInfo { TablebaseHit = false, TimeMs = softMs }
            };
        }
    }
}
