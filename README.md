![Logo](img/mas_general_sm.svg)
# Markuse asjade tekstiliidesed

Tekstiliidesed Markuse asjade jaoks.

## Markuse arvuti juhtpaneel (MasCpanel)

Projekti eesmärk on asendada "Markuse arvuti tööriistad" bash skript ja pakkuda samasugust liidest igale toetatud Markuse asjade operatsioonsüsteemile (Windows/macOS/Linux).

Projekt on hilises Alpha arendusjärgus.

### Kompileerimine

Projekti ehitamiseks on vajalik .NET 10.0 raamistik, SDK ja PowerShell. Kui olete PowerShellis, saate rakenduse kompileerida järgmiste käsklustega:

```
Set-ExecutionPolicy Bypass -Scope Process
.\build.ps1
```

### Eelvaade (must-valge)

#### Avaleht

```
 <     Avaleht      >      MarkuStation        Konfiguratsioon            Teave         

  < Taaskäivita Markuse asjad     > 
    Taaskäivita töölaud             
    Taaskäivita audioserver         
    Taaskäivita valgustus           
    Taaskäivita klaviatuuriekraan   

 Taaskäivitab Markuse asjade integratsiooniprogrammi ja M.A.I.A. serveri 
 ```

#### MarkuStation

```
       Avaleht         <   MarkuStation   >    Konfiguratsioon            Teave         

╔═Mängud═══════════════════════════════════════════════════════════════════════════════╗
║ < Minecraft                    /usr/bin/prismlauncher                              > ║
║   TrackMania                   /usr/bin/steam steam://rungameid/2225070              ║
║   PCSX2                        /usr/bin/pcsx2-qt                                     ║
║   Steam                        /usr/bin/steam                                        ║
║   Lutris                       /usr/bin/lutris                                       ║
╚══════════════════════════════════ U Lisa mäng   DEL Kustuta   ↵ Muuda   ↕ Vali mäng ═╝

 [✓] (L)uba introd              (M)onitor režiim: Laienda            
 [ ] (C)reepypasta intro        (R)akenda muudatused
 [ ] (P)ärandintro
 [ ] (S)pecial intro
```

#### Konfiguratsioon

```
       Avaleht             MarkuStation      < Konfiguratsioon  >         Teave         

Taustad:  (V)aheta miniversioon töölauataustaga

 Töölaud (1)                           Logimisekraan (2)                      Miniversioon (3)
 ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀   ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀    ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀
 ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀   ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀    ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀
 ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀   ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▄▀▀▀▀▀▀▀▀▀▀    ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀
 ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀   ▀▀▀▀▀▀▀▀▄▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀    ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀
 ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀   ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀    ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀

 Integratsioon:                                   Värvid:
  [✓] (K)uva Markuse asjade logo käivitumisel     Ta(u)st:      #0F3168
  [✓] Lub(a) ajastatud sündmused                  (E)siplaan:   #FDFDFD
  [ ] Käivi(t)a töölauamärkmed käivitumisel
  < Pollimise sagedus: 5000ms (+/-) >
```

#### Teave
```
       Avaleht             MarkuStation        Konfiguratsioon     <      Teave       > 

         Markuse arvuti asjad 52 – Heksagon       Funktsioonid:
         Pro                                       ✓ Interaktiivne töölaud
         HG530000a                                 ✗ Rainmeter
                                                   ✓ Integratsioonitarkvara
Keel: et-EE                                        ✓ Klassikaline start menüü
Juurutatud: Jah                                    ✓ Standardfunktsioonid
Kasutaja: markus                                   ✓ Kaugjuhtimine
Kinnituskood: 4262                                 ✓ Windows 10+
Olek: VERIFIED                                     ✗ LiveTuner optimeerimised
Räsi: 2da1e9ea32                                   ✓ Grupipoliitika

 ↵  Laadi andmed uuesti (salvestamata muudatused lähevad kaotsi)
```


## Markuse mälupulk (MasFlashDrv)

Sisuliselt tekstiliidesel põhinev alternatiiv mälupulga juhtpaneelile.

Projekt on Alpha arendusjärgus.

Implementeeritud funktsionaalsus:

* Uudiste lugemine (postitused Markuse asjad ajaveebist)
* Uusimate videote vaatamine
* Mälupulga teabe vaatamine