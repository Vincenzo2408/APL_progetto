import tkinter as tk
from tkinter import ttk
import subprocess
import sys
import os
import mysql.connector
from tkinter import messagebox

class GameLauncher:
    def __init__(self, master):
        self.master = master
        master.title("Game Launcher")
        
        # Imposta le dimensioni della finestra
        window_width = 420
        window_height = 300
        master.geometry(f"{window_width}x{window_height}")

        # Crea un frame principale
        main_frame = ttk.Frame(master)
        main_frame.pack(fill=tk.BOTH, expand=True)

        # Carica l'immagine
        original_image = tk.PhotoImage(file="Peggy.gif")  
        self.photo = original_image.subsample(original_image.width() // 200, original_image.height() // 200)

        # Crea un label per l'immagine
        image_label = ttk.Label(main_frame, image=self.photo)
        image_label.pack(side=tk.LEFT, padx=20, pady=20)

        # Crea un frame per i pulsanti
        button_frame = ttk.Frame(main_frame)
        button_frame.pack(side=tk.RIGHT, padx=20, fill=tk.Y, expand=False)

        self.label = ttk.Label(button_frame, text="Seleziona gioco")
        self.label.pack(pady=20)

        self.snake_button = ttk.Button(button_frame, text="Snake", command=self.launch_snake)
        self.snake_button.pack(pady=10)

        self.tris_button = ttk.Button(button_frame, text="Tris", command=self.launch_tris)
        self.tris_button.pack(pady=10)

        self.classifica_button = ttk.Button(button_frame, text="Classifica", command=self.chiamaClassifica) 
        self.classifica_button.pack(pady=10)

        # Avvia il server Tris automaticamente
        # self.launch_tris_server()

    def launch_snake(self):
        subprocess.Popen(["SnakeGame.exe"])

    def launch_tris(self):
        subprocess.Popen([sys.executable, "TrisClient.py"])

    def chiamaClassifica(self):
        try:
            # Connessione al database
            conn = mysql.connector.connect(
                host="localhost",
                user="root",
                password="",
                database="snake_game"
            )
            cursor = conn.cursor()

            # Query per ottenere i top 10 punteggi
            query = "SELECT score, date FROM leaderboard ORDER BY score DESC LIMIT 10"
            cursor.execute(query)
            results = cursor.fetchall()

            # Creazione di una nuova finestra per la classifica
            classifica_window = tk.Toplevel(self.master)
            classifica_window.title("Classifica Top 10")
            classifica_window.geometry("400x300")

            # Creazione di un widget Text per visualizzare la classifica
            text_widget = tk.Text(classifica_window, wrap=tk.WORD, width=50, height=15)
            text_widget.pack(padx=10, pady=10)

            # Inserimento dei dati nella finestra
            text_widget.insert(tk.END, "Classifica Top 10:\n\n")
            for i, (score, date) in enumerate(results, 1):
                text_widget.insert(tk.END, f"{i}. Score: {score} - Data: {date.strftime('%d/%m/%Y %H:%M:%S')}\n")

            # Rendi il widget di testo in sola lettura
            text_widget.config(state=tk.DISABLED)

            cursor.close()
            conn.close()

        except mysql.connector.Error as err:
            messagebox.showerror("Errore Database", f"Errore di connessione al database: {err}")
        

    # def launch_tris_server(self):
    # subprocess.Popen(["TrisServer.exe"])

if __name__ == "__main__":
    root = tk.Tk()
    game_launcher = GameLauncher(root)
    root.mainloop()