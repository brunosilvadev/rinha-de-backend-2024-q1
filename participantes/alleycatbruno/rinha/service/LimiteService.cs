namespace rinha.transacao;

public class ErrorService : IErrorService
{
    public bool Overdraft {get;set;}
    public bool NaoExiste {get;set;}
}

public interface IErrorService
{
    bool Overdraft {get;set;}
    bool NaoExiste {get;set;}
}