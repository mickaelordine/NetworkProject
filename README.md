# Katamari Multiplayer Project

## Descrizione
Questo progetto è un prototipo multiplayer sviluppato in Unity utilizzando Photon PUN per la sincronizzazione di scena tra più client.

La scena consiste in una mappa con un grande pavimento e circa 50 piccoli cubetti neri sparsi. C’è un cubo rosso più grande che il giocatore controlla tramite input fisico.

## Meccaniche principali
Il giocatore muove il cubo rosso liberamente sulla mappa basato interamente sulla fisica.

- Quando il cubo rosso tocca un cubetto nero, questo cambia colore passando gradualmente a rosso.

- Ogni cubetto rosso trasmette parte della sua intensità di rosso ai cubetti vicini, creando un effetto di propagazione del colore.

- I cubetti neri possono essere attratti dal cubo rosso quando sono abbastanza vicini, generando una forza di attrazione.

## Sincronizzazione multiplayer
- Tutti i movimenti del cubo rosso e lo stato colore dei cubetti sono sincronizzati in tempo reale tra più istanze del gioco usando Photon PUN.

- Il sistema garantisce coerenza visiva e di gameplay fra tutti i client collegati.

## Tecnologie
Unity 6
Photon PUN 2
Shader personalizzati per la gestione dinamica del colore dei cubetti

### Contributors
Mickael Ordine
Manuel Sabbadini