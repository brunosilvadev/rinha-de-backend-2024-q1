using rinha.model;

namespace rinha.transacao;

public class ValidatorService : IValidatorService
{
    public bool Overdraft {get;set;}
    public bool NaoExiste {get;set;}

    public bool ValidaRequest(TransacaoRequest txn)
    {
        if(string.IsNullOrEmpty(txn.Descricao))
        {
            return false;
        }
        if(txn.Descricao.Length > 10)
        {
            return false;
        }
        if(!int.TryParse(txn.Valor.ToString(), out _))
        {
            return false;
        }
        if(txn.Tipo != 'c' && txn.Tipo != 'd')
        {
            return false;
        }
        
        return true;
    }
}

public interface IValidatorService
{
    bool Overdraft {get;set;}
    bool NaoExiste {get;set;}
    bool ValidaRequest(TransacaoRequest txn);
}