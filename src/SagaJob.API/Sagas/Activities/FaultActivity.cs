using MassTransit;

namespace SagaJob.API.Sagas.Activities
{
    public class FaultActivity<TSaga, TFault> : IStateMachineActivity<TSaga, Fault<TFault>>
        where TSaga : class, SagaStateMachineInstance
        where TFault : class
    {
        private readonly ILogger<FaultActivity<TSaga, TFault>> _logger;
        private readonly ISendEndpointProvider _sendEndpoint;
        private readonly Uri _faultQueue;
        private readonly (string SagaName, string FaultName) _activityInfo = (typeof(TSaga).Name, typeof(TFault).Name);

        public FaultActivity(ILogger<FaultActivity<TSaga, TFault>> logger, ISendEndpointProvider sendEndpoint, string faultQueue = "RE.URL.TOKENEXPORT.ERROR")
        {
            //1. trazer configurações de filas e exchanges do appsettings utilizando o IOptions<T>
            //2. Verificar a melhor opção entre exchange/queue/topico
            //https://masstransit.io/documentation/concepts/messages#commands
            //https://masstransit.io/documentation/concepts/producers

            _faultQueue = new Uri($"queue:{faultQueue}");
            _logger = logger;
            _sendEndpoint = sendEndpoint;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TSaga, Fault<TFault>> context, IBehavior<TSaga, Fault<TFault>> next)
        {
            //ajustar para no futuro pegar todas as excessões, não apenas a primeira da lista
            var firstException = context.Message.Exceptions.FirstOrDefault();
            if (firstException != null)
            {
                //1. Enviar a mensagem para a fila de erro
                _logger.LogDebug($"FaultQueue:{_faultQueue} - FaultActivity<{_activityInfo.SagaName},{_activityInfo.FaultName}>: {firstException.Message}");
            }
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, Fault<TFault>, TException> context, IBehavior<TSaga, Fault<TFault>> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("sagajob-faulted");
        }
    }
}
