namespace PathsEnvariament.Messages;

public static class HelpMessages
{
    public const string Help = """

    ========================================
     short-paths commands
    ========================================

    add / toevoegen
        Nieuwe naam en path opslaan

    list / toon
        Alle opgeslagen variabelen en paths tonen

    check / controleer
        Beschikbaarheid controleren

    pwd
        Huidige werkmap tonen

    ls [-e <extensie>] [-r] [<path>]
        Bestanden en folders tonen
        -e <extensie> Filter op extensie, bijvoorbeeld .txt
        -r Toon type, grootte en wijzigingsdatum

    cd <path>
        Werkmap aanpassen
        Voorbeeld: cd $routePilot

    prompt
        Huidige prompttekst tonen

    prompt <tekst>
        Prompttekst aanpassen

    dev [on|off|help|tools|<commando>]
        Developer mode en developer shortcuts

    Shift+Enter
        Nieuwe invoerregel; Enter voert de invoer uit
        Plak meerdere commando's om ze na elkaar uit te voeren
        Begin met 'cmd' voor een meerregelig CMD-script

    clear / cls
        Console leegmaken

    edit <naam>
        Naam of path bewerken

    delete <naam>
        Opgeslagen path verwijderen

    go <naam>
        Bestand of folder openen

    exit
        Programma sluiten

    ========================================
     Windows CMD
    ========================================

    Normale Windows CMD-commando's kunnen worden
    uitgevoerd via:

    cmd <commando>

    Voorbeeld:
    cmd dir
    cmd whoami
    cmd ipconfig
    cmd echo hello

    """;
}