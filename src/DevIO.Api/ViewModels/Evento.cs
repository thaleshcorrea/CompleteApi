namespace DevIO.Api.ViewModels;

public class Evento
{
    public RetornoEvento Evento001 { get; set; }
}

public class RetornoEvento
{
    public int nProt { get; set; }
    public string xMotivo { get; set; }
}