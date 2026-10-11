string[] parkingGarage = new string[100];
string menyval;

do
{
    Console.WriteLine();
    Console.WriteLine("=== PRAGUE PARKING V1 ===");
    Console.WriteLine("1. Visa parkeringen");
    Console.WriteLine("2. Parkera fordon");
    Console.WriteLine("3. Sök fordon");
    Console.WriteLine("4. Flytta fordon");
    Console.WriteLine("5. Hämta ut fordon");
    Console.WriteLine("0. Avsluta");
    Console.Write("Välj: ");

    menyval = Console.ReadLine() ?? "";

    switch (menyval)
    {
        case "1":
            VisaParkering(parkingGarage);
            break;

        case "2":
            ParkeraFordon(parkingGarage);
            break;

        case "3":
            SokFordon(parkingGarage);
            break;

        case "4":
            FlyttaFordon(parkingGarage);
            break;

        case "5":
            HamtaUtFordon(parkingGarage);
            break;

        case "0":
            Console.WriteLine("Programmet avslutas.");
            break;

        default:
            Console.WriteLine("Ogiltigt menyval. Försök igen.");
            break;
    }
}
while (menyval != "0");

// Visar innehållet på alla 100 platser.
void VisaParkering(string[] platser)
{
    for (int i = 0; i < platser.Length; i++)
    {
        if (string.IsNullOrEmpty(platser[i]))
        {
            Console.WriteLine("Plats " + (i + 1) + ": Ledig");
        }
        else
        {
            Console.WriteLine("Plats " + (i + 1) + ": " + platser[i]);
        }
    }
}

// Läser och kontrollerar ett registreringsnummer.
string LasRegistreringsnummer()
{
    while (true)
    {
        Console.Write("Registreringsnummer (1–10 tecken): ");
        string regnummer = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

        bool giltigt = regnummer.Length >= 1 && regnummer.Length <= 10;

        foreach (char tecken in regnummer)
        {
            if (!char.IsLetterOrDigit(tecken))
            {
                giltigt = false;
            }
        }

        if (giltigt)
        {
            return regnummer;
        }

        Console.WriteLine("Ogiltigt registreringsnummer. Försök igen.");
    }
}

// Läser ett platsnummer och returnerar dess index.
int LasPlatsIndex()
{
    while (true)
    {
        Console.Write("Ange ny parkeringsplats (1–100): ");
        string inmatning = Console.ReadLine() ?? "";

        if (int.TryParse(inmatning, out int platsnummer) &&
            platsnummer >= 1 && platsnummer <= 100)
        {
            return platsnummer - 1;
        }

        Console.WriteLine("Välj en parkeringsplats mellan 1 och 100.");
    }
}

// Kontrollerar om en plats kan ta emot fordonstypen.
bool KanParkera(string plats, string fordonstyp)
{
    if (string.IsNullOrEmpty(plats))
    {
        return true;
    }

    return fordonstyp == "MC" &&
           plats.StartsWith("MC#") &&
           !plats.Contains("|");
}

// Hittar en plats för ett nytt fordon.
int HittaLedigPlats(string[] platser, string fordonstyp)
{
    for (int i = 0; i < platser.Length; i++)
    {
        if (KanParkera(platser[i], fordonstyp))
        {
            return i;
        }
    }

    return -1;
}

// Hittar fordonets index. -1 betyder att det inte finns.
int HittaFordon(string[] platser, string regnummer)
{
    for (int i = 0; i < platser.Length; i++)
    {
        if (string.IsNullOrEmpty(platser[i]))
        {
            continue;
        }

        string[] fordon = platser[i].Split('|');

        foreach (string ettFordon in fordon)
        {
            string[] uppgifter = ettFordon.Split('#');

            if (uppgifter[1] == regnummer)
            {
                return i;
            }
        }
    }

    return -1;
}

// Hämtar strängen för ett bestämt fordon på en plats.
string HamtaFordonsdata(string plats, string regnummer)
{
    string[] fordon = plats.Split('|');

    foreach (string ettFordon in fordon)
    {
        string[] uppgifter = ettFordon.Split('#');

        if (uppgifter[1] == regnummer)
        {
            return ettFordon;
        }
    }

    return "";
}

