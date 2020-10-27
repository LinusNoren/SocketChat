Krav
* Programmet ska vara gjort i C#/.NET
* Programmet skall kunna köras i flera instanser
* Den första instansen som startas ska automatiskt fungera som server
* De övriga instanserna blir klienter
* Varje klient skall automatiskt koppla upp sig med en socket mot servern
* Klient och server skall köras från samma exe-fil
* När en klient kopplar upp sig ska alla aktiva klienter, utom den nya, få ett meddelande om att en kopplat upp sig.
* Man ska kunna skicka meddelanden mellan klienter
* Någon av klasserna i System.Net.Sockets skall användas (utan färdiga hjälpbibliotek runt)

Begränsningar av krav
* Programmet kan med fördel göras som Command Line Interface, men det är inte ett krav
* Alla klienter körs från samma dator
* Vi är inte intresserade av ett snyggt GUI. Lägg inte tid på det
* Du behöver inte överdriva felhanteringen. Du kan utgå från att vi använder programmet som det är tänkt
