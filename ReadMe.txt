Il gioco è composto da un'interfaccia Python dove è possibile selezionare tra:
1) Snake
2) Tris
3) Classifica

Snake è stato implementato in C# in modo client-based. La logica di gioco è stata implementata con C#, mentre per l'interfaccia grafica si è usato XAML, che viene tipicamente utilizzato assieme al C# per creare applicazioni Desktop. Alla fine della partita lo score viene memorizzato in una classifica che verrà visualizzata attraverso un'interazione di C# con il database phpMyAdmin (Xampp).

Inoltre, è possibile interagire con la classifica mediante l'interfaccia utente iniziale in Python. 

Tris è stato implementato con l'uso di due linguaggi: Python per l'interfaccia grafica e C++ per la costruzione del server e della logica del gioco. La modalità di gioco è player vs computer, dove il player seleziona dove inserire la X nel tabellone di gioco ed il server risponde con O. E' stata, inoltre, implementata la concorrenza. E' possibile, quindi, aprire più schermate di gioco contemporaneamente. 

Il lavoro è stato diviso tra i due membri del gruppo come segue: 
1) Logica ed interfaccia grafica di Snake da Francesco Meli
2) Logica ed interfaccia grafica di Tris da Vincenzo Micieli
3) L'interfaccia utente di scelta dei giochi e le interazioni con il database sono state implementate da entrambi 

Per avviare il gioco, andare nella cartella "InterfacciaIniziale" e selezionare il progetto "InterfacciaIniziale.sln" e avviarlo. Per giocare a Tris bisogna anche avviare il server che è posizionato nella cartella "TrisServer" -> "Debug" -> "TrisServer.exe". E' possibile cliccare più volte "Avvia nuova partita" per giocare a più Tris contemporaneamente. Per giocare, invece, a Snake è possibile farlo anche senza avere il database. Se invece si vuole usare la funzionalità di salvare gli score e visualizzare la classifica bisognerà creare su phpMyAdmin il database "snake_game" ed eseguire questa query: 
USE snake_game;

CREATE TABLE leaderboard (
    id INT AUTO_INCREMENT PRIMARY KEY,
    score INT NOT NULL,
    date DATETIME NOT NULL
);