// Lägger ett fordon på en plats som redan har kontrollerats.
void LaggTillPaPlats(string[] platser, int index, string fordon)
{
    if (string.IsNullOrEmpty(platser[index]))
    {
        platser[index] = fordon;
    }
    else
    {
        platser[index] += "|" + fordon;
    }
}

// Tar bort ett fordon och behåller eventuell annan MC.
void TaBortFranPlats(string[] platser, int index, string regnummer)
{
    string[] fordon = platser[index].Split('|');
    string kvarvarande = "";

    foreach (string ettFordon in fordon)
    {
        string[] uppgifter = ettFordon.Split('#');

        if (uppgifter[1] != regnummer)
        {
            if (kvarvarande != "")
            {
                kvarvarande += "|";
            }

            kvarvarande += ettFordon;
        }
    }

    platser[index] = kvarvarande;
}

// Hanterar inmatning och parkering av ett nytt fordon.
void ParkeraFordon(string[] platser)
{
    Console.WriteLine("1. Bil");
    Console.WriteLine("2. MC");
    Console.Write("Välj fordonstyp: ");
    string val = Console.ReadLine() ?? "";

    string fordonstyp;

    if (val == "1")
    {
        fordonstyp = "CAR";
    }
    else if (val == "2")
    {
        fordonstyp = "MC";
    }
    else
    {
        Console.WriteLine("Ogiltig fordonstyp. Återgår till menyn.");
        return;
    }

    string regnummer = LasRegistreringsnummer();

    if (HittaFordon(platser, regnummer) != -1)
    {
        Console.WriteLine("Registreringsnumret finns redan på parkeringen.");
        return;
    }

    int index = HittaLedigPlats(platser, fordonstyp);

    if (index == -1)
    {
        Console.WriteLine("Det finns ingen plats för fordonet.");
        return;
    }

    string fordon = fordonstyp + "#" + regnummer;
    LaggTillPaPlats(platser, index, fordon);

    Console.WriteLine("Parkera " + regnummer + " på plats " + (index + 1));
}

// Läser registreringsnummer och visar sökresultatet.
void SokFordon(string[] platser)
{
    string regnummer = LasRegistreringsnummer();
    int index = HittaFordon(platser, regnummer);

    if (index == -1)
    {
        Console.WriteLine("Fordonet finns inte på parkeringen.");
        return;
    }

    Console.WriteLine(regnummer + " står på plats " + (index + 1));
}

// Flyttar ett fordon om målplatsen har utrymme.
void FlyttaFordon(string[] platser)
{
    string regnummer = LasRegistreringsnummer();
    int gammaltIndex = HittaFordon(platser, regnummer);

    if (gammaltIndex == -1)
    {
        Console.WriteLine("Fordonet finns inte på parkeringen.");
        return;
    }

    Console.WriteLine("Fordonet står på plats " + (gammaltIndex + 1));
    int nyttIndex = LasPlatsIndex();

    if (nyttIndex == gammaltIndex)
    {
        Console.WriteLine("Fordonet står redan på den platsen.");
        return;
    }

    string fordon = HamtaFordonsdata(platser[gammaltIndex], regnummer);
    string fordonstyp = fordon.Split('#')[0];

    if (!KanParkera(platser[nyttIndex], fordonstyp))
    {
        Console.WriteLine("Fordonet får inte plats där. Ingen flytt gjordes.");
        return;
    }

    TaBortFranPlats(platser, gammaltIndex, regnummer);
    LaggTillPaPlats(platser, nyttIndex, fordon);

    Console.WriteLine(
        "Flytta " + regnummer +
        " från plats " + (gammaltIndex + 1) +
        " till plats " + (nyttIndex + 1));
}

// Tar bort fordonet vid uthämtning.
void HamtaUtFordon(string[] platser)
{
    string regnummer = LasRegistreringsnummer();
    int index = HittaFordon(platser, regnummer);

    if (index == -1)
    {
        Console.WriteLine("Fordonet finns inte på parkeringen.");
        return;
    }

    TaBortFranPlats(platser, index, regnummer);

    Console.WriteLine(
        "Hämta ut " + regnummer +
        " från plats " + (index + 1) +
        ". Fordonet har tagits bort ur systemet.");
}