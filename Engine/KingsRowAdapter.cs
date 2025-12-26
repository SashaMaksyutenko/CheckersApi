using System.Text;
using CheckersApi.Contracts;
using CheckersApi.Validation;

namespace CheckersApi.Engine
{
    public class KingsRowAdapter : IEngineAdapter
    {
        private readonly string _dbPath;
        private readonly bool _useInit;

        public KingsRowAdapter(string dbPath, bool useInit = false)
        {
            _dbPath = dbPath;
            _useInit = useInit;

            KingsRowBootstrap.Initialize(_dbPath, _useInit); // UseInit має бути false
        }

        public SuggestResponse Suggest(SuggestRequest request, CancellationToken ct)
        {
            var sb = new StringBuilder(8192);
            int rc;

            try
            {
                rc = NativeKingsRow.get_best_moves(
                    request.State.Position,
                    request.Limits?.MaxDepth ?? 8,
                    sb,
                    sb.Capacity
                );
            }
            catch (ExecutionEngineException ex)
            {
                throw new InvalidOperationException($"get_best_moves crashed: {ex.Message}", ex);
            }

            if (rc != 0 || sb.Length == 0)
                throw new InvalidOperationException($"KingsRow failed. rc={rc}, pos={request.State.Position}");

            return new SuggestResponse
            {
                Engine = "kingsrow",
                BestMove = sb.ToString(),
                PositionKey = PdnNormalizer.ToPositionKey(request.State.Position),
                Info = new SuggestInfo
                {
                    TablebaseHit = false,
                    TimeMs = 0,
                    Evaluation = NativeKingsRow.staticevaluation(request.State.Position)
                }
            };
        }
    }
}