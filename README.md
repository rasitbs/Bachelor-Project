# Traftec VR Simulator 🛠️⚡

## Proof of concept 📑
Dette repoet er en samling av kode utviklet som et "proof-of-concept" for Traftec's VR Simulator som en del av IKT302-G 26V Bacheloroppgave for Dataingeniører på UiA Grimstad. Hensikten med prosjektet er å kunne bevise ovenfor Traftec at VR kan bidra med opplæring av deres lærlinger og montører på interne HMS rutiner og samtidig la dem "arbeide" under spenning og arbeidsforhold uten ellers eksponering for fare eller støt. Om dette prosjektet faller i smak vil Traftec videreutvikle konseptet for eget bruk. 

## Rammeverk 🖥️
Prosjektet er utviklet i [Unity](https://unity.com/) for Meta plattformens VR briller, da spesifikt [Meta Quest 3](https://www.meta.com/no/quest/quest-3/).

## Strategi 📊
Prosjektet totalt utvikles av tre utviklere som er Dataingeniør studenter som er tilsatt sine egne "origin branches". Hver tilsatte gren bygger opp mot en `dev` gren som brukes for å løse konflikter og klargjøre sammenfelte oppdateringer for å dyttes ut på `main`. Tankegangen er da;

1. Utvikler har utviklet og testet sin kode lokalt på VR briller.
2. Utvikler "pusher" sin kode opp på sin "origin branch".
3. Utvikler "merger" sin "origin branch" med `dev` og bruker auto konflikt løsning for å løse eventuelle konflikter i koden.
4. Repo administrator, ved Scrum møte, "merger" `dev` med `main` slik at utviklere kan kjøre gjennom total simulasjonen og teste i felles.
5. `main` versjoneres og er klar for opplastning som en `.apk` på den aktuelle VR brillen.

## Beskrivelse av Simulator

Simulatoren er brutt ned til tre hoved "nivåer" til etterspørsel fra Traftec og skal simulere skifte av lys armatur ved en avsideliggende vei som innebærer arbeid i høyden:

1. Scene 1 er til for valg av riktig Personlig Verneutstyr (PVU) samt utstyr til arbeid. Spiller blir tilbudt en rekke ferdig definerte arbeids samlinger av PVU og utstyr. Basert på en lokalt tilgjengelig samling med HMS rutiner skal spiller kunne finne hvilken av de tre samlingene som stemmer til arbeidet som skal utføres og velge riktig "kit" før de kan gå videre med arbeidet.
2. Scene 2 er til for å bedrive en simplifisert Sikker Jobb Analyse (SJA). Spiller skal kunne markere ut faremomenter på arbeidplass og ved at alle farer blir markert kan de levere inn analysen og gå videre til faktisk Arbeid Under Spenning (AUS).
3. Scene 3 er til for å simulere det faktiske arbeidet som skal gjennomføres i henhold til de predefinerte rutinene montører blir gitt og opplært i. Spiller skal kunne ha en større grad av frihet i denne scenen og blir gitt muligheten til å begå grove feil for å kunne leke seg med konseptet samt utforske faremomenter uten risiko for faktisk støt. Ved ferdig utført arbeidsprosedyre vil spiller bli overført til en sluttscene som viser frem en poengsum basert på riktighet av utført arbeid.

Arbeid utført blir loggført av MQTT hendelser lagret av et eksternt database system på en Virtuell Privat Server (VPS).  

## Mål 🏁
Når VR simulatoren er ferdig utviklet skal den brukes for å måle læringsoppnåelse hos lærlinger hvor igjen den aktuelle statistikken skal fremvises slik at Traftec kan ta en beslutning på om prosjektet er verd å investere mer tid og ressurser i. Disse resultatene vil bli tilgjengeligjort som en del av bachelor oppgaven.
