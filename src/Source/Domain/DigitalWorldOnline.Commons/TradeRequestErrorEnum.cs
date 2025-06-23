
namespace DigitalWorldOnline.Commons.Enums
{
    public enum TradeRequestErrorEnum : int
    {
        othertransact = 30023,//Estou em outros negócios. Não é possível solicitar a transação
        inventoryfull = 11015,// nenhum espaço vazio na minha bolsa
        otherinventoryfull = 30031 // Sem espaço vazio na bolsa do oponente
    }
}
