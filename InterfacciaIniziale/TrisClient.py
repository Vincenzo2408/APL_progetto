import socket
import tkinter as tk
from tkinter import messagebox
import threading

class TicTacToeGame:
    def __init__(self, host='localhost', port=12345):
        #Inizializzazione della connessione e dell'interfaccia grafica
        self.host = host
        self.port = port
        self.client_socket = None
        self.window = None
        self.buttons = None
        self.frame = None

    def connect_and_play(self):
        #Connessione al server e creazione dell'interfaccia di gioco
        self.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.client_socket.connect((self.host, self.port))
        
        #Creazione della finestra di gioco
        self.window = tk.Toplevel()
        self.window.title("Tic Tac Toe")
        self.window.geometry("300x300")
        
        #Frame principale per contenere i pulsanti
        self.frame = tk.Frame(self.window)
        self.frame.pack(expand=True, fill=tk.BOTH)
        
        #Creazione della griglia di pulsanti 3x3
        self.buttons = [[None for _ in range(3)] for _ in range(3)]
        for i in range(3):
            self.frame.grid_rowconfigure(i, weight=1)
            self.frame.grid_columnconfigure(i, weight=1)
            for j in range(3):
                self.buttons[i][j] = tk.Button(self.frame, text="", font=('normal', 20),
                                               command=lambda row=i, col=j: self.make_move(row, col))
                self.buttons[i][j].grid(row=i, column=j, sticky="nsew")
        
        # Binding per il ridimensionamento della finestra
        self.window.bind("<Configure>", self.resize_buttons)

        #Avvio del thread per ricevere le mosse del server
        threading.Thread(target=self.receive_moves, daemon=True).start()

    def resize_buttons(self, event=None):
        # Ridimensiona i pulsanti in base alle dimensioni della finestra
        width = self.frame.winfo_width() // 3
        height = self.frame.winfo_height() // 3
        font_size = min(width, height) // 3
        
        for i in range(3):
            for j in range(3):
                self.buttons[i][j].config(font=('normal', font_size))

    def make_move(self, row, col):
        #Gestisce la mossa del giocatore
        if self.buttons[row][col]['text'] == "":
            self.buttons[row][col]['text'] = "X"
            self.send_move(row, col)

    def send_move(self, row, col):
        #Invia la mossa al server
        move = f"{row},{col}"
        self.client_socket.send(move.encode())

    def receive_moves(self):
        #Riceve e gestisce le mosse del server e gli stati di gioco
        while True:
            try:
                data = self.client_socket.recv(1024).decode()
                if data == "WIN":
                    self.game_over("Hai vinto!")
                    break
                elif data == "LOSE":
                    self.game_over("Hai perso!")
                    break
                elif data == "DRAW":
                    self.game_over("Pareggio!")
                    break
                else:
                    self.window.after(0, self.update_board, data)
            except:
                break

    def update_board(self, data):
        #Aggiorna la board con la mossa del server
        row, col = map(int, data.split(','))
        self.buttons[row][col]['text'] = "O"

    def game_over(self, message):
        #Gestisce la fine del gioco
        messagebox.showinfo("Fine partita", message, parent=self.window)
        self.window.destroy()

class TicTacToeClient:
    def __init__(self):
        #Inizializzazione del client principale
        self.root = tk.Tk()
        self.root.title("Tic Tac Toe - Menu Principale")
        self.root.geometry("300x200")
        
        self.games = []
        
        #Frame principale per i pulsanti del menu
        self.frame = tk.Frame(self.root)
        self.frame.pack(expand=True, fill=tk.BOTH, padx=20, pady=20)
        
        #Pulsanti del menu principale
        start_button = tk.Button(self.frame, text="Inizia Nuova Partita", command=self.start_new_game)
        start_button.pack(expand=True, fill=tk.BOTH, pady=10)
        
        exit_button = tk.Button(self.frame, text="Esci", command=self.root.quit)
        exit_button.pack(expand=True, fill=tk.BOTH, pady=10)
        
        self.root.bind("<Configure>", self.resize_buttons)
        
    def resize_buttons(self, event=None):
        #Ridimensiona i pulsanti del menu principale 
        font_size = min(self.frame.winfo_width(), self.frame.winfo_height()) // 10
        for widget in self.frame.winfo_children():
            if isinstance(widget, tk.Button):
                widget.config(font=('normal', font_size))

    def start_new_game(self):
        #Avvia una nuova partita
        self.games = [game for game in self.games if game.window and game.window.winfo_exists()]
        
        if len(self.games) < 10:
            game = TicTacToeGame()
            game.connect_and_play()
            self.games.append(game)
            game.window.protocol("WM_DELETE_WINDOW", lambda g=game: self.remove_game(g))
        else:
            messagebox.showwarning("Limite raggiunto", "Puoi giocare al massimo 10 partite contemporaneamente.")

    def remove_game(self, game):
        #Rimuove una partita dalla lista quando viene chiusa
        if game in self.games:
            self.games.remove(game)
        game.window.destroy()

    def run(self):
        #Avvia il loop principale dell'applicazione
        self.root.mainloop()

if __name__ == "__main__":
    client = TicTacToeClient()
    client.run()