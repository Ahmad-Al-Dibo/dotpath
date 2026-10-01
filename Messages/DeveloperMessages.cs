namespace dotpath.Messages;

public static class DeveloperMessages
{
    public const string Help = """

    Developer mode
        dev                 Developer mode aan/uit
        dev on | dev off    Developer mode instellen
        dev tools           Beschikbare developer tools tonen
        dev status          Git-status van de huidige map tonen
        dev build [args]    dotnet build uitvoeren
        dev test [args]     dotnet test uitvoeren
        dev run [args]      dotnet run uitvoeren

    Andere tools kunnen direct worden aangeroepen:
        dev git <args>
        dev dotnet <args>
        dev npm <args>
        dev node <args>
        dev python <args>
        dev code <args>

    Voorbeeld: dev git log --oneline -5
    In developer mode worden echte commando-uitvoeren als subword-tokens getoond.
    De oorspronkelijke uitvoer en witruimte blijven behouden.

    """;
}