# CheckPrepper

Een hulpmiddel voor docenten om ingeleverde opdrachten van studenten uit Moodle te verwerken en te structureren.

## Doel

CheckPrepper automatiseert het uitpakken en organiseren van studenteninzendingen die via de elektronische leeromgeving Moodle zijn gedownload. Het programma zorgt voor een gestructureerde opzet van de inzendingen, waardoor het nakijkproces eenvoudiger en sneller verloopt.

## Gebruik

1. Download alle inzendingen via Moodle
2. Plaats de gedownloade bestanden in de folder `d:\correctiewerk`
3. Voer het programma uit

## Functionaliteit

Het programma voert automatisch de volgende acties uit:

- **Uitpakken van archieven**: Ondersteunt momenteel ZIP en RAR bestanden
- **Hernoemen van mappen**: Directory wordt hernoemd naar alleen de studentnaam
- **Opschonen**: Verwijdert automatisch `bin` en `obj` mappen uit de projecten

## Op de planning

- Ondersteuning voor GZ (gzip) bestanden
- Ondersteuning voor 7z bestanden
- Rapportage genereren van verwerkte bestanden
- Detectie van ontbrekende inzendingen

## Vereisten

- Windows besturingssysteem
- Schrijfrechten op de `d:\correctiewerk` folder

## Licentie

Dit project is gelicenseerd onder de **GNU General Public License v3.0** met een non-commercial clausule.

Je bent vrij om:
- De software te gebruiken voor educatieve doeleinden
- De software te kopiëren en te verspreiden
- De broncode te wijzigen en aan te passen

Onder de volgende voorwaarden:
- Het gebruik is uitsluitend voor niet-commerciële doeleinden
- Wijzigingen moeten onder dezelfde licentie worden verspreid
- De broncode moet beschikbaar blijven

Zie [LICENSE](LICENSE) voor de volledige licentietekst.
