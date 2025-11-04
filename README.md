# Sprint 0 - Game Design Document : Tower Defense
Naam: D'onell Fränkel

Klas: SD2B

Datum:

<<<<<<< Updated upstream
1. Titel en elevator pitch
Titel: Carr
=======
# 1. Titel en elevator pitch
## Titel
# Carr
>>>>>>> Stashed changes

Elevator pitch, maximaal twee zinnen: Je bent ANWB en veroorzaakt 'perongelukke' auto-ongelukken om genoeg geld te verdienen.

2. Wat maakt jouw tower defense uniek
Beschrijf in één of twee zinnen wat jouw game onderscheidt van een standaard tower defense. Denk aan iets dat de speler op een nieuwe manier laat nadenken of spelen.

Je schiet auto's in de richtig van andere auto's op de snelweg om zoveel mogelijk schade aan te richten. Hierdoor verdien je geld. Je moet ook ervoor zorgen dat de auto's niet hun bestemming bereiken.

3. Schets van je level en UI
Maak een schets op papier of digitaal en voeg deze afbeelding toe aan je repository. Voeg in deze sectie de afbeelding in.

Je schets bevat minimaal:

Het pad waar de vijanden over lopen met beginpunt en eindpunt.
De plaatsen waar torens gebouwd kunnen worden.
De locatie van de basis of goal die verdedigd moet worden.
De UI onderdelen geld, wave teller, levens, startknop en pauzeknop.
Een legenda met symbolen of kleuren voor torens, vijanden, pad, basis en UI.

![](/Images/Carr%20Sketch.png)
4. Torens
Toren 1 naam: ANWB, bereik: 1, schade: 1, unieke eigenschap: Design.

Toren 2 naam: ANWB+, bereik: 1, schade: 2, unieke eigenschap: Design.

Toren 3 naam: ANWB Pro, bereik: 1, schade: 3, unieke eigenschap: Design.

Toren 4 naam: ANWB Premium, bereik: 1, schade: 4, unieke eigenschap: Design.

Eventuele extra torens:

5. Vijanden
Vijand 1 naam: Student, snelheid: 1, levens: 1, speciale eigenschap: Rood.

Vijand 2 naam: Gezin, snelheid: 2, levens: 3, speciale eigenschap: Groen.

Vijand 3 naam: Celebrity, snelheid: 3, levens: 4, speciale eigenschap: Blauw.

Vijand 4 naam: Bob, snelheid: 4, levens: 7, speciale eigenschap: Oranje.

Vijand 5 naam: Oma, snelheid: 6, levens: 10, speciale eigenschap: Paars.

Eventuele extra vijanden:

6. Gameplay loop
Beschrijf in drie tot vijf stappen wat de speler steeds doet.

- Speler koopt ANWB Tunnel
- Speler raakt vijand
- Speler upgradet de Tunnel OF koopt een nieuwe tunnel.

7. Progressie
Leg uit hoe het spel moeilijker wordt naarmate de waves doorgaan. Denk aan sterkere vijanden, kortere tussenpozen, hogere kosten of lagere beloningen.

Elke wave komen er sterkere, snellere en waardevollere vijanden, maar je moet ook nieuwe tunnels kopen en die upgraden om zo de sterkere auto's te kunnen verslaan en niet failliet gaan. 

8. Risico’s en oplossingen volgens PIO
Probleem 1: Rode Auto

Impact: Je verliest geld

Oplossing: ANWB Busje

Probleem 2: Groene Auto

Impact: Je verliest meer geld

Oplossing: ANWB Busjes OF betere

Probleem 3: Blauwe Auto

Impact: je verliest nog meer geld

Oplossing: ANWB Busjes OF betere

(etc.)

9. Planning per sprint en mechanics
Schrijf per sprint welke mechanics jij oplevert in de build. Denk aan voorbeelden zoals vijandbeweging over een pad, torens plaatsen, doel kiezen en schieten, waves starten, UI voor geld en levens, upgrades, jouw unieke feature.

Sprint 1 mechanics: ANWB Tunnels

Sprint 2 mechanics: UI

Sprint 3 mechanics: vijanden

Sprint 4 mechanics: Upgrades

Sprint 5 mechanics: Map

10. Inspiratie
Noem een bestaande tower defense game die jou inspireert en wat je daarvan meeneemt of juist vermijdt.

Bloons TD: Ik neem de simpelheid over.

11. Technisch ontwerp mini
Lees dit korte voorbeeld en vul daarna jouw eigen keuzes in.

Voorbeeld ingevuld bij 11.1 Vijandbeweging over het pad

Keuze: Vijanden volgen punten A, B, C en daarna de goal.
Risico: Een vijand loopt een punt voorbij of blijft hangen.
Oplossing: Als de vijand dichtbij genoeg is kiest hij het volgende punt. Bij de goal gaat één leven omlaag en verdwijnt de vijand.
Acceptatie: Tien vijanden lopen van start naar de goal zonder vastlopers en verbruiken elk één leven. Alle tien vijanden bereiken achtereenvolgens elk waypoint binnen één seconde na elkaar.
11.1 Vijandbeweging over het pad
Keuze: Alle vijanden volgen dezelfde weg naar de goal.
Risico: Vijand bereikt de goal (Speler verliest geld).
Oplossing: ANWB Bus(jes)
Acceptatie:

11.2 Waves en spawnen
Keuze: Auto's spawnen aan het begin van de weg en rijden naar de goal.
Risico: Speler verliest geld.
Oplossing: ANWB BUSS
Acceptatie:

11.3 Economie en levens
Keuze: Geld < 0 = Failliet
Risico: Failliet als je niet boven de 0 blijft
Oplossing: ANWB BUSJES!
Acceptatie: